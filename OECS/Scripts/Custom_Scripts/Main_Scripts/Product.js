$(document).ready(function () {
    ReadyProductTableList();
});

$(document).on("click", "#BtnTriggerAddNewProductModalForm", function () {
    FetchData("/Product/NewProductModalForm", null).done(function (content) {
        $("#NewProductForm").html(content);
        ManagePluginContent();
    });
});

$(document).on("click", ".mdb-searchable", function (event) {
    event.stopPropagation();
});

$(document).on("click", "#BtnSaveNew", function () {
    if (confirm("Sure you want to add this product?") == true) {
        ButtonLoader($(this));
        $("#BtnConfirmSaveNew").click();
        if ($("#chkNewlyAddedProduct").is(":checked")) {
            ReadyNewlyAddedProducts();
        }
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

$(document).on("click", "#BtnSaveNewProductDetail", function () {
    if (confirm("Sure you want to add this new color?") == true) {
        if ((parseInt($("#ColorID").val()) > 0 && $("#IconFile").get(0).files.length !== 0 && $("#Files").get(0).files.length !== 0) && $("#IsDisplayPosition").val() != 0) {
            ButtonLoader($(this));
            $("#BtnConfirmSaveNewProductDetail").click();
        }
        else {
            toastr.error("Color, Color Display, Images and Icons are required!", "All Fields are Required", { "positionClass": "md-toast-top-right" });
        }
    }
});

$(document).on("click", "#BtnSaveEditProductDetail", function () {
    if (confirm("Sure you want to edit this color?") == true) {
        ButtonLoader($(this));
        $("#BtnConfirmSaveEditProductDetail").click();
    }
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

    $(this).siblings("#FileToRemove").val($(this).siblings("img").attr("imgID"));
});

$(document).on("change", ".add-size", function () {
    var sizeID = $(this).attr("sizeID");
    $("#txtHidSizeID-" + sizeID).val($(this).is(":checked") ? sizeID : 0);
});

$(document).on("click", ".color-wrapper", function () {
    MarkSelectedIcon(".add-color", ".color-wrapper", "#ColorID");
});

$(document).on("click", ".color-edit-wrapper", function () {
    MarkSelectedIcon(".edit-color", ".color-edit-wrapper", "#NewColorID");
});

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
        $(this).popover({   //activate new size quantity popover
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

    if (isNaN(quantity)) {
        toastr.warning("Only Number is allowed to enter", "Warning", { "positionClass": "md-toast-top-right" });
    }
    else {
        $("#new-size-quantity-holder-" + id[3]).val(id[3] + "-" + quantity);
    }
});

$(document).on("keyup", ".edit-size-quantity", function () {
    var id = $(this).attr("id").split("-");
    var quantity = $(this).val();

    $("#new-size-quantity-holder-" + id[3]).val(id[3] + "-" + quantity);
});

$(document).on("change", ".existing-size", function () {
    var id = $(this).attr("id").split("-");  //split and get the sizeID to remove
    if (!$(this).is(":checked")) {
        $("#txthid-remove-size-" + id[2]).val(id[2]);
    }
    else {
        $("#txthid-remove-size-" + id[2]).val(0);
    }
});

$(document).on("change", "#chkNewlyAddedProduct", function () {
    if ($(this).is(":checked")) {
        ReadyNewlyAddedProducts();
    }
    else {
        ReadyProductTableList();
    }
});

function MarkSelectedIcon(classChildren, classSiblings, idColor) {
    var colorID = $(this).children(classChildren).attr("colorID");
    $(idColor).val(colorID);
    $(this).siblings(classSiblings).children(".edit-color").css({ "width": "30px", "height": "30px", "border-radius": "50%" });
    $(this).children(classChildren).css({ "width": "40px", "height": "40px", "border-radius": "50%" });
}

function ReadyProductTableList() {
    MainLoader("#ProductTableList");
    FetchData("/Product/ReadyProductListTable").done(function (table) {
        $("#ProductTableList").html(table);
        LoadTable();
    });
}

function ReadyNewlyAddedProducts() {
    MainLoader("#ProductTableList");
    FetchData("/Product/ShowNewlyAddedProductListTable").done(function (table) {
        $("#ProductTableList").html(table);
        LoadNewlyAddedProducts();
    });
}

function LoadNewlyAddedProducts() {
    var columns = [{ 'data': 'ProductID' }, { 'data': 'ProductName' }, { 'data': 'BrandName' }, { 'data': 'CategoryName' },
    {
        'data': 'ProductID', render: function (productID, type, row) {
            return "<button class='btn btn-success btn-sm btn-block' data-toggle='modal' data-target='#modalNewProductDetail' onclick='AddNewProductDetail(" + parseInt(productID) + ")'>ADD DETAILS</button>";
        }
    },
    { 'data': 'Date', "render": function (value) { return moment(value).format('YYYY-MM-DD'); } }, { 'data': 'Price' },
    {
        'data': 'ProductID', render: function (productID, type, row) {
            return "<button class='btn btn-primary btn-sm' type='button' data-toggle='modal' data-target='#modalEditProduct' onclick='TriggerEditProductModalForm(" + parseInt(productID) + ")'>EDIT</button>";
        }
    }];
    var columnDefs = [{
        targets: [0],
        visible: false,
        searchable: false,
        orderable: false
    }];
    LoadTableViaServerSide("ShowNewlyAddedProductList", "/Product/ShowProduct", columns, columnDefs, { name: "isNewlyAdded", value: true });
}

function LoadTable() {
    var columns = [{
        'data': 'ProductID', render: function (productID, type, row) {
            return "<div class='custom-control custom-checkbox'>" +
                "<input class='custom-control-input' type='checkbox' id='display-" + parseInt(productID) + "' " + (row.display == true ? 'checked' : '') + " />" +
                "<label class='custom-control-label' for='display-" + parseInt(productID) + "' />" +
                "</div>";
        }
    },
        { 'data': 'ProductID' }, { 'data': 'ColorID' }, { 'data': 'IconID' }, { 'data': 'ProductName', 'width': '30%' }, { 'data': 'BrandName'}, { 'data': 'CategoryName', 'width': '20%' },
    {
        'data': 'ProductID', render: function (productID, type, row) {
            return "<button class='btn btn-success btn-sm' type='button' data-toggle='modal' data-target='#modalProductImageGallery' onclick='ViewProductPhotoGallery(" + parseInt(productID) + ", " + parseInt(row.ColorID) + ", " + parseInt(row.IconID) + ")'>VIEW</button>";
        }
    },
    { 'data': 'Date', "render": function (value) { return moment(value).format('YYYY-MM-DD'); } }, { 'data': 'Price' },
    {
        'data': 'ProductID', render: function (productID, type, row) {
            return "<div class='dropdown'><button class='btn btn-info btn-sm dropdown-toggle mr-4' id='dropdown-1' type='button' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'>DETAILS</button>" +
                "<div class='dropdown-menu'>" +
                "<a class='dropdown-item' role='button' data-toggle='modal' data-target='#modalNewProductDetail' onclick='AddNewProductDetail(" + parseInt(productID) + ")'><i class='fas fa-plus'></i> Add</a>" +
                "<a class='dropdown-item' role='button' data-toggle='modal' data-target='#modalEditProductDetail' onclick='EditProductDetail(" + parseInt(productID) + ", " + parseInt(row.ColorID) + ", " + parseInt(row.IconID) + ")'><i class='fas fa-edit'></i> Edit</a>" +
                "</div></dropdown>";
        }
    },
    {
        'data': 'ProductID', render: function (productID, type, row) {
            return "<button class='btn btn-primary btn-sm' type='button' data-toggle='modal' data-target='#modalEditProduct' onclick='TriggerEditProductModalForm(" + parseInt(productID) + ")'>EDIT</button>";
        }
    }];

    var columnDefs = [{
        targets: [1, 2, 3],
        visible: false,
        searchable: false,
        orderable: false
    }];
    LoadTableViaServerSide("ShowProductList", "/Product/ShowProduct", columns, columnDefs, { name: "isNewlyAdded", value: false });
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

function AddNewProductDetail(productID) {
    $("#EditProductDetailForm").html("");  //clear other form to avoid conflicts on properties
    FetchData("/Product/NewProductDetailModalForm", null).done(function (content) {
        $("#NewProductDetailForm").html(content);
        $("#ProductID").val(productID);
    });
}

function TriggerEditProductModalForm(productID) {
    FetchData("/Product/EditProductModalForm", { productID: productID }).done(function (content) {
        $("#EditProductForm").html(content);
        ManagePluginContent();
    });
}

function EditProductDetail(productID, colorID, iconID) {
    $("#NewProductDetailForm").html("");  //clear other form to avoid conflicts on properties
    FetchData("/Product/EditProductDetailModalForm", { productID: productID, colorID: colorID, iconID: iconID }).done(function (content) {
        $("#EditProductDetailForm").html(content);

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

function ManagePluginContent() {
    $('.datepicker').pickadate({
        format: 'yyyy-mm-dd'
    });
    $('.mdb-select').materialSelect();
}

function ViewProductPhotoGallery(productID, colorID, iconID) {
    FetchData("/Product/ViewPhotoGallery", { productID: productID, colorID: colorID, iconID: iconID }).done(function (content) {
        $("#ProductImageGalleryForm").html(content);
        $("#icon-" + iconID).css({ "height": "45px", "width": "45px" });
    });
}

function Logout() {
    if (confirm("Sure you want to logout?") == true) {
        $("#BtnLogout").click();
    }
}