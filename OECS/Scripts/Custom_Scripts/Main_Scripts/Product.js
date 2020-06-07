$(document).ready(function () {
    LoadTable();
});

$(document).on("click", "#BtnTriggerModalForm", function () {
    FetchData("/Product/NewProductModalForm", null).done(function (content) {
        $("#NewProductForm").html(content);
        $('.datepicker').pickadate({
            format: 'yyyy-mm-dd'
        });
    });
});

$(document).on("click", "#BtnSaveNew", function () {
    if (confirm("Sure you want to add this product?") == true) {
        $("#BtnConfirmSaveNew").click();
    }
});

$(document).on("change", "#IconFile", function () { //changed from "file-add-icon" to "IconFile"
    var img = $(this).siblings("img");
    ReadUrl(this, img);
});

$(document).on("change", "#Files", function () {  //changed from "Files" to "file-6"
    var files = $(this)[0].files;
    var c = 1;

    if (files.length == 5) {
        for (var i = 0, f; f = files[i]; i++) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#img' + c).attr('src', e.target.result);
                $("#isDisplayImg-" + c).attr("disabled", false);
                c++;
            };
            reader.readAsDataURL(f);
        }
    }
    else {
        toastr.error("Please select 5 images to continue!", "All Image are Required", { "positionClass": "md-toast-top-right" });
        $(this).val("");
    }
});

$(document).on("change", ".add-display", function () {
    var position = $(this).attr("id").split("-");
    $("#IsDisplayPosition").val(position[1]);
});

$(document).on("click", "#BtnSaveNewProductColor", function () {
    if (confirm("Sure you want to add this new color?") == true) {
        if ((parseInt($("#ColorID").val()) > 0 && $("#IconFile").get(0).files.length !== 0 && $("#Files").get(0).files.length !== 0) && $("#IsDisplayPosition").val() != 0) {
            $("#BtnConfirmSaveNewProductColor").click();
        }
        else {
            toastr.error("Color, Color Display, Images and Icons are required!", "All Fields are Required", { "positionClass": "md-toast-top-right" });
        }
    }
});

$(document).on("click", "#BtnSaveEditProductColor", function () {
    //if (confirm("Sure you want to add this new color?") == true) {
    //    if (parseInt($("#ColorID").val()) > 0 && $("#IconFile").get(0).files.length !== 0 && $("#Files").get(0).files.length !== 0) {
    //var c = 1;
    //$(".file-edit-img-input").each(function () {  //manage images first via ajax
    //    var file = $(this).get(0).files;
    //    var data = new FormData();
    //    if (file.length !== 0) {  //select image file to be updated
    //        data.append("ImageID", parseInt($("#txtHidImgID-" + c).val()));
    //        data.append(file[0].name, file[0]);
    //        $.ajax({
    //            type: "post",
    //            url: "/Product/EditProductImage",
    //            data: data,
    //            contentType: false,
    //            processData: false
    //        });
    //    }
    //    c++;
    //});
    $("#BtnConfirmSaveEditProductColor").click();
    //    }
    //    else {
    //        toastr.error("Color, Color Display, Images and Icons are required!", "All Fields are Required", { "positionClass": "md-toast-top-right" });
    //    }        
    //}
});

$(document).on("click", "#BtnSetUsDisplay", function () {
    var defaultID = $(".default-img").attr("productImageID");
    var selectedID = $(".selected").attr("productImageID");

    if (confirm("Sure you want to set this image us default color display?") == true) {
        FetchData("/Product/SetImageDisplay", { defaultDisplayID: defaultID, selectedDisplayID: selectedID }).done(function (result) {
            toastr.success("Image Setup us display", "Success", { "positionClass": "md-toast-top-right" });
        });
    }
});

$(document).on("click", "#BtnSetUsMainDisplay", function () {
    var selectedID = $(".selected").attr("productImageID");

    if (confirm("Sure you want to set this image us default display?") == true) {
        FetchData("/Product/SetImageMainDisplay", { productID: parseInt($("#txtHidProductID").val()), selectedMainDisplayID: selectedID }).done(function (result) {
            toastr.success("Image Setup us main display", "Success", { "positionClass": "md-toast-top-right" });
        });
    }
});

$(document).on("click", "#BtnSetUsBothDisplay", function () {
    alert("NOT YET WORKING!");
});

$(document).on("change", ".file-edit-img-input", function () {
    var img = $(this).siblings("img");
    ReadUrl(this, img);
});

$(document).on("change", ".add-size", function () {
    var sizeID = $(this).attr("sizeID");
    $("#txtHidSizeID-" + sizeID).val($(this).is(":checked") ? sizeID : 0);
});

/** START REVISE THIS CODE LATER **/
$(document).on("click", ".color-wrapper", function () {
    var colorID = $(this).children(".add-color").attr("colorID");
    $("#ColorID").val(colorID);
    //mark selected
    $(this).siblings(".color-wrapper").children(".add-color").css({ "width": "30px", "height": "30px", "border-radius": "50%" });
    $(this).children(".add-color").css({ "width": "40px", "height": "40px", "border-radius": "50%" });
});

$(document).on("click", ".color-edit-wrapper", function () {
    var colorID = $(this).children(".edit-color").attr("colorID");
    $("#ColorID").val(colorID);
    $(this).siblings(".color-edit-wrapper").children(".edit-color").css({ "width": "30px", "height": "30px", "border-radius": "50%" });
    $(this).children(".edit-color").css({ "width": "40px", "height": "40px", "border-radius": "50%" });
});
/** END REVISE THIS CODE LATER **/

