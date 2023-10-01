var tablecolumn = @ReplaceColumns;
// DataTable initialization options
$(document).ready(function () {
    $('#@ReplaceTableName').DataTable({
        "bServerSide": @bServerSide,
    "sScrollX": @sScrollX,
    "sScrollY": @sScrollY,
    "bScrollCollapse": false,
        "bAutoWidth": true,
            "bDestroy": true,
                "bInfo": @bInfo,
    "bStateSave": false,
        "sDom": "Blfrtip",
            "bDeferRender": true,
                "bSort": @bSort,
    "bPaginate": @bPaginate,
    "sPaginationType": "full_numbers",
        "iDisplayLength": @iDisplayLength,
    "aLengthMenu": [10, 50, 75, 100, 200, 500, 1000],
        "stateSave": false,
            "dom": "Blfrtip",
                "buttons": ["colvis"],
                    "deferRender": true,
                        "aoColumns": tablecolumn,
                            "rowId": "@pkColumnName",
                                "drawCallback": function (settings) {
                                    $('#@ReplaceTableName tbody tr').unbind('dblclick');
                                    $('#@ReplaceTableName tbody tr').bind('dblclick', function (data) {
                                        editEntity(this.id);
                                    });
                                },
    "ajax": function (data, callback, settings) {
        if (settings.aaSorting.length > 0) {
            data.order = [];
            data.order.push({
                "column": settings.aaSorting[0][0],
                "dir": settings.aaSorting[0][1]
            });
        }

        for (i = 0; i < data.columns.length; i++) {
            var datatype = tablecolumn[i].datatype;
            data.columns[i]["dataType"] = datatype
            data.columns[i]["bVisible"] = settings.aoColumns[i].bVisible;
            data.columns[i].search = data.search;
        }
        data.TableName = "@ReplaceTableName";
        data.pkid = "@pkColumnName";
        data.pkvalue = "-1";

        $.ajax({
            "url": "/api/@ReplaceControllerName/getfiltereddata",
            "type": "POST",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).done(function (data) {
            data.data = JSON.parse(data.data);
            setTimeout(function () {
                callback({
                    data: data?.data,
                    recordsTotal: data?.data[0]?.cnt,
                    recordsFiltered: data?.data[0]?.cnt,
                });
            }, 50);
        })
    },
});
});
// Open add/edit an entity form.
function addNewEntity() {
    $('#entityModalView').hide();
    $('#divAddNewEntity').hide();
    $('#addNewEntityModal').show();
}
// Function to close the new entity modal
function closeNewEntityModal() {
    clearNewEntityForm();
    $('#divAddNewEntity').show();
    $('#entityModalView').show();
    $('#addNewEntityModal').hide();
}
// Function to save/update an entity
function saveNewEntity() {
    if (validateEntity()) {
        var Listdata = [];
        var data = {};
        var PKId = 0;
        for (i = 0; i < tablecolumn.length; i++) {
            if (tablecolumn[i].datatype == "actionbutton") {
                continue;
            }
            data[tablecolumn[i].data] = "";
            if (tablecolumn[i].primaryKey) {
                PKId = $('#DTE_' + tablecolumn[i].data).val();
                if (PKId == "") {
                    PKId = 0;
                }
            }
            if (tablecolumn[i].datatype == "text" || tablecolumn[i].datatype == "integer") {
                data[tablecolumn[i].data] = $('#DTE_' + tablecolumn[i].data).val();
            }
            if (tablecolumn[i].datatype == "date") {
                data[tablecolumn[i].data] = $('#DTE_' + tablecolumn[i].data).val();
            }
            if (tablecolumn[i].datatype == "checkbox" || tablecolumn[i].datatype == "bit") {
                data[tablecolumn[i].data] = $('#DTE_' + tablecolumn[i].data).attr('checked') == "checked";
            }
        }
        if (data.@pkColumnName== "")
        data.@pkColumnName=-1;
        Listdata.push(data);
        $.ajax({
            type: ((PKId == 0) ? "Post" : "Put"),
            url: "api/@ReplaceControllerName",
            data: JSON.stringify(Listdata),
            contentType: "application/json",
            dataType: 'json',
            // ------v-------use it as the callback function
            success: function (request, error) {
                $.alert({ title: 'Confirmation', content: 'Data saved successfully.' });
                $('#@ReplaceTableName').DataTable().ajax.reload();
                closeNewEntityModal();
            },
            error: function (request, error) {
                $.alert({ title: 'Alert!', content: 'An error occurred attempting to save record.' });
            }
        });
    }
}
// Function to validate an entity
function validateEntity() {
    result = true;
    for (i = 0; i < tablecolumn.length; i++) {
        var ctrl = $('#DTE_' + tablecolumn[i].data);
        if (tablecolumn[i].datatype == SqlToDtDataTypeMaping.hidden || tablecolumn[i].primaryKey) {
            continue;
        }
        else if (!tablecolumn[i].is_nullable && ctrl.val() == "") {
            ctrl.addClass("error");
            result = false;
        }
        else if (tablecolumn[i].datatype == SqlToDtDataTypeMaping.Email && !IsValidEmail(ctrl.val())) {
            ctrl.addClass("error");
            ctrl.attr("title", "Please enter a valid email address");
            result = false;
        }
        else if (tablecolumn[i].datatype == SqlToDtDataTypeMaping.Phone && !IsValidPhone(ctrl.val())) {
            ctrl.addClass("error");
            ctrl.attr("title", "Please enter Valid Phone No.");
            result = false;
        }
        else {
            ctrl.removeClass("error");
            ctrl.removeAttr("title");
        }
    }
    return result;
}
// Function to clear the new entity form
function clearNewEntityForm() {
    for (i = 0; i < tablecolumn.length; i++) {
        $('#DTE_' + tablecolumn[i].data).removeClass("error");
        $('#DTE_' + tablecolumn[i].data).removeAttr("title");
    }
    $('#newEntityForm input').val('');
    $('#newEntityForm input[type="checkbox"]').attr('checked', false);
}
// Function to Get an entity to edit
function editEntity(id) {
    $.ajax({
        type: "get",
        url: "api/@ReplaceControllerName/" + id,
        contentType: "application/json",
        dataType: 'json',
        // ------v-------use it as the callback function
        success: function (Data, error) {
            for (i = 0; i < tablecolumn.length; i++) {
                $('#DTE_' + tablecolumn[i].data).val(Data[0][tablecolumn[i].data]);
                if (tablecolumn[i].datatype == "text" || tablecolumn[i].datatype == "integer") {
                    $('#DTE_' + tablecolumn[i].data).val(Data[0][tablecolumn[i].data]);
                }
                if (tablecolumn[i].datatype == "date") {
                    $('#DTE_' + tablecolumn[i].data).val(Data[0][tablecolumn[i].data]);
                }
                if (tablecolumn[i].datatype == "checkbox" || tablecolumn[i].datatype == "bit") {
                    $('#DTE_' + tablecolumn[i].data).attr('checked', Data[0][tablecolumn[i].data]);
                }
            }
            addNewEntity();
        },
        error: function (request, error) {
            $.alert({ title: 'Alert!', content: 'An error occurred attempting to save record.' });
        }
    });
}
// Function to delete an entity
function deleteEntity(id) {
    $.confirm({
        title: 'Confirm!',
        content: 'Are you sure you want to delete this record?',
        buttons: {
            Yes: function () {
                $.ajax({
                    type: "delete",
                    url: "api/@ReplaceControllerName/" + id,
                    contentType: "application/json",
                    dataType: 'json',
                    // ------v-------use it as the callback function
                    success: function (Data, error) {
                        $.alert({ title: 'Alert!', content: 'Record deleted successfully.' });
                        $('#@ReplaceTableName').DataTable().ajax.reload();
                    },
                    error: function (request, error) {
                        $.alert({ title: 'Alert!', content: 'An error occurred attempting to delete record.' });
                    }
                });
            },
            No: function () {
            }
        }
    });
}