$(document).ready(function () {
    ShowProduct(0, 0, 0, 0, 0, "");
});

function ShowProduct(categoryID, subCategoryID, brandID, colorID, sizeID, searchString) {
    FetchData("/Product/Show", { categoryID: categoryID, subCategoryID: subCategoryID, brandID: brandID, colorID: colorID, sizeID: sizeID, searchString: searchString }).done(function (productList) {
        $("#ProductList").html(productList);
    });
}

function ViewProductDetail(productID) {
    window.location.href = "Product/ViewFullDetail/" + productID;
}

$(document).on("keyup", "#txtSearchProduct", function () {
    ShowProduct(0, 0, 0, 0, 0, $(this).val());
});

$(document).on("click", ".myPopover", function () {
    var item = parseInt($(this).attr("item"));
    
    $(this).popover({   //activate color popover
        html: true,
        title: '<h6 class="custom-title">Available Colour(s)</h6>',
        content: $("#popover-" + item).html(),
        placement: "bottom"
    });
    $(".img-thumbnail").css({ "height": "28px", "width": "28px" });
});

$(document).on("click", ".product-icon", function () {
    var id = $(this).attr("id").split("|~|");  //id as path and productID
    $("#card-img-" + id[1]).attr("src", id[0]);
});

$(document).on("change", "#customSelectionSwitches", function () {
    if ($(this).is(":checked")) {
        $(this).addClass("selection-active");
    }
    else {
        $(this).removeClass("selection-active");
        $(this).html("");
    }
});