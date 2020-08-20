$(document).ready(function () {
    LoadCartItems();
    LoadOrderSummary();
});

function LoadCartItems() {
    FetchData("/Cart/LoadCartTable", null).done(function (content) {
        $(".cart-item").html(content);
    });
}

function LoadOrderSummary() {
    FetchData("/Cart/LoadOrderSummary", null).done(function (content) {
        $(".order-summary").html(content);
    });
}