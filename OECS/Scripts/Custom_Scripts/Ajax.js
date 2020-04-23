var FetchData = function (url, data) {
    return $.ajax({
        url: url,
        data: data,
        type: "post"
    });
};