function filterPrinterTable() {
    const modelFilter = document.getElementById('modelFilter');
    const cartridgeFilter = document.getElementById('cartridgeFilter');

    const selectedModel = modelFilter.value;
    const selectedCartridge = cartridgeFilter.value;

    const rows = document.querySelectorAll('.printer-row');

    rows.forEach(row => {

        const model = row.getAttribute('data-model');
        const cartridge = row.getAttribute('data-cartridge');

        const visible =
            (selectedModel === "" || model === selectedModel) &&
            (selectedCartridge === "" || cartridge === selectedCartridge);

        row.style.display = visible ? "" : "none";
    });
}


function printFilteredQr() {

    const stickers = document.querySelectorAll('.print-sticker');

    stickers.forEach(sticker => {
        sticker.style.display = 'none';
    });

    const visibleRows =
        document.querySelectorAll('.printer-row:not([style*="display: none"])');

    visibleRows.forEach(row => {

        const id = row.getAttribute('data-id');

        const sticker =
            document.getElementById('sticker-' + id);

        if (sticker) {
            sticker.style.display = 'flex';
        }
    });

    window.print();
}


function printSingleQr(id) {

    const stickers =
        document.querySelectorAll('.print-sticker');

    stickers.forEach(sticker => {

        sticker.style.display =
            sticker.id === 'sticker-' + id
                ? 'flex'
                : 'none';
    });

    window.print();
}