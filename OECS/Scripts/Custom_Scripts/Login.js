$(document).on("click", "#BtnLogin", function () {
    $("#BtnConfirmLogin").click();
});

function RedirectToLandingPage(response) {
    if (response.data == "success") {
        if (response.shopNow == true) {
            location.reload(true);
        }
        else {
            window.location.href = "/" + response.controller;
        }
    }
}

$(document).on("click", "#BtnLoginViewForm, .btn-shop", function () {
    var btn = $(this);

    FetchData("/Account/LoginForm", "").done(function (loginForm) {
        $("#RegisterOrLoginFormContent").html(loginForm);
        //bypass
        $("#RoleID").val(3);
        $("#UserID").val("q@gmail.com");
        $("#Password").val("q");
        $("#ShopNow").val(false);
        if (btn.hasClass("btn-shop")) {
            $("#ShopNow").val(true);
        }
        //$("#BtnConfirmLogin").click();
    });
});