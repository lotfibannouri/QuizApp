// Surveillance du passage de test : acces webcam, captures et detection de perte de focus.
//
//cimporte depuis .NET via IJSRuntime.InvokeAsync<IJSObjectReference>("import", ...).
// Volontairement PAS charge par une balise <script> dans index.html : un index.html en cache
// suffisait alors a laisser window.proctoring indefini, et la surveillance echouait
// silencieusement. Ici le module est resolu au moment ou le composant en a besoin, et son
// absence remonte immediatement une erreur exploitable.

let stream = null;
let videoElement = null;
let dotNetRef = null;
let hiddenSince = null;
let listenersAttached = false;

function stopTracks() {
    if (stream) {
        stream.getTracks().forEach(t => t.stop());
        stream = null;
    }
    if (videoElement) {
        videoElement.srcObject = null;
        videoElement = null;
    }
}

function onVisibilityChange() {
    if (document.hidden) {
        hiddenSince = Date.now();
    } else if (hiddenSince !== null) {
        const seconds = Math.round((Date.now() - hiddenSince) / 1000);
        hiddenSince = null;
        if (dotNetRef) {
            // Appel JS -> .NET : le composant journalise l'incident cote serveur.
            dotNetRef.invokeMethodAsync("OnTabBlur", seconds);
        }
    }
}

// La fermeture d'onglet ne declenche pas Dispose cote Blazor : sans cet ecouteur,
// le voyant de la webcam resterait allume.
window.addEventListener("pagehide", stopTracks);

// Rattache le flux a un element <video>. Indispensable au-dela du premier appel :
// Blazor detruit le <video> de l'ecran de consentement et en cree un nouveau pour
// l'epreuve. Sans ce reattachement, le module continue de pointer l'element supprime,
// qui n'est plus rafraichi : apercu noir cote nouveau <video>, et captures toutes
// identiques car figees sur la derniere image de l'ancien.
export async function attach(videoId) {
    if (!stream) {
        return false;
    }

    const element = document.getElementById(videoId);
    if (!element) {
        return false;
    }

    videoElement = element;
    videoElement.srcObject = stream;
    await videoElement.play().catch(() => { });
    return true;
}

export async function start(videoId, dotNetReference) {
    try {
        stream = await navigator.mediaDevices.getUserMedia({ video: true, audio: false });
    } catch (e) {
        return { ok: false, reason: e && e.name ? e.name : "Erreur inconnue" };
    }

    await attach(videoId);

    dotNetRef = dotNetReference;
    if (!listenersAttached) {
        document.addEventListener("visibilitychange", onVisibilityChange);
        window.addEventListener("blur", onVisibilityChange);
        listenersAttached = true;
    }

    return { ok: true, reason: null };
}

// Retourne une image JPEG en base64, ou null si le flux n'est pas encore exploitable.
export function capture(maxWidth) {
    if (!stream || !videoElement || !videoElement.videoWidth) {
        return null;
    }

    const width = Math.min(maxWidth || 320, videoElement.videoWidth);
    const scale = width / videoElement.videoWidth;
    const height = Math.round(videoElement.videoHeight * scale);

    const canvas = document.createElement("canvas");
    canvas.width = width;
    canvas.height = height;
    canvas.getContext("2d").drawImage(videoElement, 0, 0, width, height);

    return canvas.toDataURL("image/jpeg", 0.6);
}

// ---------------------------------------------------------------------------
// Detection de presence : signale quand l'etudiant sort du champ de la camera.
//
// L'analyse tourne entierement dans le navigateur (MediaPipe Face Detection) :
// aucune image supplementaire ne transite sur le reseau, seuls les incidents
// remontent. Le modele et le runtime WASM sont charges depuis un CDN, donc
// l'initialisation peut echouer hors ligne : dans ce cas on renvoie l'echec au
// lieu de l'ignorer, pour qu'une surveillance inactive soit tracee en base et
// non confondue avec une absence d'incident.
// ---------------------------------------------------------------------------

