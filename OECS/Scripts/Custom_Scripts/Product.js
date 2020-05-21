$(document).ready(function () {
    ShowProduct(0, 0, "");
});

function ShowProduct(categoryID, colorID, searchString) {
    FetchData("/Product/Show", { categoryID: categoryID, colorID: colorID, searchString: searchString }).done(function (productList) {
        $("#ProductList").html(productList);
    });
}

$(document).on("keyup", "#txtSearchProduct", function () {
    ShowProduct(0, 0, $(this).val());
});

$(document).on("click", ".myPopover", function () {
    var item = parseInt($(this).attr("item"));
    
    $(this).popover({   //activate color popover
        html: true,
        title: '<h6 class="custom-title">Available Colour(s)</h6>',
        content: $("#popover-" + item).html(),
        placement: "auto"
    });
    $(".img-thumbnail").css({ "height": "28px", "width": "28px" });
});

$(document).on("click", ".product-icon", function () {
    var id = $(this).attr("id").split("|~|");  //id as path and productID
    $("#card-img-" + id[1]).attr("src", id[0]);
});