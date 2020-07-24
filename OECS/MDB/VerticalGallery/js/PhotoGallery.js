//Photo Gallery
$(document).ready(function () {
    $(document).on("click", 'a', function () {
        var largeImage = $(this).attr('data-full');
        $('.selected').removeClass("selected");
        $(this).addClass('selected');
        $('.full img').hide();
        $('.full img').attr('src', "..\\..\\" + largeImage);  //added backslashes to find image path/url
        $('.full img').fadeIn();
    }); // closing the listening on a click

    $(document).on('click', '.full img', function () {
        var modalImage = $(this).attr('src');
        $.fancybox.open(modalImage);
    });
}); //closing our doc ready