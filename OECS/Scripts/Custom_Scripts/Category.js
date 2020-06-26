$(document).ready(function () {
    ShowCategory();
});

$(document).on("click", ".category", function () {
    var categoryID = parseInt($(this).attr("categoryID"));
    ShowProduct(categoryID, 0, 0, 0, 0, "");
    ShowProductBrand(categoryID, GetSubCategoryID());
    ShowProductColor(categoryID, GetSubCategoryID());
    ShowProductSize(categoryID, GetSubCategoryID());
    $(this).parent().siblings().removeClass("active");
    $(this).parent().addClass("active");
    ShowSubCategory();
});

$(document).on("click", "input[name='subCategory']", function () {
    var subCategoryID = parseInt($(this).attr("id"));

    ShowProduct(GetCategoryID(), subCategoryID, 0, 0, 0, "");
    ShowProductBrand(GetCategoryID(), subCategory);
    ShowProductColor(GetCategoryID(), subCategoryID);
    ShowProductSize(GetCategoryID(), subCategoryID);
});

$(document).one("click", "#accordionEx77", function () {
    ShowSubCategory();
});

function ShowSubCategory() {
    FetchData("/Category/SubCategoryList", { categoryID: GetCategoryID() }).done(function (subCategory) {
        $("#SubCategoryList").html(subCategory);
    });
}

function ShowCategory() {
    FetchData("/Category/CategoryList", null).done(function (category) {
        $("#NavChooseCategory").html(category);
    });
}

function GetCategoryID() {
    return parseInt($(".active").children(".category").attr("categoryID"));
}

function GetSubCategoryID() {
    var SubCategoryID = parseInt($("input[name='subCategory']:checked").attr("id"));
    return isNaN(SubCategoryID) ? 0 : SubCategoryID;
}