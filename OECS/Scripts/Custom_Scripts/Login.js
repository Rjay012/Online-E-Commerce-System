$(document).ready(function () {
    ReadyLoginForm();  //first to appear in navigation tabs
});

function ReadyLoginForm() {
    FetchData("/Login/LoginForm", "").done(function (loginForm) {
        $("#panel555").html(loginForm);
        $('.mdb-select').material_select();
    });
}

$(document).on("click", "#BtnLogin", function () {
    $("#BtnConfirmLogin").click();
});

function RedirectToLandingPage(data) {
    if (data != "failed") {
        window.location.href = "/" + data.controller + "/" + data.action;
    }
}