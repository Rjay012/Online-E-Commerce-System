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

$(document).on("click", ".myPopover", function () {
    var item = parseInt($(this).attr("item"));
    $("#popover-" + item).children(".col-sm-12").children(".colors").each(function () {
        var color = $(this).attr("color");
        $(this).css("color", "green");
    });
    $(this).popover({   //activate color popover
        html: true,
        title: '<h6 class="custom-title">Available Colour(s)</h6>',
        content: $("#popover-" + item).html(),
        //template: "<div class='popover'>" +
        //    "<div class='arrow'></div>" +
        //    "<h3 class='popover-header'>dsfsd</h3>" +
        //    "<div class='popover-body'></div>" +
        //    "<div class='popover-footer'></div>" +
        //    "</div>",
        placement: "auto"
    });
    
    
});