$(document).on("click", ".size-popover", function () {
    var item = parseInt($(this).attr("sizeID"));
    $(this).popover({   //activate color popover
        html: true,
        title: '<h6 class="custom-title">Size Quantity</h6>',
        content: $("#size-popover-" + item).html(),
        placement: "top",
        sanitize: false
    });
});

$(document).on("change", ".new-size", function () {
    var sizeID = $(this).attr("sizeID");
    if ($(this).is(":checked")) {
        $(this).popover({   //activate color popover
            html: true,
            title: '<h6 class="custom-title">New Size Quantity</h6>',
            content: $("#new-size-quantity-popover-" + sizeID).html(),
            placement: "top",
            sanitize: false
        });
    }
    else {
        $("#new-size-quantity-holder-" + sizeID).val("");
    }
});

$(document).on("keyup", ".new-size-quantity", function () {
    var id = $(this).attr("id").split("-");
    var quantity = $(this).val();

    $("#new-size-quantity-holder-" + id[3]).val(id[3] + "-" + quantity);
});

function LoadTable() {
    var columns = [{
        'data': 'ProductID', render: function (productID, type, row) {
            return "<div class='custom-control custom-checkbox'>" +
                "<input class='custom-control-input' type='checkbox' id='display-" + parseInt(productID) + "' " + (row.display == true ? 'checked': '') + " />" +
                "<label class='custom-control-label' for='display-" + parseInt(productID) + "' />" +
                "</div>";
        }
    },
    { 'data': 'ProductID' }, { 'data': 'ColorID' }, { 'data': 'IconID'}, { 'data': 'productName', 'width': '30%' }, { 'data': 'category1', 'width': '20%' },
    {
        'data': 'ProductID', render: function (productID, type, row) {
            return "<button class='btn btn-success btn-sm' type='button' data-toggle='modal' data-target='#modalProductImageGallery' onclick='ViewProductPhotoGallery(" + parseInt(productID) + ", " + parseInt(row.ColorID) + ", " + parseInt(row.IconID) + ")'>VIEW GALLERY</button>";
        }
    },
    { 'data': 'date', "render": function (value) { return moment(value).format('YYYY-MM-DD'); } }, { 'data': 'price' },
    {
        'data': 'ProductID', render: function (productID, type, row) {
            return "<div class='dropdown'><button class='btn btn-info btn-sm dropdown-toggle mr-4' id='dropdown-1' type='button' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'>COLOR</button>" +
                "<div class='dropdown-menu'>" +
                "<a class='dropdown-item' role='button' data-toggle='modal' data-target='#modalNewProductColor' onclick='AddColorImages(" + parseInt(productID) + ")'><i class='fas fa-plus'></i> Add</a>" +
                "<a class='dropdown-item' role='button' data-toggle='modal' data-target='#modalEditProductColor' onclick='EditColorImages(" + parseInt(productID) + ", " + parseInt(row.ColorID) + ", " + parseInt(row.IconID) + ")'><i class='fas fa-edit'></i> Edit</a>" +
                "</div></dropdown>";

        }
    },
    {
        'data': 'ProductID', render: function (productID, type, row) {
            return "<button class='btn btn-primary btn-sm' type='button' data-toggle='modal' data-target='#modalProductEdit' onclick=''>EDIT</button>";
        }
    }];

    var columnDefs = [{
        targets: [1, 2, 3],
        visible: false,
        searchable: false,
    }];
    LoadTableViaServerSide("ShowProductList", "/Product/ShowProduct", columns, columnDefs);
}

//preview images 
function ReadUrl(input, img) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(img).attr("src", e.target.result);
        }
        reader.readAsDataURL(input.files[0]);
    }
}

function AddSize(productID) {
    FetchData("/Product/AddSizeModalForm", null).done(function (content) {
        $("#NewProductSizeForm").html(content);
    });
}

function AddColorImages(productID) {
    $("#EditProductColorForm").html("");  //clear other form to avoid conflicts on properties
    FetchData("/Product/NewColorModalForm", null).done(function (content) {
        $("#NewProductColorForm").html(content);
        $("#ProductID").val(productID);
    });
}

function EditColorImages(productID, colorID, iconID) {
    $("#NewProductColorForm").html("");  //clear other form to avoid conflicts on properties
    FetchData("/Product/EditColorModalForm", { productID: productID, colorID: colorID, iconID: iconID }).done(function (content) {
        $("#EditProductColorForm").html(content);
        //MarkSelectedOption(colorID);

        //increase the height and width of selected icon
        $("#edit-icon-" + iconID).css({ "height": "45px", "width": "45px" });

        //mark default color when updating
        $(".color-edit-wrapper .edit-color").each(function () {
            var colorID = $(this).attr("colorID");
            if (colorID == $("#ColorID").val()) {
                $(this).css({ "height": "40px", "width": "40px" });
            }
        });

        $(".txtHidImgID").each(function () {
            $("#hid-strongly-typed-img-" + $(this).val()).val($(this).val());
        });
    });
}

function ViewProductPhotoGallery(productID, colorID, iconID) {
    FetchData("/Product/ViewPhotoGallery", { productID: productID, colorID: colorID, iconID: iconID }).done(function (content) {
        $("#ProductImageGalleryForm").html(content);
        $("#icon-" + iconID).css({ "height": "45px", "width": "45px" });
    });
}

//function MarkSelectedOption(val) {
//    $("#sEditColor option").each(function () {
//        if ($(this).val() == val) {
//            $(this).prop("selected", true);
//        }
//    });
//}

function Success(response) {
    if (response.data == "success") {
        toastr.success("Request Successful", "Success", { "positionClass": "md-toast-top-right" });
    }
}