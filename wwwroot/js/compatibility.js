document.addEventListener('DOMContentLoaded', function () {

    const firmSelect =
        document.getElementById('firmSelect');

    const newFirmBlock =
        document.getElementById('newFirmBlock');

    const newFirmInput =
        document.getElementById('newFirmInput');


    if (!firmSelect) {
        return;
    }


    firmSelect.addEventListener('change', function () {

        if (this.value === '-1') {

            newFirmBlock.style.display = 'block';

            newFirmInput.required = true;

            newFirmInput.focus();

        }
        else {

            newFirmBlock.style.display = 'none';

            newFirmInput.required = false;

            newFirmInput.value = '';
        }

    });

});