$(document).ready(function () {
    ReadyRegisterForm();
});

function ReadyRegisterForm() {
    FetchData("/Login/RegisterForm", null).done(function (registerForm) {
        $("#panel666").html(registerForm);
    });
}

function Load(data) {
    alert(JSON.stringify(data));
}

$(document).on("click", "#BtnSignUp", function () {
    $("#BtnConfirmSignUp").click();
});