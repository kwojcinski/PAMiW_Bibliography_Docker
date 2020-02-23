function DeletePosition(link) {
    if (confirm("Czy na pewno chcesz usunąć tę pozycję?")) {
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