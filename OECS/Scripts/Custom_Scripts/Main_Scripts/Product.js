$(document).ready(function () {
    var columns = [{ 'data': 'ProductID' }, { 'data': 'ColorID' }, {'data': 'isMainDisplay'}, { 'data': 'productName', 'width': '30%' }, { 'data': 'category1', 'width': '20%' },
        {
            'data': 'ProductID', render: function (productID, type, row) {
                return "<button class='btn btn-success btn-sm' type='button' data-toggle='modal' data-target='#modalProductImageGallery' onclick='ViewProductPhotoGallery(" + parseInt(productID) + ", " + parseInt(row.ColorID) + ")'>View Gallery</button>";
            }
        },
        { 'data': 'date' }, { 'data': 'price' },
        {
            'data': 'ProductID', render: function (productID, type, row) {
                return "<button class='btn btn-info btn-sm' type='button' data-toggle='modal' data-target='#modalNewProductColor' onclick=''>Add Colors</button>" +
                    "<button class='btn btn-warning btn-sm' type='button' data-toggle='modal' data-target='#myModal' onclick=''>Add Sizes</button>";
            }
        }
    ];
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

function ViewProductPhotoGallery(productID, colorID) {
    FetchData("/Product/ViewPhotoGallery", { productID: productID, colorID: colorID }).done(function (content) {
        $("#ProductImageGalleryForm").html(content);
    });
}

function success(data) {
    alert(JSON.stringify(data));
}