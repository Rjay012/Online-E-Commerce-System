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
                "<a class='dropdown-item' role='button' href='' data-toggle='modal' data-target='#modalNewProductColor' onclick='AddColorImages(" + parseInt(productID) + ")'>Add</a>" +
                "<a class='dropdown-item' href='' data-toggle='modal' data-target='#modalEditProductColor' onclick='EditColorImages(" + parseInt(productID) + ")'>Edit</a>" +
                "</div>" +
                "<button class='btn btn-warning btn-sm' type='button' data-toggle='modal' data-target='#myModal' onclick=''>Add Sizes</button>";
        }
    }];

    var columnDefs = [{
        targets: [0, 1, 2],
        visible: false,
        searchable: false
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

$(document).on("change", ".file-img-input", function () {
    var img = $(this).siblings("img");
    ReadUrl(this, img);
});

$(document).on("click", "#BtnSaveNewProductColor", function () {
    var counter = 1;
    $(".file-img-input").each(function () {
        var file = $(this).get(0).files;
        var data = new FormData();
        data.append(file[0].name, file[0]);
        data.append("ProductID", parseInt($("#txtHidProductID").val()));
        data.append("ColorID", parseInt($("#sColor").val()));
        data.append("IsDisplay", $("[id*=isDisplayImg" + counter + "]:checked").val() == "on" ? true : false);
        data.append("IsMainDisplay", $("[id*=isMainDisplayImg" + counter + "]:checked").val() == "on" ? true : false);

        $.ajax({
            type: "post",
            url: "/Product/CreateNewProductColor",
            data: data,
            contentType: false,
            processData: false,
            success: function (result) {
                //alert(JSON.stringify(result));
            }
        });

        counter++;
    });
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

function ReadUrl(input, img) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(img).attr("src", e.target.result);
        }
        reader.readAsDataURL(input.files[0]);
    }
}

function AddColorImages(productID) {
    FetchData("/Product/NewColorModalForm", null).done(function (content) {
        $("#NewProductColorForm").html(content);
        $("#txtHidProductID").val(productID);
    });
}

function EditColorImages(productID) {
    FetchData("/Product/EditColorModalForm", null).done(function (content) {
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