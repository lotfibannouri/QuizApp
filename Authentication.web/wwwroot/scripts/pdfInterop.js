<script>
        window.openPdf = (bytes) => {
            const blob = new Blob(
    [new Uint8Array(bytes)],
    {type: "application/pdf" }
    );

    const url = URL.createObjectURL(blob);

    window.open(url, "_blank");
        };
</script>