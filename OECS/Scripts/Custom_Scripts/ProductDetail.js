$(document).ready(function () {
    PreviewImages(routeID, null, null);
    ViewTab();
});

$(document).on("change", "#SizeID", function () {
    $(".quantity").val(null);  //reset quantity first to null
    $(".quantity, .minus, .plus").attr("disabled", true);

    var optSize = $(this).children("option:selected").text().split("---");
    var size = optSize[1].trim().split(" ");

    if (size[0] > 1) {
        $(".quantity, .minus, .plus").attr("disabled", false);
        $(".quantity").val(1);
        $(".quantity").attr("max", size[0]);
    }
});

function PreviewImages(productID, colorID, iconID) {
    FetchData("/Product/PreviewProductDetails", { productID: productID, colorID: colorID, iconID: iconID }).done(function (content) {
        $(".product-image-gallery").html(content);
    });
}

function ViewTab() {
    FetchData("/Product/ViewTab").done(function (content) {
        $(".tab-list").html(content);
    });
}