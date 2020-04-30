$(document).ready(function () {
    ShowProduct(0, 0);
});

function ShowProduct(categoryID, colorID, searchString) {
    FetchData("/Product/Show", { categoryID: categoryID, colorID: colorID, searchString: searchString }).done(function (productList) {
        $("#ProductList").html(productList);
    });
}

$(document).on("keyup", "#txtSearchProduct", function () {
    ShowProduct(0, 0, $(this).val());
});