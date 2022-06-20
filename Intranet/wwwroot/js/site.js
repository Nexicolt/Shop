
function DeleteSelectedRows(tableId) {
    let table = $(`#${tableId}`);
    table.find('input[type="checkbox"]').each((index, child) => {

        if (child.checked) {
            let rowContainingCheckbox = $(child).parent().parent();

            $.ajax({
                url: window.location.href + "/RemoveByPost",
                method: "post",
                dataType: "json",
                data: {
                    id: parseInt($(child).val())
                },
                success: function (result) {
                    if (result.success) {
                        ToastSuccess(result.message);
                        rowContainingCheckbox.remove();
                    } else {
                        ToastError(result.message);
                    }
                }
            });
        }


    });
}
function ToastSuccess(message) {
    Toastify({
        text: message,
        duration: 3000,
        newWindow: true,
        close: true,
        gravity: "top", // `top` or `bottom`
        position: "right", // `left`, `center` or `right`
        style: {
            background: "green",
        },
    }).showToast();
}

function ToastError(message) {
    Toastify({
        text: message,
        duration: 3000,
        newWindow: true,
        close: true,
        gravity: "top", // `top` or `bottom`
        position: "right", // `left`, `center` or `right`
        style: {
            background: "red",
        },
    }).showToast();
}