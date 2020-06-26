$(document).ready(function () {
    
});

function ShowProductColor(categoryID, subCategoryID) {
    FetchData("/Color/ShowProductColor", { categoryID: categoryID, subCategoryID: subCategoryID }).done(function (ProductColor) {
        $("#ProductColorList").html(ProductColor);
    });
}

function SortByColor(colorID) {
    ShowProduct(GetCategoryID(), GetSubCategoryID(), 0, colorID, 0, "");
}

$(document).one("click", "#accordionEx80", function () {
    ShowProductColor(GetCategoryID(), GetSubCategoryID());
});