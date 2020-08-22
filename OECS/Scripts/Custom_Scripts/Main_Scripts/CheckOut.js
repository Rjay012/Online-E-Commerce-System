$(document).ready(function () {
    LoadPackage();
    LoadShippingAndOrderSummary();
});

$(document).on("click", "#BtnPaymentMethodConfirmation", function () {
    LoadPartials("PaymentMethodConfirmationModal", "#paymentMethodConfirmation");
});

function LoadPackage() {
    LoadPartials("LoadPackage", ".checkout-package");
}

function LoadShippingAndOrderSummary() {
    LoadPartials("ShippingAndOrderSummary", ".checkout-shipping-order-summary");
}

function LoadPartials(actionMethod, container) {
    FetchData("/CheckOut/" + actionMethod, null).done(function (content) {
        $(container).html(content);
    });
}