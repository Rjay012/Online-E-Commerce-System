$(document).one("click", "#accordionEx79", function () {
    ShowProductSize(0);
});

function ShowProductSize(productCategoryID) {
    FetchData("/Size/ShowProductSize", { productCategoryID: productCategoryID }).done(function (ProductSize) {
        $("#ProductSizeList").html(ProductSize);
    });
}

function SortBySize(sizeID) {
    var categoryID = $(".active").children(".category").attr("categoryID");
    ShowProduct(categoryID, 0, sizeID, "");
}