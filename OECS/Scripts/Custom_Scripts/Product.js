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
    
    $(this).popover({   //activate color popover
        html: true,
        title: '<h6 class="custom-title">Available Colour(s)</h6>',
        content: $("#popover-" + item).html(),
        placement: "auto"
    });
    
    $("#popover-" + item).children(".col-sm-12").children().each(function () {
        var color = $(this).attr("color");
        var myClass = $(this).attr("class");
        $("." + myClass).css("background-color", color);
    });
});