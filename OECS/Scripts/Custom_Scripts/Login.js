$(document).ready(function () {
    ReadyLoginForm();  //first to appear in navigation tabs
});

function ReadyLoginForm() {
    FetchData("/Login/LoginForm", null).done(function (loginForm) {
        $("#panel555").html(loginForm);
    });
}