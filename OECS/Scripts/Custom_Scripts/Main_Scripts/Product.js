$(document).ready(function () {
    var columns = [{ 'data': 'ProductID' }, { 'data': 'ColorID' }, { 'data': 'isMainDisplay' }, { 'data': 'productName', 'width': '30%' }, { 'data': 'category1', 'width': '20%' },
    {
        'data': 'ProductID', render: function (productID, type, row) {
            return "<button class='btn btn-success btn-sm' type='button' data-toggle='modal' data-target='#modalProductImageGallery' onclick='ViewProductPhotoGallery(" + parseInt(productID) + ", " + parseInt(row.ColorID) + ", " + parseInt(row.IconID) + ")'>View Gallery</button>";
        }
    },
    { 'data': 'date', "render": function (value) { return moment(value).format('YYYY-MM-DD'); } }, { 'data': 'price' },
    {
        'data': 'ProductID', render: function (productID, type, row) {
            return "<button class='btn btn-info btn-sm dropdown-toggle mr-4' type='button' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'>Color</button>" +
                "<div class='dropdown-menu'>" +
                "<a class='dropdown-item' role='button' href='' data-toggle='modal' data-target='#modalNewProductColor' onclick='AddColorImages(" + parseInt(productID) + ")'><i class='fas fa-plus'></i> Add</a>" +
                "<a class='dropdown-item' href='' data-toggle='modal' data-target='#modalEditProductColor' onclick='EditColorImages(" + parseInt(productID) + ", " + parseInt(row.ColorID) + ", " + parseInt(row.IconID) + ")'><i class='fas fa-edit'></i> Edit</a>" +
                "</div>" +
                "<button class='btn btn-warning btn-sm' type='button' data-toggle='modal' data-target='#myModal' onclick=''>Add Sizes</button>";
        }
    }];

    var columnDefs = [{
        targets: [0, 1, 2],
        visible: false,
        searchable: false,
    }];
    LoadTableViaServerSide("ShowProductList", "/Product/ShowProduct", columns, columnDefs);
});

$(document).on("click", "#BtnTriggerModalForm", function () {
    FetchData("/Product/NewProductModalForm", null).done(function (content) {
        $("#NewProductForm").html(content);
    });
});

$(document).on("click", "#BtnSaveNew", function () {
    if (confirm("Sure you want to add this product?") == true) {
        $("#BtnConfirmSaveNew").click();
    }
});

//add file
$(document).on("change", ".file-img-input", function () {
    var img = $(this).siblings("img");
    ReadUrl(this, img);

    //enable radio button
    var id = $(this).attr("id").split("-");
    $("#isDisplayImg-" + id[1] + ", #isMainDisplayImg-" + id[1]).removeAttr("disabled");
});

$(document).on("change", "#file-add-icon", function () {
    var img = $(this).siblings("img");
    ReadUrl(this, img);
});

//update file
$(document).on("change", ".file-edit-img-input", function () {
    var img = $(this).siblings("img");
    ReadUrl(this, img);

    //enable radio button (Set up display and main display)
    var id = $(this).attr("id").split("-");
    $("#isDisplayEditImg-" + id[1] + ", #isMainDisplayEditImg-" + id[1]).removeAttr("disabled");
})

$(document).on("click", "#BtnSaveNewProductColor", function () {
    if ((parseInt($("#sColor").val()) > 0 && $("#file-add-icon").get(0).files.length !== 0) && $(".custom-control-input").is(":checked")) {
        if (AddIcon() === "success") {  //add icon first then proceed for adding images
            var counter = 1;
            $(".file-img-input").each(function () {
                var file = $(this).get(0).files;
                var data = new FormData();

                //set default image if input file doesn't have image
                if (file.length === 0) {
                    data.append("Path", "AddImageIcon\\add-image-icon.png");
                }
                else {
                    data.append(file[0].name, file[0]);
                }

                data.append("ProductID", parseInt($("#txtHidProductID").val()));
                data.append("ColorID", parseInt($("#sColor").val()));
                data.append("IsDisplay", $("[id*=isDisplayImg-" + counter + "]:checked").val() == "on" ? true : false);
                data.append("IsMainDisplay", $("[id*=isMainDisplayImg-" + counter + "]:checked").val() == "on" ? true : false);

                UploadFileToServer("CreateNewProductColor", data);
                counter++;
            });
        }
        else {
            alert("failed");
        }
    }
    else {
        alert("failed");
    }
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

function UploadFileToServer(actionName, data) {
    $.ajax({
        type: "post",
        url: "/Product/" + actionName,
        data: data,
        contentType: false,
        processData: false
    });
}

function AddIcon() {
    var file = $("#file-add-icon").get(0).files;
    var data = new FormData();

    if (file.length > 0) {  //check if icon exist
        data.append(file[0].name, file[0]);

        UploadFileToServer("CreateNewIcon", data);
        return "success";
    }

    return "failed";  //return failed if icon cant uploaded
}

function AddColorImages(productID) {
    FetchData("/Product/NewColorModalForm", null).done(function (content) {
        $("#NewProductColorForm").html(content);
        $("#txtHidProductID").val(productID);
    });
}

function EditColorImages(productID, colorID, iconID) {
    FetchData("/Product/EditColorModalForm", { productID: productID, colorID: colorID, iconID: iconID }).done(function (content) {
        $("#EditProductColorForm").html(content);
    });
}

function ViewProductPhotoGallery(productID, colorID, iconID) {
    FetchData("/Product/ViewPhotoGallery", { productID: productID, colorID: colorID, iconID: iconID }).done(function (content) {
        $("#ProductImageGalleryForm").html(content);
    });
}

function success(data) {
    alert(JSON.stringify(data));
}