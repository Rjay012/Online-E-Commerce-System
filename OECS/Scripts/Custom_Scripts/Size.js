$(document).one("click", "#accordionEx79", function () {
    ShowProductSize(GetCategoryID(), GetSubCategoryID());
});

function ShowProductSize(categoryID, subCategoryID) {
    FetchData("/Size/ShowProductSize", { categoryID: categoryID, subCategoryID: subCategoryID }).done(function (ProductSize) {
        $("#ProductSizeList").html(ProductSize);
    });
}

function SortBySize(sizeID) {
    ShowProduct(GetCategoryID(), GetSubCategoryID(), 0, 0, sizeID, "");
}