// Runtime et modele servis par l'application, pas par un CDN. Le chargeur WASM
// d'Emscripten tente d'abord WebAssembly.instantiateStreaming, qui exige que le .wasm
// arrive avec le type MIME application/wasm et sans redirection ; sinon il retombe sur
// une branche qui relit une Response deja consommee -> "Cannot compile WebAssembly.Module
// from an already read Response". Servir les fichiers nous-memes supprime les deux causes,
// et rend la surveillance utilisable hors ligne.
//
// Attention aux deux formes de chemin ci-dessous, elles ne se resolvent PAS pareil :
//  - l'import ESM est relatif a CE module (/scripts/proctoringInterop.js)
//  - les chemins passes a MediaPipe sont des URL fetchees a l'execution, resolues
//    depuis la racine du site (<base href="/">)
const MEDIAPIPE_MODULE = "./mediapipe/vision_bundle.mjs";
const MEDIAPIPE_WASM = "/scripts/mediapipe/wasm";
// Extension .bin et non .tflite : ASP.NET Core ne sert pas les extensions absentes de sa
// table MIME, et .tflite n'y figure pas -> le modele repondrait 404. MediaPipe telecharge
// ce fichier en binaire brut, son extension ne lui importe pas.
const FACE_MODEL = "/scripts/mediapipe/models/blaze_face_short_range.bin";

let faceDetector = null;
let faceTimer = null;
let missingSince = null;
let missingReported = false;

// Etat de la derniere analyse, expose par getFaceWatchStatus(). Sans cela, un detecteur
// qui ne tourne pas est indiscernable d'une session sans incident : les sorties anticipees
// de checkFace sont silencieuses par conception (une frame illisible ne doit rien conclure).
let lastCheck = { at: null, skipped: null, faces: null, error: null };

// Mettre a false une fois la surveillance validee en conditions reelles.
const FACE_DEBUG = true;

function log(...args) {
    if (FACE_DEBUG) {
        console.info("[proctoring]", ...args);
    }
}

// Previent le composant Blazor d'un changement d'etat de presence, pour qu'il affiche ou
// retire le bandeau d'avertissement. Distinct de OnFaceMissing, qui enregistre l'incident :
// ici on informe l'etudiant, la on constitue la preuve.
function notifyPresence(present, secondsMissing) {
    if (!dotNetRef) {
        return;
    }
    dotNetRef.invokeMethodAsync("OnPresenceChanged", present, secondsMissing);
}

function checkFace(thresholdSeconds) {
    lastCheck.at = new Date().toISOString();
    lastCheck.error = null;

    // Chaque cas est trace separement : "pas de detecteur", "pas d'element video" et
    // "video sans dimensions" ont des causes tres differentes.
    if (!faceDetector) {
        lastCheck.skipped = "detecteur absent";
        log("analyse ignoree : detecteur absent");
        return;
    }
    if (!videoElement) {
        lastCheck.skipped = "element video absent";
        log("analyse ignoree : element video absent");
        return;
    }
    if (!videoElement.videoWidth) {
        lastCheck.skipped = "flux sans dimensions (video masquee ou en pause ?)";
        log("analyse ignoree : videoWidth =", videoElement.videoWidth);
        return;
    }

    lastCheck.skipped = null;

    let faces = 0;
    try {
        const result = faceDetector.detectForVideo(videoElement, performance.now());
        faces = result && result.detections ? result.detections.length : 0;
    } catch (e) {
        // Frame non exploitable (video en pause, changement de resolution) : on ne conclut
        // rien et on retentera au tick suivant. Mais on garde la trace de l'erreur, sinon
        // un detecteur qui echoue a chaque appel passerait totalement inapercu.
        lastCheck.error = e && e.message ? e.message : String(e);
        lastCheck.faces = null;
        log("detectForVideo a echoue :", lastCheck.error);
        return;
    }

    lastCheck.faces = faces;

    if (faces > 0) {
        if (missingSince !== null) {
            log("visage retrouve apres", Math.round((Date.now() - missingSince) / 1000), "s");
            notifyPresence(true, 0);
        }
        missingSince = null;
        missingReported = false;
        return;
    }

    if (missingSince === null) {
        missingSince = Date.now();
        log("aucun visage detecte : debut du decompte");
        // Averti des la premiere analyse sans visage, et non au seuil : c'est ce qui laisse
        // a l'etudiant une chance de revenir avant que l'incident ne soit enregistre.
        notifyPresence(false, 0);
        return;
    }

    // Un seul signalement par episode : l'etudiant qui s'absente longuement ne
    // doit pas generer un incident toutes les deux secondes.
    const seconds = Math.round((Date.now() - missingSince) / 1000);

    // Avant le test de missingReported : le decompte affiche doit continuer d'avancer
    // meme apres l'enregistrement de l'incident, tant que l'etudiant est hors champ.
    notifyPresence(false, seconds);

    if (missingReported) {
        return;
    }
    if (seconds < thresholdSeconds) {
        log("aucun visage depuis", seconds, "s (seuil", thresholdSeconds, "s)");
        return;
    }
    if (!dotNetRef) {
        lastCheck.error = "reference .NET absente : incident non transmis";
        log(lastCheck.error);
        return;
    }

    missingReported = true;
    log("SIGNALEMENT : aucun visage depuis", seconds, "s");
    dotNetRef.invokeMethodAsync("OnFaceMissing", seconds);
}

