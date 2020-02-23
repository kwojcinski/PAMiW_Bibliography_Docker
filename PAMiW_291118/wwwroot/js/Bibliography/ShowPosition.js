function download(link) {
    $.get(link, function (data) {
    });
}
function disconnect(link, guid) {
    if (confirm("Czy na pewno chcesz odłączyć ten plik?")) {
        $.ajax({
            url: link + guid,
            type: 'PATCH',
            success: function (data) {
                alert("Pomyślnie odłączono!");
                location.reload();
            }
        });
    }
}