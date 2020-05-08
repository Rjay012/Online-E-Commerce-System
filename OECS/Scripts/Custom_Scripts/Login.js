$(document).on("click", "#BtnLogin", function () {
    $("#BtnConfirmLogin").click();
});

function RedirectToLandingPage(data) {
    if (data != "failed") {
        window.location.href = "/" + data.controller + "/" + data.action;
    }
}

$(document).on("click", "#BtnLoginViewForm", function () {
    FetchData("/Account/LoginForm", "").done(function (loginForm) {
        $("#RegisterOrLoginFormContent").html(loginForm);
        $("select").addClass("browser-default");
    });
});