$(document).ready(function () {
    ShowCategory();
});

function choose() {
    alert("Chose!");
}

function ShowCategory() {
    FetchData("/Category/Show", "").done(function (category) {
        $("#NavChooseCategory").html(category);
    });
}