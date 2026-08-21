function assignRequests() {

    const ids =
        $('.assign-checkbox:checked')
            .map(function () {
                return $(this).attr('data-request-id');
            })
            .toArray();


    const data = {
        assign: $('#assigne').val(),
        requests: ids
    };


    console.log(data);


    $.ajax({
        type: "POST",

        url: "/api/" +
            requestController +
            "Rest/AssignTo",

        data: JSON.stringify(data),

        contentType: "application/json",

        success: function () {
            location.reload();
        },

        error: function (xhr) {

            console.error(
                "Ошибка при назначении заявок:",
                xhr
            );

            alert("Не удалось назначить заявки.");
        }
    });
}