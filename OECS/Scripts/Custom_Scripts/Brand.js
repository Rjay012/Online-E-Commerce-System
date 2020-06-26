$(document).ready(function () {
    
});

function ShowProductBrand(categoryID, subCategoryID) {
    FetchData("/Brand/BrandList", { categoryID: categoryID, subCategoryID: subCategoryID }).done(function (brand) {
        $("#BrandList").html(brand); 
    });
}

function SortByBrand(brandID) {
    ShowProduct(GetCategoryID(), GetSubCategoryID(), brandID, 0, 0, "");
}

$(document).one("click", "#accordionEx78", function () {
    ShowProductBrand(GetCategoryID(), GetSubCategoryID());
});

