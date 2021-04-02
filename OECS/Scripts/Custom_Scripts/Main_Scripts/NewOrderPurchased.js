$(document).ready(function () {
    ShowOrderDetailSummaryCard();
});

function ShowOrderDetailSummaryCard() {
    FetchData("/Order/OrderDetailCard", null).done(function (content) {
        $(".order-details").html(content);
        ShowPurchasedItemTable();
    });
}

function ShowPurchasedItemTable() {
    FetchData("/Order/PurchasedItemTable", null).done(function (content) {
        $("#PurchasedItemTable").html(content);
        ShowOrderSummaryAccordion();
    });
}

function ShowOrderSummaryAccordion() {
    FetchData("/Order/OrderSummaryAccordion", null).done(function (content) {
        $("#OrderSummaryAccordion").html(content);
    });
}
