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

$(document).on("change", ".custom-control-input", function () {
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
    var c = 1;
    $(".file-edit-img-input").each(function () {  //manage images first via ajax
        var file = $(this).get(0).files;
        var data = new FormData();
        if (file.length !== 0) {  //select image file to be updated
            data.append("ImageID", parseInt($("#txtHidImgID-" + c).val()));
            data.append(file[0].name, file[0]);
            $.ajax({
                type: "post",
                url: "/Product/EditProductImage",
                data: data,
                contentType: false,
                processData: false
            });
        }
        c++;
    });
    $("#BtnConfirmSaveEditProductColor").click();
    //    }
    //    else {
    //        toastr.error("Color, Color Display, Images and Icons are required!", "All Fields are Required", { "positionClass": "md-toast-top-right" });
    //    }        
    //}
});

$(document).on("click", "#BtnSetUsDisplay", function () {
    var defaultID = $(".default-img").attr("productColorID");
    var selectedID = $(".selected").attr("productColorID");

    if (confirm("Sure you want to set this image us default color display?") == true) {
        FetchData("/Product/SetDisplay", { defaultID: defaultID, selectedID: selectedID }).done(function (result) {
            alert("Image Setup");
        });
    }
});

$(document).on("click", "#BtnSetUsMainDisplay", function () {
    var selectedID = $(".selected").attr("productColorID");

    if (confirm("Sure you want to set this image us default display?") == true) {
        FetchData("/Product/SetMainDisplay", { productID: parseInt($("#txtHidProductID").val()), selectedID: selectedID }).done(function (result) {
            alert("Image Setup");
        });
    }
});

$(document).on("click", "#BtnSetUsBothDisplay", function () {
    $("#BtnSetUsDisplay, #BtnSetUsMainDisplay").trigger("click");
});

$(document).on("change", ".file-edit-img-input", function () {
    var img = $(this).siblings("img");
    ReadUrl(this, img);
});

function LoadTable() {
    var columns = [{ 'data': 'ProductID' }, { 'data': 'ColorID' }, { 'data': 'isMainDisplay' }, { 'data': 'productName', 'width': '30%' }, { 'data': 'category1', 'width': '20%' },
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
            return "<div class='dropdown'><button class='btn btn-warning btn-sm dropdown-toggle mr-4' id='dropdown-2' type='button' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'>SIZE</button>" +
                "<div class='dropdown-menu'>" +
                "<a class='dropdown-item' role='button' data-toggle='modal' data-target='#modalNewProductSize' onclick='AddSize(" + parseInt(productID) + ")'><i class='fas fa-plus'></i> Add</a>" +
                "<a class='dropdown-item' role='button' data-toggle='modal' data-target='#' onclick=''><i class='fas fa-edit'></i> Edit</a>" +
                "</div></div>";

        }
    },
    {
        'data': 'ProductID', render: function (productID, type, row) {
            return "<button class='btn btn-primary btn-sm' type='button' data-toggle='modal' data-target='#modalProductEdit' onclick=''>EDIT</button>";
        }
    }];

    var columnDefs = [{
        targets: [0, 1, 2],
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
    FetchData("/Product/NewColorModalForm", null).done(function (content) {
        $("#NewProductColorForm").html(content);
        $("#ProductID").val(productID);
    });
}

function EditColorImages(productID, colorID, iconID) {
    FetchData("/Product/EditColorModalForm", { productID: productID, colorID: colorID, iconID: iconID }).done(function (content) {
        $("#EditProductColorForm").html(content);
        MarkSelectedOption(colorID);

        //increase the height and width of selected icon
        $("#edit-icon-" + iconID).css({ "height": "45px", "width": "45px" });
    });
}

function ViewProductPhotoGallery(productID, colorID, iconID) {
    FetchData("/Product/ViewPhotoGallery", { productID: productID, colorID: colorID, iconID: iconID }).done(function (content) {
        $("#ProductImageGalleryForm").html(content);
        $("#icon-" + iconID).css({ "height": "45px", "width": "45px" });
    });
}

function MarkSelectedOption(val) {
    $("#sEditColor option").each(function () {
        if ($(this).val() == val) {
            $(this).prop("selected", true);
        }
    });
}

function Success(response) {
    if (response.data == "success") {
        toastr.success("Request Successful", "Success", { "positionClass": "md-toast-top-right" });
    }
}