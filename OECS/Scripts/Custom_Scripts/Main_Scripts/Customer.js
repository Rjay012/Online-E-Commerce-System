$(document).ready(function () {
    GetCustomerAddress();
});

function GetCustomerAddress() {
    FetchData("/Customer/GetCustomerAddress", null).done(function (response) {
        $("#customer-address").html(response.address);
    });
}