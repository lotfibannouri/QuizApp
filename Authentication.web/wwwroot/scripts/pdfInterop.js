// Ouverture d'un PDF recu depuis .NET sous forme de tableau d'octets.
// Convention du projet : fonctions globales sur window, appelees par JSRuntime.InvokeAsync.
window.openPdf = (bytes) => {
    const blob = new Blob([new Uint8Array(bytes)], { type: "application/pdf" });

    const url = URL.createObjectURL(blob);

    window.open(url, "_blank");
};
