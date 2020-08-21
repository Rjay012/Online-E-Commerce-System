$(document).ready(function () {
    LoadPackage();
    LoadShippingAndOrderSummary();
});

function LoadPackage() {
    FetchData("/CheckOut/LoadPackage", null).done(function (content) {
        $(".checkout-package").html(content);
    });
}

function LoadShippingAndOrderSummary() {
    FetchData("/CheckOut/ShippingAndOrderSummary", null).done(function (content) {
        $(".checkout-shipping-order-summary").html(content);
    });
}