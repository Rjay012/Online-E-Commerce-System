$(document).ready(function () {
    LoadPackage();
});

$(document).on("click", "#BtnPaymentMethodConfirmation", function () {
    FetchData("/CheckOut/PaymentMethodConfirmationModal", null).done(function (content) {
        $("#paymentMethodConfirmation").html(content);

        //calculate total amount to pay
        var subTotal = parseFloat($("#hidTotalAmountCheckout").val());
        var vat = subTotal * 0.12;
        var total = subTotal + vat;

        //show to shopper
        $("#checkout-subtotal-amount").html(CurrencyFormat(subTotal));
        $("#checkout-vat").html(CurrencyFormat(vat));
        $("#checkout-amount-payable").html(CurrencyFormat(total));

        $('.datepicker').pickadate({
            format: 'yyyy-mm-dd'
        });
    });
});

$(document).one("click", "#BtnOTC", function () {
    GetCheckoutItems();
});

function FinishedPurchased() {  //redirect
    window.location.href = "Order/NewOrderPurchased";
}

function GetCheckoutItems() {          //get checkout items id and quantity
    FetchData("/CheckOut/GetCheckoutItem", null).done(function (item) {
        var orderData = [];
        $.each(item.CheckoutItem, function (indx) {
            var orderNumber = "";
            var isFound = true;
            do {
                orderNumber = GenerateOrderNumber();
                isFound = CheckOrderNumber(orderNumber);
                
                if (isFound == false) {
                    orderData.push({
                        OrderNumber: orderNumber,
                        ProductID: item.CheckoutItem[indx].ProductID,
                        Quantity: item.CheckoutItem[indx].Quantity,
                        ShippingAddress: $("#checkout-shipping-address").html(),
                        Price: parseFloat(item.CheckoutItem[indx].Price) * parseInt(item.CheckoutItem[indx].Quantity)
                    });
                    break;
                }
            }
            while (isFound == true);  //stop when order number is uniquely identified
        });
        CheckoutOrder(orderData);
    });
}

function CheckoutOrder(orderData) {
    FetchData("/CheckOut/Checkout", { orders: orderData }).done(function () {
        var subTotal = parseFloat($("#hidTotalAmountCheckout").val());
        $("#PaymentTypeID").val(1);
        $("#Amount").val(subTotal + (subTotal * 0.12));
        $("#BtnConfirmOTC").click();
    });
}

function CheckOrderNumber(orderNumber) {   //check order number if available
    FetchData("/CheckOut/CheckOrderNumberAvailability", { orderNumber: orderNumber }).done(function (response) {
        if (response.isAvailable == true) {
            return true;
        }
    });
    return false;
}

function GenerateOrderNumber() {
    let now = Date.now().toString(); // '1492341545873'

    // pad with extra random digit
    now = now + Math.floor(Math.random() * 10);
    var mid = Math.random(now).toString();

    // format
    return [now.slice(0, 4), mid.slice(2, 8), now.slice(10, 14)].join('-');
}

function LoadPackage() {
    FetchData("/CheckOut/LoadPackage", null).done(function (content) {
        $(".checkout-package").html(content);
        LoadShippingAndOrderSummary();
    });
}

function LoadShippingAndOrderSummary() {
    FetchData("/CheckOut/ShippingAndOrderSummary", null).done(function (content) {
        $(".checkout-shipping-order-summary").html(content);
        $("#checkout-total-amount").html(CurrencyFormat(parseFloat($("#hidTotalAmountCheckout").val())));
    });
}

function CurrencyFormat(amount) {                  //thanks to : https://stackoverflow.com/questions/149055/how-to-format-numbers-as-currency-string
    var formatter = Intl.NumberFormat("en-PH", {
        style: "currency",
        currency: "PHP"
    }).format(amount);

    return formatter;
}