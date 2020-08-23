$(document).ready(function () {
    ShowPurchasedGreetingCard();
    ShowOrderDetailsSummaryCard();
});

function ShowPurchasedGreetingCard() {
    LoadPartials("PurchasedGreetingCard", ".purchased-greeting-card");
}

function ShowOrderDetailsSummaryCard() {
    LoadPartials("OrderDetailSummaryCard", ".order-details");
    LoadPartials("PurchasedItemTable", "#PurchasedItemTable");
    LoadPartials("OrderSummaryAccordion", "#OrderSummaryAccordion");
}

function LoadPartials(actionMethod, container) {
    FetchData("/OrderPurchased/" + actionMethod, null).done(function (content) {
        $(container).html(content);
    });
}