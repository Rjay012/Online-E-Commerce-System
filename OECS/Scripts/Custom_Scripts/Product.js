$(document).ready(function () {
    ShowProduct();
});

function ShowProduct() {
    FetchData("/Product/Show", "").done(function (productList) {
        $("#ProductList").html(productList);
    });
}