function MainLoader(container) {
    $(container).html(
        "<div class='text-center'>" +
        "<div class='spinner-border' role='status'><span class='sr-only'></span></div>" +
        "</div>"
    );
}

function ButtonLoader(attr) {
    $(attr).addClass("working-button");
    $(attr).html('<span class="spinner-border spinner-border-sm mr-2" role="status" aria-hidden="true"></span>Loading...').addClass('disabled');
}