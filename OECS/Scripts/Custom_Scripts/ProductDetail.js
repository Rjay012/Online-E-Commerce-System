$(document).ready(function () {
    PreviewImages(routeID, null, null, null);
    PreviewProductDetails(routeID);
    ViewTab();
});

$(document).on("change", "#SizeID", function () {
    SetQuantity();
});

$(document).on("keyup", "#Quantity", function () {
    var quantity = parseInt($(this).val());
    var appSize = GetAppropriateProductSize($("#SizeID"));

    if (quantity > appSize || quantity < 1) {  //input quantity should not be less than or greater than the product quantity
        $(this).val(1);
    }
});

$(document).on("click", "#BtnAddToCart", function () {
    var modal = $(this).attr("modal");
    if (parseInt($("#ProductID").val()) != 0 && parseInt($("#ColorID").val()) != 0 && parseInt($("#SizeID").val()) != 0 && (parseInt($("#Quantity").val()) != 0 && $("#Quantity").val() != "")) {
        $("#BtnConfirmAddToCart").click();
    }

    modal == "#modalLoginOrRegister" ? $(modal).modal("show") : "";
});

function ViewItemAdded() {  //after adding item(s) to cart, toggle modal to view the item (THIS FUNCTION CALLS FROM VIEW "_PreviewProductDetails.cshtml")
    $("#modalCart").modal("show");
    FetchData("/Cart/ViewAddedItem", { quantity: parseInt($("#Quantity").val()) }).done(function (content) {
        $("#ModalAddedItem").html(content);
    }); 
}

function SetQuantity() {
    $("#Quantity").val(1);  //default quantity
    $("#Quantity").attr("max", GetAppropriateProductSize($("#SizeID")));
}

function GetAppropriateProductSize(element) {
    var selectedOpt = element.children("option:selected").text();
    var optSize = selectedOpt.split("---");

    return optSize[1] != undefined ? optSize[1].trim().split(" ")[0] : 1;
}

function PreviewImages(productID, colorID, iconID, element) {
    if (element != null) {
        element.children("img").css({ "width": "40px", "height": "40px" });   //mark selected color icon
        element.siblings("a").children("img").css({ "width": "30px", "height": "30px" });  //revert to default size when user changed selected color icon
    }

    $("#ColorID").val(colorID);
    FetchData("/Product/PreviewProductImage", { productID: productID, colorID: colorID, iconID: iconID }).done(function (content) {
        $(".preview-image-section").html(content);
        colorID == null || iconID == null ? PreviewProductDetails(productID) : RefreshSizeList(productID, colorID, iconID);
    });
}

function RefreshSizeList(productID, colorID, iconID) {
    FetchData("/Product/FetchNewSizeList", { productID: productID, colorID: colorID, iconID: iconID }).done(function (content) {
        $("#SizeID").html("");
        $.each(content, function (indx) {
            $("#SizeID").append("<option value='" + parseInt(content[indx].Value) + "'>" + content[indx].Text + "</option>");
        });
        SetQuantity();
    });
}

function PreviewProductDetails(productID) {
    FetchData("/Product/PreviewProductDetails", { productID: productID }).done(function (content) {
        $(".preview-detail-section").html(content);
        SetQuantity();
    });
}

function ViewTab() {
    FetchData("/Product/ViewTab").done(function (content) {
        $(".tab-list").html(content);
    });
}