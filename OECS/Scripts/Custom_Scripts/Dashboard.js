$(document).ready(function () {
    FetchData("/Dashboard/SideMenu", null).done(function (header) {
        //$("header").html(header);
        // SideNav Initialization
        $(".button-collapse").sideNav();

        var container = document.querySelector('.custom-scrollbar');
        Ps.initialize(container, {
            wheelSpeed: 2,
            wheelPropagation: true,
            minScrollbarLength: 20
        });

        // Tooltips Initialization
        $(function () {
            $('[data-toggle="tooltip"]').tooltip();
        });
    });
});