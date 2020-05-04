$(document).on("click", "#BtnRegisterViewForm", function () {
    FetchData("/Account/RegisterForm", "").done(function (loginForm) {
        $("#RegisterOrLoginFormContent").html(loginForm);
        $('.mdb-select').material_select();
    });
});

$(document).on("click", "#BtnSignUp", function () {
    $("#BtnConfirmSignUp").click();
});