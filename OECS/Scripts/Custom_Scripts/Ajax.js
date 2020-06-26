var FetchData = function (url, data) {
    return $.ajax({
        url: url,
        data: data,
        type: "post"
    });
};

//ajax response
function Success(response) {
    if (response.data == "success") {
        toastr.success("Request Successful", "Success", { "positionClass": "md-toast-top-right" });
    }
    ResetButton();
}

function Complete() {
    ResetButton();
}

function ResetButton() {
    //enable button and set to its default content
    $(".working-button").html("SAVE");
    $(".working-button").removeClass("working-button disabled");
}