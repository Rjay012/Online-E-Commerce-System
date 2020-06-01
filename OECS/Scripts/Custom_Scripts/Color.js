$(document).ready(function () {
    
});

function ShowProductColor(productCategoryID) {
    FetchData("/Color/ShowProductColor", { productCategoryID: productCategoryID }).done(function (ProductColor) {
        $("#ProductColorList").html(ProductColor);
    });
}

function SortByColor(colorID) {
    var categoryID = $(".active").children(".category").attr("categoryID");
    ShowProduct(categoryID, colorID, 0, "");
}

$(document).one("click", "#accordionEx80", function () {
    ShowProductColor(0);
});