/// Etat interne de la surveillance, pour diagnostic depuis la console.
export function getFaceWatchStatus() {
    return {
        detectorLoaded: !!faceDetector,
        timerRunning: !!faceTimer,
        hasVideo: !!videoElement,
        videoWidth: videoElement ? videoElement.videoWidth : null,
        hasDotNetRef: !!dotNetRef,
        missingSince: missingSince,
        missingReported: missingReported,
        lastCheck: lastCheck
    };
}

if (FACE_DEBUG) {
    // Poignee de diagnostic : un module ES n'est pas accessible depuis la console, alors
    // que c'est justement la qu'on a besoin de lire cet etat. Disparait avec FACE_DEBUG.
    window.__proctoringStatus = getFaceWatchStatus;
}

export async function startFaceWatch(intervalMs, thresholdSeconds) {
    if (faceTimer) {
        return { ok: true, reason: null };
    }
    if (!stream) {
        return { ok: false, reason: "flux video indisponible" };
    }

    // Blazor et MediaPipe embarquent CHACUN un runtime Emscripten, et le code genere par
    // Emscripten commence invariablement par :
    //     var Module = typeof Module != 'undefined' ? Module : {};
    // MediaPipe adopte donc l'objet Module global publie par dotnet.js, avec son callback
    // instantiateWasm cable sur dotnet.wasm et sur une Response deja consommee. D'ou
    // "Cannot compile WebAssembly.Module from an already read Response".
    //
    // La pile d'appel le montre sans ambiguite :
    //     createWasm (vision_wasm_internal.js) -> instantiateWasm (blazor.webassembly.js)
    //
    // On masque donc Module pendant le chargement : MediaPipe repart alors sur un objet
    // neuf et instancie son propre .wasm. Meme precaution pour define/require, qu'Emscripten
    // teste aussi et que Monaco (scripts/min/vs/loader.js) publie globalement.
    const saved = {
        Module: window.Module,
        define: window.define,
        require: window.require
    };

    window.Module = undefined;
    if (typeof saved.define === "function" && saved.define.amd) {
        window.define = undefined;
        window.require = undefined;
    }

    try {
        const vision = await import(MEDIAPIPE_MODULE);
        const fileset = await vision.FilesetResolver.forVisionTasks(MEDIAPIPE_WASM);
        faceDetector = await vision.FaceDetector.createFromOptions(fileset, {
            baseOptions: { modelAssetPath: FACE_MODEL },
            runningMode: "VIDEO"
        });
        log("detecteur charge");
    } catch (e) {
        faceDetector = null;
        const reason = e && e.message ? e.message : "chargement du modele impossible";
        log("ECHEC du chargement du detecteur :", reason, e);
        return { ok: false, reason: reason };
    } finally {
        // finally : la restauration doit avoir lieu meme si le chargement echoue, sinon un
        // echec de MediaPipe priverait Blazor de son Module et Monaco de son chargeur AMD.
        // La fenetre de masquage dure le temps du chargement, soit une a deux secondes.
        window.Module = saved.Module;
        window.define = saved.define;
        window.require = saved.require;
    }

    missingSince = null;
    missingReported = false;
    faceTimer = setInterval(() => checkFace(thresholdSeconds), intervalMs);

    log("surveillance demarree : analyse toutes les", intervalMs, "ms, seuil", thresholdSeconds, "s");
    return { ok: true, reason: null };
}

export function stopFaceWatch() {
    if (faceTimer) {
        clearInterval(faceTimer);
        faceTimer = null;
    }
    if (faceDetector) {
        try {
            faceDetector.close();
        } catch (e) {
            // Detecteur deja libere : rien a rattraper.
        }
        faceDetector = null;
    }
    missingSince = null;
    missingReported = false;
}

export function stop() {
    stopFaceWatch();

    if (listenersAttached) {
        document.removeEventListener("visibilitychange", onVisibilityChange);
        window.removeEventListener("blur", onVisibilityChange);
        listenersAttached = false;
    }
    dotNetRef = null;
    hiddenSince = null;
    stopTracks();
}
