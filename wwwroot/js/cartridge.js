function filterCartridgeTable() {

    const firmFilter =
        document.getElementById('firmFilter').value;

    const modelFilter =
        document.getElementById('modelFilter').value;

    const statusFilter =
        document.getElementById('statusFilter').value;

    const locationFilter =
        document.getElementById('locationFilter')
            .value
            .toLowerCase();


    const rows =
        document.querySelectorAll('.cartridge-row');


    rows.forEach(row => {

        const firm =
            row.getAttribute('data-firm');

        const model =
            row.getAttribute('data-model');

        const status =
            row.getAttribute('data-status');

        const location =
            (row.getAttribute('data-location') || '')
                .toLowerCase();


        const visible =
            (firmFilter === "" || firm === firmFilter) &&
            (modelFilter === "" || model === modelFilter) &&
            (statusFilter === "" || status === statusFilter) &&
            (locationFilter === "" ||
                location.includes(locationFilter));


        row.style.display =
            visible ? "" : "none";
    });
}


function printSingleCartridge(id) {

    const element =
        document.getElementById('single-' + id);

    if (!element) {
        return;
    }

    element.classList.add('printable-active');

    window.print();

    element.classList.remove('printable-active');
}


function printAllCartridges() {

    const zone =
        document.getElementById('mass-print-zone');

    zone.innerHTML = '';


    const rows =
        document.querySelectorAll('.cartridge-row');


    rows.forEach(row => {

        if (row.style.display !== "none") {

            const sticker =
                row.querySelector('.print-sticker');

            if (sticker) {

                const clone =
                    sticker.cloneNode(true);

                clone.removeAttribute('id');

                zone.appendChild(clone);
            }
        }
    });


    zone.classList.add('printable-active');

    window.print();

    zone.classList.remove('printable-active');

    zone.innerHTML = '';
}