$(document).ready(function () {
    LoadCartItems();
});

$(document).on("change", "#defaultUnchecked", function () {
    if ($(this).is(":checked")) {
        var amount = 0.0;
        $(".chk-cart-item").prop("checked", true);
        CalculateAmount();
    }
    else {
        $(".chk-cart-item").prop("checked", false);
        $("#selected-item-amount").text("₱0.00");
        $("#hidSelectedItemAmount").val(0.00);
    }
});

$(document).on("click", "#BtnDeleteSelectedItem", function () {
    if ($(".chk-cart-item").is(":checked")) {
        var itemArr = [];
        $(".chk-cart-item:checked").each(function (indx) {
            itemArr[indx] = parseInt($(this).attr("OrderNo"));
        });
        DeleteSelectedItem(...itemArr);
    }
    else {
        toastr.error("Please Select Item to Delete!", "Select an Item", { "positionClass": "md-toast-top-right" });
    }
});


$(document).on("change", ".chk-cart-item", function () {
    var amount = 0.0;
    if ($(this).is(":checked")) {
        $(".chk-cart-item:checked").each(function () {
            amount += parseFloat(CalculateAmountSelectedItem($(this)));
            $("#hidSelectedItemAmount").val(amount);
        });
    }
    else {
        var calculatedAmount = parseFloat($("#hidSelectedItemAmount").val());
        amount = calculatedAmount - parseFloat($(this).attr("ItemPrice"));
        $("#hidSelectedItemAmount").val(amount);
    }
    $("#selected-item-amount").text(CurrencyFormat(amount));
});

function DeleteSelectedItem(...itemArr) {
    DeleteItem(itemArr);
}

function DeleteItem(orderNo) {
    if (confirm("Sure you want to delete selected item(s)?") == true) {
        FetchData("/Cart/Delete", { orderNo: orderNo }).done(function (response) {
            LoadCartItems();
            LoadOrderSummary();
            Success(response);
        });
    }
}

function CalculateAmount() {
    var amount = 0.0;
    $(".chk-cart-item:checked").each(function () {
        amount += parseFloat(CalculateAmountSelectedItem($(this)));
        $("#selected-item-amount").text(CurrencyFormat(parseFloat(amount)));
        $("#hidSelectedItemAmount").val(parseFloat(amount));
    });
}

function CalculateAmountSelectedItem(element) {
    var quantity = parseInt($(element).parent("div").parent("td").siblings("td").children("div").children(".quantity").val());
    return parseFloat($(element).attr("ItemPrice")) * quantity;
}

function ChangeQuantity(totalPrice, element) {
    var quantity = parseInt(element.siblings(".quantity").val());
    element.parent("div").parent("td").siblings(".td-price").text(CurrencyFormat(totalPrice * quantity));

    CalculateAmount();
}

function CurrencyFormat(amount) {                  //thanks to : https://stackoverflow.com/questions/149055/how-to-format-numbers-as-currency-string
    var formatter = Intl.NumberFormat("en-PH", {
        style: "currency",
        currency: "PHP"
    }).format(amount);

    return formatter;
}

function LoadCartItems() {
    FetchData("/Cart/LoadCartTable", null).done(function (content) {
        $(".cart-item").html(content);
        LoadOrderSummary();
    });
}

function LoadOrderSummary() {
    FetchData("/Cart/LoadOrderSummary", null).done(function (content) {
        $(".order-summary").html(content);
        $("#cart-total-amount").html(CurrencyFormat(parseInt($("#hidTotalAmount").val())));
    });
}