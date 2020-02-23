function remove(link) {
    if (confirm("Czy na pewno chcesz usunąć ten plik?")) {
        $.ajax({
            url: link,
            type: 'DELETE',
            success: function (result) {
                alert("Pomyślnie usunięto!");
                location.reload();
            }
        });
    }
}