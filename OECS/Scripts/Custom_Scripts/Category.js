$(document).ready(function () {
    ShowCategory();
});

function ShowCategory() {
    FetchData("/Category/Show", "").done(function (category) {
        $("#NavChooseCategory").html(category);
    });
}

$(document).on("click", ".category", function () {
    var id = parseInt($(this).attr("categoryID"));
    ShowProduct(id, 0, "");
    ShowProductColor(id);
    $(this).parent().siblings().removeClass("active");
    $(this).parent().addClass("active");
});