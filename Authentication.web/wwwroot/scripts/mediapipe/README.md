# MediaPipe Face Detection — fichiers hébergés localement

Utilisés par `wwwroot/scripts/proctoringInterop.js` pour détecter la sortie du champ de la
caméra pendant un passage de test (incident `NoFace`).

## Pourquoi ces fichiers sont dans le dépôt

Ils étaient initialement chargés depuis `cdn.jsdelivr.net`, ce qui échouait avec :

```
TypeError: Failed to execute 'compile' on 'WebAssembly':
Cannot compile WebAssembly.Module from an already read Response
```

Le chargeur WASM d'Emscripten tente d'abord `WebAssembly.instantiateStreaming`, qui exige
que le `.wasm` arrive avec le type MIME `application/wasm` et sans redirection. En cas
d'échec il retombe sur une branche qui relit une `Response` déjà consommée — d'où ce
message, qui masque la cause réelle. Servir les fichiers depuis l'application supprime les
deux causes possibles, et permet à la surveillance de fonctionner hors ligne.

## Provenance

`@mediapipe/tasks-vision` **0.10.14**, depuis
`https://cdn.jsdelivr.net/npm/@mediapipe/tasks-vision@0.10.14` :

| Fichier local | Source |
|---|---|
| `vision_bundle.mjs` | `/vision_bundle.mjs` |
| `wasm/vision_wasm_internal.js` | `/wasm/vision_wasm_internal.js` |
| `wasm/vision_wasm_internal.wasm` | `/wasm/vision_wasm_internal.wasm` |
| `wasm/vision_wasm_nosimd_internal.js` | `/wasm/vision_wasm_nosimd_internal.js` |
| `wasm/vision_wasm_nosimd_internal.wasm` | `/wasm/vision_wasm_nosimd_internal.wasm` |

Modèle : **BlazeFace short-range** (float16), depuis
`https://storage.googleapis.com/mediapipe-models/face_detector/blaze_face_short_range/float16/1/blaze_face_short_range.tflite`

## Le modèle porte l'extension `.bin`, pas `.tflite`

Volontaire. ASP.NET Core ne sert pas les extensions absentes de sa table MIME, et
`.tflite` n'y figure pas : le fichier répondrait 404. MediaPipe le télécharge en binaire
brut, son extension lui est indifférente. Ne pas le renommer sans adapter `FACE_MODEL`
dans `proctoringInterop.js`.

## Mise à jour

Remplacer les fichiers par ceux de la nouvelle version, en gardant l'arborescence et le
renommage du modèle en `.bin`, puis recompiler pour régénérer le manifeste des ressources
statiques.
