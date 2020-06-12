function LoadTableViaServerSide(id, url, projectColumn, columnDef, additionalData) {
    $("#" + id).DataTable().clear().destroy();
    $('#' + id).DataTable({
        lengthMenu: [[5, 10, 15], [5, 10, 15]],
        serverSide: true,
        bPaginate: true,
        bServerSide: true,
        pagingType: "simple_numbers",
        processing: true,
        sAjaxSource: url,
        fnServerParams: function (aoData) {
            if (additionalData != null) {
                aoData.push(additionalData);  //manual adding of parameter/s to datatables to be sent to server
            }
        },
        fnServerData: function (sSource, aoData, fnCallBack) {
            $.ajax({
                type: "post",
                dataType: "json",
                data: aoData,
                url: sSource,
                success: fnCallBack
            });
        },
        columns: projectColumn,
        columnDefs: columnDef
    });
    $("select").addClass("browser-default");
}

function LoadNormalTable() {
    $.ajax({
        type: "post",
        dataType: "json",
        url: url,
        success: function (result) {
            $('#' + id).DataTable({
                lengthMenu: [[5, 10, 15], [5, 10, 15]],
                pagingType: "simple_numbers",
                processing: true,
                data: result,
                columns: projectColumn
            });
            $("select").addClass("browser-default");
        }
    });
}