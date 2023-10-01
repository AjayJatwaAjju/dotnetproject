//Creating a CodeSettings object to auto checked checkbox
var CodeSettings = {};

$(document).ready(function () {
    //Call this finction to select auto tab procedure and providers
    GetDatabaseTypes(bindDatabaseTypes);

    $("#tabsStoredProcedure").tabs(tabsStoredProcedureSetting);
    // Using this for hide show Copy button in Privew Section
    $("#tabsHtml").tabs(privewTabs);
    // Set default value for select database
    //$('#txtServerName').val('192.168.61.178');
    //$('#textUserName').val('rrdba');
    //$('#textPassword').val('Vision@2019');
    $('#txtServerName').val('192.168.61.227');
    $('#textUserName').val('PMeravat');
    $('#textPassword').val('123456789!');

    bindApiType();

    CodeSettings = {
        "ApiType": $('#SelectApiType').val(),
        "GridType": $('#SelectGridType').val(),
        "bSort": $('#DTE_Sorting')[0].checked,
        "searching": $('#DTE_Searching')[0].checked,
        "bServerSide": $('#DTE_bServerSide')[0].checked,
        "bInfo": $('#DTE_bInfo')[0].checked,
        "sScrollX": $('#DTE_ScrollX')[0].checked,
        "sScrollY": $('#DTE_ScrollY')[0].checked,
        "bPaginate": $('#DTE_Paginate')[0].checked,
        "aLengthMenu": $('#aLengthMenu').val(),
        "DatabaseType": $('#SelectDatabaseType').val(),
    }
    if ($('#DTE_Paginate')[0].checked) {
        $("#pageLength").css("display", "block");
    } else {
        $("#pageLength").css("display", "none");
    }
    $("#CustomeNameSpace").val("VEC");
    $("#myBtn").click(function () {
        $('.toast').toast('show');
    });
    $("#copyBtn").hide();
});

function bindDatabaseTypes(data) {
    bindSelect(data, '#SelectDatabaseType', { "Select Server Type": 0 }, 2);
}
$('#ConnectDatabase').click(function (s) {
    ConnectDatabase(false);
});
// Check database Connection valid 
function canConnectDatabase() {
    var result = true;

    if ($('#SelectDatabaseType').val() == "0") {
        result = false;
        $('#SelectDatabaseType').addClass("text-danger is-invalid");
        $('#SelectDatabaseType').attr('title', 'Select Server Type')
    }
    else {
        $('#SelectDatabaseType').removeClass("text-danger is-invalid");
        $('#SelectDatabaseType').attr('title', '')
    }
    if ($('#txtServerName').val() == "") {
        result = false;
        $('#txtServerName').addClass("text-danger is-invalid");
        $('#txtServerName').attr('title', 'Enter Server Name')
    }
    else {
        $('#txtServerName').removeClass("text-danger is-invalid");
        $('#txtServerName').attr('title', '')
    }
    if ($('#textUserName').val() == "") {
        result = false;
        $('#textUserName').addClass("text-danger is-invalid");
        $('#textUserName').attr('title', 'Enter Login Id')
    }
    else {
        $('#textUserName').removeClass("text-danger is-invalid");
        $('#textUserName').attr('title', '')
    }
    if ($('#textPassword').val() == "") {
        result = false;
        $('#textPassword').addClass("text-danger is-invalid");
        $('#textPassword').attr('title', 'Enter Password')
    }
    else {
        $('#textPassword').removeClass("text-danger is-invalid");
        $('#textPassword').attr('title', '')
    }

    return result
}
// database connection establish
$('#selectDatabase').change(function (s) {
    if ($('#selectDatabase').val() == "0" || $('#selectDatabase').val() == null) {
        $('#selectDatabase').addClass("text-danger is-invalid");
        $('#selectDatabase').attr('title', 'Please Select Database');
        bindSelect([], '#SelectTable', { "Select Table": 0 }, 0);
    }
    else {
        $('#selectDatabase').removeClass("text-danger is-invalid");
        $('#selectDatabase').attr('title', '')

        DatabaseType = $('#SelectDatabaseType').val();
        txtServerName = $('#txtServerName').val();
        textUserName = $('#textUserName').val();
        textPassword = $('#textPassword').val();
        textDatabase = $('#selectDatabase').val();
        $('#preloader').show();
        GetTableList(DatabaseType, txtServerName, textUserName, textPassword, textDatabase, function (data) {
            if (!data.isError) {
                bindSelect(data.data, '#SelectTable');
                bindSelect(data.data, '#selectMultipleTables1'); // Populate selectMultipleTables1
                bindSelect(data.data, '#selectMultipleTables2');
                $('#SelectTable').change();
            }
            else {
                bindSelect([], '#SelectTable', { "Select Table": 0 }, 0);
                $.alert({ title: 'Error!', content: data.error });
            }
            $('#preloader').hide();
        });
    }
});
// Select Table options
$('#SelectTable').change(function (s) {
    if ($('#SelectTable').val() == "0") {
        $('#SelectTable').addClass("text-danger is-invalid");
        $('#SelectTable').attr('title', 'Please Select Table')
    }
    else {
        $('#SelectTable').removeClass("text-danger is-invalid");
        $('#SelectTable').attr('title', '')
    }
});
var tablecolumn = [];
var tablecolumnData = [];
// This function to show data aording to condition
function GenerateCode() {
    tablecolumn = [];
    $("#tabsStoredProcedure").tabs("option", "active", 1);
    $("#tabsStoredProcedure").tabs("option", "active", 0);
    $("#tabsFunctions").tabs("option", "active", 1);
    $("#tabsFunctions").tabs("option", "active", 0);
    controlshtml = "";
    var tablecolumnDatanew = [];
    $('#TableDetails tbody tr').each(function (key, data) {
        var samplecontrolhtml = '<div class="col-md-6"><div class="form-group"><div class="form-group"><label for="@ColumnName">@ColumnName @isRequired</label>@inputcontrol</div></div></div>';
        data1 = $('#TableDetails').DataTable().rows(data).data()[0];
        var alltd = $(data).find('td');
        var newcolumn = {
            title: alltd[0].innerText,
            data: alltd[0].innerText,
            "sWidth": "140px",
            "sClass": "text-right " + alltd[0].innerText,
            datatype: $(alltd[1]).find('select').val(),
            SqlDatatype: $(alltd[1]).find('select').val(),
            mRender: dataTablerender,
            "@mRender": "@dataTablerender",
            bVisible: $(alltd[2]).find('input')[0].checked,
            is_nullable: $(alltd[5]).find('input')[0].checked,
            primaryKey: data1.primaryKey
        }
        data1.datatype = $(alltd[1]).find('select').val();
        data1.bVisible = $(alltd[2]).find('input')[0].checked;
        data1.allowfiltering = $(alltd[3]).find('input')[0].checked;
        data1.iseditable = $(alltd[4]).find('input')[0].checked;
        data1.is_nullable = $(alltd[5]).find('input')[0].checked;
        tablecolumn.push(newcolumn);
        tablecolumnDatanew.push(data1);
        if (!newcolumn.is_nullable) {
            samplecontrolhtml = samplecontrolhtml.replaceAll("@isRequired", '<span class="required">*</span>');
        }
        else {
            samplecontrolhtml = samplecontrolhtml.replaceAll("@isRequired", '');
        }
        if ((!data1.primaryKey && data1.iseditable) || !data1.is_nullable) {
            if (newcolumn.datatype == "text" || newcolumn.datatype == "integer" || newcolumn.datatype == "Phone" || newcolumn.datatype == "Email") {
                samplecontrolhtml = samplecontrolhtml.replaceAll("@inputcontrol", '<input type="text" class="form-control" id="DTE_@ColumnName" name="@ColumnName" placeholder="Enter @ColumnName">');
            }
            else if (newcolumn.datatype == "Password") {
                samplecontrolhtml = samplecontrolhtml.replaceAll("@inputcontrol", '<input type="password" class="form-control" id="DTE_@ColumnName" name="@ColumnName" placeholder="Enter @ColumnName">');
            }
            else if (newcolumn.datatype == "date") {
                samplecontrolhtml = samplecontrolhtml.replaceAll("@inputcontrol", '<input type="text" class="form-control" id="DTE_@ColumnName" name="@ColumnName" placeholder="MM/dd/yyyy">');
            }
            else if (newcolumn.datatype == "checkbox" || newcolumn.datatype == "bit") {
                samplecontrolhtml = samplecontrolhtml.replaceAll("@inputcontrol", '<div class="switch"><input type="checkbox" id="DTE_@ColumnName" checked="checked"><span class="slider round"></span></div>');
            }
            else if (newcolumn.datatype == SqlToDtDataTypeMaping.drop_down) {
                samplecontrolhtml = samplecontrolhtml.replaceAll("@inputcontrol", '<Select id="DTE_@ColumnName" class="form-control"><option value="0">Select @ColumnName</option></Select>');
            }
            else if (newcolumn.datatype == SqlToDtDataTypeMaping.hidden) {
                samplecontrolhtml = samplecontrolhtml.replaceAll("@inputcontrol", '<input type="hidden" class="form-control" id="DTE_@ColumnName" name="@ColumnName" placeholder="MM/dd/yyyy">');
                samplecontrolhtml = samplecontrolhtml.replaceAll("col-md-6", 'col-md-6 hiddden');
            }
            else {
                samplecontrolhtml = samplecontrolhtml.replaceAll("@inputcontrol", '<input type="text" class="form-control" id="DTE_@ColumnName" name="@ColumnName" placeholder="Enter @ColumnName">');
            }
        }
        else {
            samplecontrolhtml = samplecontrolhtml.replaceAll("@inputcontrol", '<input type="text" class="form-control" id="DTE_@ColumnName" name="@ColumnName" placeholder="Enter @ColumnName">');
        }
        if (data1.primaryKey || newcolumn.datatype == SqlToDtDataTypeMaping.hidden) {
            samplecontrolhtml = samplecontrolhtml.replaceAll("col-md-6", 'col-md-6 hiddden');
        }
        if ((!data1.primaryKey && data1.iseditable) || !data1.is_nullable) {
            controlshtml = controlshtml + samplecontrolhtml.replaceAll("@ColumnName", alltd[0].innerText);
        }
    });
    PK = $('#TableDetails').DataTable().rows().data().filter(w => w.primaryKey == true)[0];
    if (PK) {
        tablecolumn.push({ "title": "", "data": PK.columnName, "sWidth": "50px", "sClass": "text-right ActionButton", "datatype": "actionbutton", "SqlDatatype": "actionbutton", "mRender": dataTablerender, "bVisible": true, "is_nullable": true, "primaryKey": false, "@mRender": "@dataTablerender", });

        SelectTable = $('#SelectTable').val();
        if (SelectTable.indexOf('.') > 0) {
            SelectTable = SelectTable.split('.')[1];
        }
       /* $('#tabs-preview').html('');*/
        tablestring = '<table class="table table-hover  dataTable no-footer" style="min-width: 100%; margin-left: 0px; width: auto;" role="grid" id="' + SelectTable.replace('.', '') + '"></table>';

        GetFileDetail(LookupServiceSettings.htmlDatatableExample, function (data) {
            if (SelectTable.indexOf('.') > 0) {
                SelectTable = SelectTable.split('.')[1];
            }
            data = data.replaceAll('@ReplaceTableHtml', tablestring)
            data = data.replaceAll('@ReplaceTableName', SelectTable.replace('.', ''))

            data = data.replaceAll('@EditDetailsSection', controlshtml)
            $('#tabs-preview').html(data.substring(data.indexOf('@StartpriviewSection') + "@StartpriviewSection".length, data.indexOf('@endpriviewSection')));
            data = data.replaceAll('@StartpriviewSection', '')
            data = data.replaceAll('@endpriviewSection', '')
            var encodedStr = data.replace(/[\u00A0-\u9999<>\&]/g, function (i) {
                return '&#' + i.charCodeAt(0) + ';';
            });
            $('#tabs-html').html('<pre><code>' + encodedStr + '</code></pre>');
            if (CodeSettings.DatabaseType == "1") {
                scriptName = LookupServiceSettings.javascriptPageSampleSQL;
            } else {
                scriptName = LookupServiceSettings.javascriptPageSampleNew;
            }
            if (CodeSettings.ApiType == ApiType.ASHX) {
                scriptName = LookupServiceSettings.javascriptPageSampleRshx;
            }
            GetFileDetail(scriptName, function (data) {
                data = replacetableSetting(data);

                if (CodeSettings.ApiType == ApiType.Controller) {
                    jsScript = data;
                    jsScript = jsScript.replaceAll("@ReplaceControllerName", "getdata");
                    $('#tabs-html').append("<script>" + jsScript + "</script>")
                }
                else {
                    if ($("#Multi_Procedure").is(":checked")) {
                        GetFileDetail(LookupServiceSettings.javascriptPageSample, function (jsScript) {
                            jsScript = replacetableSetting(jsScript);
                            jsScript = jsScript.replaceAll("@ReplaceControllerName", "getdata");
                            $('#tabs-html').append("<script>" + jsScript + "</script>")
                        });
                    } else {
                        GetFileDetail(LookupServiceSettings.javascriptPageSampleNew, function (jsScript) {
                            jsScript = replacetableSetting(jsScript);
                            jsScript = jsScript.replaceAll("@ReplaceControllerName", "getdata");
                            $('#tabs-html').append("<script>" + jsScript + "</script>")
                        });
                    }
                }
                data = data.replaceAll("@ReplaceControllerName", SelectTable);

                $('#tabs-Js').html('<pre><code>' + data + '</code></pre>');
            });
        });

        //Controller
        if ($("#Multi_Procedure").is(":checked")) {
            if (CodeSettings.DatabaseType == "1") {
                // Controller SQL
                scriptName = LookupServiceSettings.EntityControllerSQL;
                GetbackendCode(scriptName, '#tabs-Controler');
                // Service SQL
                scriptName = LookupServiceSettings.EntityServiceSQL;
                GetbackendCode(scriptName, '#tabs-Bussiness');
                // DAL SQL
                scriptName = LookupServiceSettings.EntityProviderMSSQL;
                GetbackendCodeDAL(scriptName, '#tabs-DAL');
            } else {
                // Controller PGSQL
                scriptName = LookupServiceSettings.EntityControllerNew;
                GetbackendCode(scriptName, '#tabs-Controler');
                //Service PGSQL
                scriptName = LookupServiceSettings.EntityServicePostgreSQL;
                GetbackendCode(scriptName, '#tabs-Bussiness');
                //DAL PGSQL
                scriptName = LookupServiceSettings.EntityBusinessPostgreSQLUSP;
                GetbackendCodeDAL(scriptName, '#tabs-DAL');
            }
        } else {
            if (CodeSettings.DatabaseType == "1") {
                // Controller SQL
                scriptName = LookupServiceSettings.EntityControllerSQL2USP;
                GetbackendCode(scriptName, '#tabs-Controler');
                // DAL SQL
                scriptName = LookupServiceSettings.EntityProviderMSSQL2USP;
                GetbackendCode(scriptName, '#tabs-DAL');
            } else {
                // Controller PGSQL
                scriptName = LookupServiceSettings.EntityController;
                GetbackendCode(scriptName, '#tabs-Controler');
                // DAL PGSQL
                scriptName = LookupServiceSettings.EntityProviderPostgreSQL;
                GetbackendCode(scriptName, '#tabs-DAL');
            }
        }

        if (CodeSettings.ApiType == ApiType.ASHX) {
            scriptName = LookupServiceSettings.RequestHandler;
        }
        scriptName = LookupServiceSettings.DataTableSearchData;
        GetbackendCode(scriptName, '#tabs-Class');
    }
    else {
        $.alert({ title: 'Error!', content: "Add a primary key to continue." });
    }
}
// Replace data its name select in tab option
function GetbackendCode(scriptName, targetdiv) {
    GetFileDetail(scriptName, function (data) {
        var customeNameSpace = $("#CustomeNameSpace").val();
        var primaryKey = tablecolumnData.find(w => w.primaryKey)
        SelectTable = $('#SelectTable').val();
        if (SelectTable.indexOf('.') > 0)
            SelectTable = SelectTable.split('.')[1]
        if (data) {
            data = data.replaceAll('@ReplaceTableName', SelectTable);
            data = data.replaceAll('@ReplaceLowerTableName', SelectTable.toLowerCase());
            data = data.replaceAll('@ReplaceTablePK', primaryKey.columnName);
            data = data.replaceAll('@CNameSpace', customeNameSpace);
        }
        var encodedStr = data.replace(/[\u00A0-\u9999<>\&]/g, function (i) {
            return '&#' + i.charCodeAt(0) + ';';
        });
        $(targetdiv).html('<pre><code>' + encodedStr + '</code></pre>')
        $('#preloader').hide();
    });
}
// Replace data its name select in tab option Creating get set class as well
function GetbackendCodeDAL(scriptName, targetdiv) {
    GetFileDetail(scriptName, function (data) {
        var customeNameSpace = $("#CustomeNameSpace").val();
        var primaryKey = tablecolumnData.find(w => w.primaryKey)
        var columnName = [];
        var UpdateColumn = [];
        var tableName = SelectTable = $('#SelectTable').val();
        var dataTableClass = [];
        var assignDatafromTable = [];
        var ProviderClass;
        for (let i = 0; i < tablecolumnData.length; i++) {
            UpdateColumn.push(`param[${i}].ParameterName = "@${tablecolumnData[i].columnName}";
            param[${i}].Value = ${tableName.slice("public.".length)}.${tablecolumnData[i].columnName};\r\n`)
            dataTableClass.push(`public ${tablecolumnData[i].datatype1} ${tablecolumnData[i].columnName} { get; set; }\r\n`);
            assignDatafromTable.push(`${tableName.slice("public.".length)}Info.${tablecolumnData[i].columnName} = dr["${tablecolumnData[i].columnName}"].ToString();\r\n`)
            if (i == 0) {
                continue;
            } else {
                columnName.push(`param[${i - 1}].ParameterName = "@${tablecolumnData[i].columnName}";
            param[${i - 1}].Value = ${tableName.slice("public.".length)}.${tablecolumnData[i].columnName};\r\n`)
            }
        }
        SelectTable = $('#SelectTable').val();
        if (SelectTable.indexOf('.') > 0)
            SelectTable = SelectTable.split('.')[1];
        if (data) {
            data = data.replaceAll('@ReplaceTableName', SelectTable);
            data = data.replaceAll('@ReplaceLowerTableName', SelectTable.toLowerCase());
            data = data.replaceAll('@ReplaceTablePK', primaryKey.columnName);
            data = data.replaceAll('@CNameSpace', customeNameSpace);
            data = data.replaceAll('@TotalCount', columnName.length + 1);
            data = data.replaceAll('@UpdateData', UpdateColumn.length + 1);
            data = data.replaceAll('@UpdateInputData', UpdateColumn.toString().replaceAll(",", " "));
            data = data.replaceAll('@InputData', columnName.toString().replaceAll(",", " "));
            data = data.replaceAll('@assignDatafromTable', assignDatafromTable.toString().replaceAll(",", " "))
            data = data.replaceAll('@Addnew', columnName.length);
            data = data.replaceAll('@Updatenew', UpdateColumn.length);
            dataTableClass = dataTableClass.toString().replaceAll(" character varying", " string")
            dataTableClass = dataTableClass.toString().replaceAll(" integer", " int")
            dataTableClass = dataTableClass.toString().replaceAll(" timestamp without time zone", " DateTime")
            dataTableClass = dataTableClass.toString().replaceAll(" smallint", " Boolean")
            dataTableClass = dataTableClass.toString().replaceAll(" text", " string")
            dataTableClass = dataTableClass.toString().replaceAll(" bigint", " int")
            dataTableClass = dataTableClass.toString().replaceAll(" nvarchar", " string")
            dataTableClass = dataTableClass.toString().replaceAll(",", " ")
            data = data.replaceAll('@dataTableClass', dataTableClass)
        }
        var encodedStr = data.replace(/[\u00A0-\u9999<>\&]/g, function (i) {
            return '&#' + i.charCodeAt(0) + ';';
        });
        $(targetdiv).html('<pre><code>' + encodedStr + '</code></pre>')
        $('#preloader').hide();
    });
}
// Using this for hide show Copy button in Privew Section
var privewTabs = {
    activate: function (event, ui) {
        tabtype = ui.newTab.attr('tabtype');
        if (tabtype == "PrivewTab") {
            GenerateCode()
            $("#copyBtn").hide();
        } else if (tabtype == "LinksTab") {
            $("#copyBtn").hide();
        }else {
            $("#copyBtn").show();
        }
    }
}
// Creating dynamic Procedure usign switch case and Replace using @ReplaceTableName and other
var tabsStoredProcedureSetting = {
    activate: function (event, ui) {
        let scriptName = "";
        tabtype = ui.newTab.attr('tabtype');
        iscode = false;
        DatabaseType = $('#SelectDatabaseType').val();
        scriptName = LookupServiceSettings[tabtype + $('#SelectDatabaseType').find(":selected").text()];
        if (!iscode) {
            GetFileDetail(scriptName, function (data) {
                console.log(ui.newTab);
                let targetdiv = ui.newTab.find('a').attr('href');
                SelectTable = $('#SelectTable').val();
                ReplaceSchema = "";
                if (SelectTable.indexOf('.') > 0) {
                    ReplaceSchema = SelectTable.split('.')[0];
                    SelectTable = SelectTable.split('.')[1];
                }
                if (data) {
                    data = data.replaceAll('@ReplaceTableName', SelectTable);
                    data = data.replaceAll('@ReplaceSchema', ReplaceSchema);
                    data = data.replaceAll('@ReplacePROCEDUREName', SelectTable.replaceAll('.', '_'));
                }
                var primaryKey = tablecolumnData.find(w => w.primaryKey)
                var userName = tablecolumnData[1].columnName;
                switch (tabtype.toString().toLowerCase()) {
                    case "updatetable":
                        {
                            var functionparameter = "";
                            var UpdateQueryparameter = "";
                            var wherecondition = "";
                            tablecolumnData.forEach(function (val, key) {
                                try {
                                    if (primaryKey.columnName == val.columnName) {
                                        wherecondition = "WHERE " + val.columnName + "=@" + val.columnName + "";
                                        fkUserId = val.columnName;
                                    }
                                    else {
                                        //UpdateQueryparameter = ((UpdateQueryparameter != '') ? UpdateQueryparameter + ",\n" : '') + val.columnName + ' = @' + val.columnName;
                                    }
                                    if (DatabaseType == "2") {
                                        functionparameter = ((key != 0) ? functionparameter + ",\n" : '') + '(json_array_elements(inputJson::JSON) ->> \'' + val.columnName + '\')::' + val.datatype1.toUpperCase() + ' "' + val.columnName + '"';
                                        UpdateQueryparameter = ((UpdateQueryparameter != '') ? UpdateQueryparameter + ",\n" : '') + val.columnName + ' = temptable."' + val.columnName + '"';
                                    }
                                    else if (DatabaseType == "1") {
                                        functionparameter = ((key != 0) ? functionparameter + ",\n" : '') + "@" + val.columnName + ' AS ' + val.datatype1 + (val.datatype1 == 'varchar' || val.datatype1 == 'varchar' ? '(500)' : '');
                                    }
                                    if (key == tablecolumnData.length - 1) {
                                        data = data.replaceAll('@ReplaceParameter', functionparameter);
                                        data = data.replaceAll('@ReplaceUpdateParameter', UpdateQueryparameter);
                                        data = data.replaceAll('@Replacewherecondition', wherecondition);
                                        data = data.replaceAll('@pkColumnName', primaryKey.columnName);
                                    }
                                }
                                catch (e) {
                                    console.log(e.message);
                                }
                            });
                            break;
                        }
                    case "insertintotable":
                        {
                            var functionparameter = "";
                            var ReplaceInsertColumn = "";
                            var ReplaceInsertParameter = "";
                            var wherecondition = "";
                            tablecolumnData.forEach(function (val, key) {
                                if (val.iseditable) {
                                    ReplaceInsertColumn = ((ReplaceInsertColumn != '') ? ReplaceInsertColumn + ",\n" : '') + val.columnName;
                                }
                                if (val.iseditable) {
                                    ReplaceInsertParameter = ((ReplaceInsertParameter != '') ? ReplaceInsertParameter + ",\n" : '') + '@' + val.columnName;
                                }

                                if (DatabaseType == "2") {
                                    functionparameter = ((key != 0) ? functionparameter + ",\n" : '') + '(json_array_elements(inputJson::JSON) ->> \'' + val.columnName + '\')::' + val.datatype1.toUpperCase() + ' "' + val.columnName + '"';
                                }
                                else if (DatabaseType == "1") {
                                    functionparameter = ((key != 0) ? functionparameter + ",\n" : '') + "@" + val.columnName + ' as ' + val.datatype1 + (val.datatype1 == 'varchar' || val.datatype1 == 'varchar' ? '(500)' : '');
                                }
                                if (key == tablecolumnData.length - 1) {
                                    data = data.replaceAll('@ReplaceParameter', functionparameter);
                                    data = data.replaceAll('@ReplaceInsertColumn', ReplaceInsertColumn);
                                    data = data.replaceAll('@ReplaceInsertParameter', ReplaceInsertParameter);
                                    data = data.replaceAll('@pkColumnName', primaryKey.columnName);
                                }
                            });
                            break;
                        }
                    case "deletefromtable":
                        {
                            if (primaryKey) {
                                wherecondition = "WHERE " + primaryKey.columnName + "=@" + primaryKey.columnName + "";
                                functionparameter = "@" + primaryKey.columnName + ' AS ' + primaryKey.datatype1 + (primaryKey.datatype1 == 'varchar' ? '(500)' : '');
                                data = data.replaceAll('@Replacewherecondition', wherecondition);
                                data = data.replaceAll('@ReplaceParameter', functionparameter);
                                data = data.replaceAll('@pkColumnName', primaryKey.columnName);
                            }
                            break;
                        }
                    case "setoprations":
                        {
                            var functionparameter = "";
                            var ReplaceInsertColumn = "";
                            var UpdateQueryparameter = "";
                            var wherecondition = "";
                            var ReplaceInsertParameter = "";
                            tablecolumnData.forEach(function (val, key) {
                                try {
                                    if (primaryKey.columnName == val.columnName) {
                                        wherecondition = "WHERE " + val.columnName + "=@" + val.columnName + "";
                                    }
                                    else {
                                        if (DatabaseType == "2") {
                                            UpdateQueryparameter = ((UpdateQueryparameter != '') ? UpdateQueryparameter + ",\n" : '') + val.columnName + ' = temptable."' + val.columnName + '"';
                                            ReplaceInsertColumn = ((ReplaceInsertColumn != '') ? ReplaceInsertColumn + ",\n" : '') + val.columnName;
                                            ReplaceInsertParameter = ((ReplaceInsertParameter != '') ? ReplaceInsertParameter + ",\n" : '') + '@' + val.columnName;
                                        }
                                        else if (DatabaseType == "1") {
                                            UpdateQueryparameter = ((UpdateQueryparameter != '') ? UpdateQueryparameter + ",\n" : '') + val.columnName + ' = @' + val.columnName;
                                            ReplaceInsertColumn = ((ReplaceInsertColumn != '') ? ReplaceInsertColumn + ",\n" : '') + val.columnName;
                                            ReplaceInsertParameter = ((ReplaceInsertParameter != '') ? ReplaceInsertParameter + ",\n" : '') + '@' + val.columnName;
                                        }
                                    }
                                    if (DatabaseType == "2") {
                                        functionparameter = ((key != 0) ? functionparameter + ",\n" : '') + '(json_array_elements(inputJson::JSON) ->> \'' + val.columnName + '\')::' + val.datatype1.toUpperCase() + ' "' + val.columnName + '"';
                                    }
                                    else if (DatabaseType == "1") {
                                        functionparameter = ((key != 0) ? functionparameter + ",\n" : '') + "@" + val.columnName + ' as ' + val.datatype1.toUpperCase() + (val.datatype1 == 'varchar' || val.datatype1 == 'varchar' ? '(500)' : '');
                                    }
                                    if (key == tablecolumnData.length - 1) {
                                        data = data.replaceAll('@ReplaceParameter', functionparameter);
                                        data = data.replaceAll('@ReplaceUpdateParameter', UpdateQueryparameter);
                                        data = data.replaceAll('@ReplaceInsertColumn', ReplaceInsertColumn);
                                        data = data.replaceAll('@pkColumnName', primaryKey.columnName);
                                        data = data.replaceAll('@ReplaceInsertParameter', ReplaceInsertParameter);
                                    }
                                }
                                catch (e) {
                                    console.log(e.message);
                                }
                            });
                            break;
                        }
                    case "gettableinfo":
                        {
                            var functionparameter = "";
                            var ReplaceOrderBy = 'ORDER BY (CASE WHEN fltr.column = \'\' and upper(fltr.dir) = \'\' THEN 0 end) ASC';
                            var UpdateQueryparameter = "";
                            var wherecondition = "";
                            if (primaryKey) {
                                functionparameter = "(json_array_elements(inputJson::JSON) ->> 'pkvalue')::BIGINT \"" + primaryKey.columnName + "\"";

                                ReplaceOrderBy = 'ORDER BY (CASE WHEN fltr.column = \'\' AND upper(fltr.dir) = \'\' THEN fltr."' + primaryKey.columnName + '" END) ASC';
                                data = data.replaceAll('@pkColumnName', primaryKey.columnName);
                            }
                            tablecolumnData.forEach(function (val, key) {
                                try {
                                    if (val.isVisibale && val.allowfiltering) {
                                        if (DatabaseType == "2") {
                                            wherecondition = wherecondition + "\n OR CAST(basetable." + val.columnName + " AS CHARACTER VARYING) LIKE '%' || fltr.search || '%'";
                                            ReplaceOrderBy = ReplaceOrderBy + "\n,(CASE WHEN fltr.column = '" + val.columnName + "' AND UPPER(fltr.dir) = 'ASC' THEN fltr.\"" + val.columnName + "\" END) ASC,(CASE WHEN fltr.column = '" + val.columnName + "' AND UPPER(fltr.dir) = 'DESC' THEN fltr.\"" + val.columnName + "\" END) DESC"
                                        }
                                        else if (DatabaseType == "1") {
                                            wherecondition = ((UpdateQueryparameter != '') ? UpdateQueryparameter + ",\n" : '') + val.columnName + ' = @' + val.columnName;
                                        }
                                    }
                                    if (DatabaseType == "2") {
                                        functionparameter = ((key != 0) ? functionparameter + ",\n" : '') + '(json_array_elements(inputJson::JSON) ->> \'' + val.columnName + '\')::' + val.datatype1.toUpperCase() + ' "' + val.columnName + '"';
                                    }
                                    else if (DatabaseType == "1") {
                                        functionparameter = ((key != 0) ? functionparameter + ",\n" : '') + "@" + val.columnName + ' as ' + val.datatype1 + (val.datatype1 == 'VARCHAR' || val.datatype1 == 'NVARCHAR' ? '(500)' : '');
                                    }
                                    if (key == tablecolumnData.length - 1) {
                                        data = data.replaceAll('@ReplaceParameter', functionparameter);
                                        data = data.replaceAll('@ReplaceWhereCondition', wherecondition);
                                        data = data.replaceAll('@ReplaceOrderBY', ReplaceOrderBy);
                                        data = data.replaceAll('@pkColumnName', primaryKey.columnName);
                                    }
                                }
                                catch (e) {
                                    console.log(e.message);
                                }
                            });
                            break;
                        }
                    case "gettableinfobyid":
                        {
                            if (primaryKey) {
                                wherecondition = "WHERE " + primaryKey.columnName + "=@" + primaryKey.columnName + "";
                                functionparameter = "@" + primaryKey.columnName + ' AS ' + primaryKey.datatype1 + (primaryKey.datatype1 == 'VARCHAR' ? '(500)' : '');
                                data = data.replaceAll('@Replacewherecondition', wherecondition);
                                data = data.replaceAll('@ReplaceParameter', functionparameter);
                            }
                            break;
                        }
                    case "fngetusernamebyid":
                        {
                            var functionparameter = "";
                            var ReplaceOrderBy = 'ORDER BY (CASE WHEN fltr.column = \'\' and upper(fltr.dir) = \'\' THEN 0 end) ASC';
                            var UpdateQueryparameter = "";
                            var wherecondition = "";
                            if (primaryKey) {
                                functionparameter = "(json_array_elements(inputJson::JSON) ->> 'pkvalue')::BIGINT \"" + primaryKey.columnName + "\"";

                                ReplaceOrderBy = 'ORDER BY (CASE WHEN fltr.column = \'\' AND upper(fltr.dir) = \'\' THEN fltr."' + primaryKey.columnName + '" END) ASC';
                                data = data.replaceAll('@pkColumnName', primaryKey.columnName);
                            }
                            tablecolumnData.forEach(function (val, key) {
                                try {
                                    if (val.isVisibale && val.allowfiltering) {
                                        if (DatabaseType == "2") {
                                            wherecondition = wherecondition + "\n							  OR CAST(basetable." + val.columnName + " AS CHARACTER VARYING) LIKE '%' || fltr.search || '%'";
                                            ReplaceOrderBy = ReplaceOrderBy + "\n							  ,(CASE WHEN fltr.column = '" + val.columnName + "' AND UPPER(fltr.dir) = 'ASC' THEN fltr.\"" + val.columnName + "\" END) ASC,(CASE WHEN fltr.column = '" + val.columnName + "' AND UPPER(fltr.dir) = 'DESC' THEN fltr.\"" + val.columnName + "\" END) DESC"
                                        }
                                        else if (DatabaseType == "1") {
                                            wherecondition = ((UpdateQueryparameter != '') ? UpdateQueryparameter + ",\n" : '') + val.columnName + ' = @' + val.columnName;
                                        }
                                    }
                                    if (DatabaseType == "2") {
                                        functionparameter = ((key != 0) ? functionparameter + ",\n" : '') + '(json_array_elements(inputJson::JSON) ->> \'' + val.columnName + '\')::' + val.datatype1.toUpperCase() + ' "' + val.columnName + '"';
                                    }
                                    else if (DatabaseType == "1") {
                                        functionparameter = ((key != 0) ? functionparameter + ",\n" : '') + "@" + val.columnName + ' as ' + val.datatype1 + (val.datatype1 == 'VARCHAR' || val.datatype1 == 'NVARCHAR' ? '(500)' : '');
                                    }
                                    if (key == tablecolumnData.length - 1) {
                                        data = data.replaceAll('@ReplaceParameter', functionparameter);
                                        data = data.replaceAll('@ReplaceWhereCondition', wherecondition);
                                        data = data.replaceAll('@ReplaceOrderBY', ReplaceOrderBy);
                                        data = data.replaceAll('@pkColumnName', primaryKey.columnName);
                                    }
                                }
                                catch (e) {
                                    console.log(e.message);
                                }
                            });
                            break;
                        }
                }

                $(targetdiv).html('<pre><code>' + data + '</code></pre>')
                $('#preloader').hide();
            });
        }
    }
}
// This function for Copy Text showing in modules
function copyToClipboard(ctrl) {
    if ($("#" + ctrl + " li.ui-state-active a").attr('href') == "tabs-preview" || $("#" + ctrl + " li.ui-state-active a").attr('href') == "tabs-Links") {
        return;
    }
    var copyText = $($("#" + ctrl + " li.ui-state-active a").attr('href')).children('pre').text()
    // Copy the text inside the text field
    try {
        navigator.clipboard.writeText(copyText).then(() => {
            $('#textCopy').toast('show');
            $('#textCopyStore').toast('show');
        });
    }
    catch {
        $('#textNotCopy').show();
        $('#textNotCopyStore').show();
    }
}
// Show Setting Modal
function hideShowSettingModal(show) {
    if (show) {
        $('#SettingModal').show();
    }
    else {
        $('#SettingModal').hide();
    }
}
// Save Setting after select Data Type
function saveSettings() {
    SelectedTable = $('#SelectTable').val();
    SelectedSchema = $('#SelectTable').val();
    SelectedSchema = LookupServiceSettings.defaultSchemaPostgreSQL;
    if ($('#SelectDatabaseType').val() == "1") {
        SelectedSchema = LookupServiceSettings.defaultSchemaSql;
    }
    if (SelectedTable.indexOf('.') > 0) {
        SelectedSchema = SelectedTable.split('.')[0];
        SelectedTable = SelectedTable.split('.')[1];
    }
    CodeSettings = {
        "ApiType": $('#SelectApiType').val(),
        "GridType": $('#SelectGridType').val(),
        "bSort": $('#DTE_Sorting')[0].checked,
        "searching": $('#DTE_Searching')[0].checked,
        "bServerSide": $('#DTE_bServerSide')[0].checked,
        "bInfo": $('#DTE_bInfo')[0].checked,
        "sScrollX": $('#DTE_ScrollX')[0].checked,
        "sScrollY": $('#DTE_ScrollY')[0].checked,
        "bPaginate": $('#DTE_Paginate')[0].checked,
        "aLengthMenu": $('#aLengthMenu').val(),
        "DatabaseType": $('#SelectDatabaseType').val(),
        "SelectedTable": SelectedTable,
        "SelectedSchema": SelectedSchema,
    }

    hideShowSettingModal(false);
}
var ApiType = [];
// Select API Type Like Controller and ASHX.
function bindApiType() {
    ApiType = GetApiList();
    bindSelect(ApiType, '#SelectApiType', null, 1);
}
// Using Back buttion in jquery Tab bar
$('.accordion-prev').click(function (a, b) {
    $($(a.currentTarget).attr('next-step')).click();
    step = "1";
    step = $(a.currentTarget).attr('step')
    switch (step) {
        case "Dbconnection":
            {
            }
            break;
        case "TableSelection":
            {
                $('#preloader').show();
                $('.accordion-Dbconnection').show();
                $('.accordion-TableSelection').hide();
                setTimeout(function () {
                    $('#preloader').hide();
                }, 1000);
                $('#successAlert').hide();
                $('.toast').toast('hide');
                $('#errorAlert').hide();
                $('.toast').toast('hide');
            }
            break;
        case "FilterOptions":
            {
                $('.accordion-TableSelection').show();
                $('.accordion-headingFilterOptions').hide();
                $('#tabs-insertIntoTable').hide();
            }
            break;
        case "headingFrontend":
            {
                $('.accordion-headingFilterOptions').show();
                $('.accordion-Frontend').hide();
            }
            break;
        case "headingUSPSelection":
            {
                $('.accordion-StoredProcedure').hide();
                $('.accordion-Frontend').show();
            }
            break;
    }
});
// Using Next buttion in jquery Tab bar
$('.accordion-next').click(function (a, b) {
    step = "1";
    step = $(a.currentTarget).attr('step')
    switch (step) {
        case "Dbconnection":
            {
                ConnectDatabase(true);
            }
            break;
        case "TableSelection":
            {
                var result = true;

                if ($('#selectDatabase').val() == "0") {
                    console.log($('#selectDatabase').val(),"-----------")
                    result = false;
                    $('#selectDatabase').addClass("text-danger is-invalid");
                    $('#selectDatabase').attr('title', 'Select Database')
                }
                else {
                    console.log($('#selectDatabase').val(), "-----else------")
                    $('#selectDatabase').removeClass("text-danger is-invalid");
                    $('#selectDatabase').attr('title', '')
                }
                if ($('#SelectTable').val() == "0") {
                    result = false;
                    $('#SelectTable').addClass("text-danger is-invalid");
                    $('#SelectTable').attr('title', 'Select Table')
                }
                else {
                    $('#SelectTable').removeClass("text-danger is-invalid");
                    $('#SelectTable').attr('title', '')
                }
                if (result) {
                    $('.accordion-TableSelection').hide();
                    $('.accordion-headingFilterOptions').show();
                    saveSettings();
                    bindTableDetails();
                }
                // Hide Show procedure tabs according to 2 USP Or 5 USP
                if ($("#Multi_Procedure").is(":checked")) {
                    $("#errorLogUSP").css('display', 'block');
                    $("#InsertUSP").css('display', 'block');
                    $("#UpdateUSP").css('display', 'block');
                    $("#DeleteUSP").css('display', 'block');
                    $("#GetInfoByIdUSP").css('display', 'block');
                    $("#GetDataUSP").css('display', 'block');
                    $("#newProcedureDal").css('display', 'block');
                    $("#setOperations").css('display', 'none');
                    $("#GetInfo").css('display', 'none');
                } else {
                    $("#errorLogUSP").css('display', 'none');
                    $("#InsertUSP").css('display', 'none');
                    $("#UpdateUSP").css('display', 'none');
                    $("#DeleteUSP").css('display', 'none');
                    $("#GetInfoByIdUSP").css('display', 'none');
                    $("#GetDataUSP").css('display', 'none');
                    $("#newProcedureDal").css('display', 'none');
                    $("#setOperations").css('display', 'block');
                    $("#GetInfo").css('display', 'block');
                }
                // Hide show Accrding to database Type Helper
                if ($('#SelectDatabaseType').val() == "1") {
                    $('#PGHepler').hide();
                    $('#SQlHelper').show();
                } else {
                    $('#PGHepler').show();
                    $('#SQlHelper').hide();
                    $("#GetInfoByIdUSP").css('display', 'none');
                }
            }
            break;
        case "FilterOptions":
            {
                GenerateCode();
                $('.accordion-headingFilterOptions').hide();
                $('.accordion-Frontend').show();
                if ($("#Multi_Procedure").is(":checked")) {
                    $("#tabsStoredProcedure").tabs("option", "active", 2);
                } else {
                    $("#tabsStoredProcedure").tabs("option", "active", 0);
                }
            }
            break;
        case "headingFrontend":
            {
                $('.accordion-StoredProcedure').show();
                $('.accordion-Frontend').hide();
            }
            break;
    }
});
// Connection of DataBase here
function ConnectDatabase(movetoNext) {
    if (canConnectDatabase()) {
        DatabaseType = $('#SelectDatabaseType').val();
        txtServerName = $('#txtServerName').val();
        textUserName = $('#textUserName').val();
        textPassword = $('#textPassword').val();

        CodeSettings.DatabaseType = $('#SelectDatabaseType').val();

        $('#preloader').show();
        GetDatabases(DatabaseType, txtServerName, textUserName, textPassword, function (data) {
            $('#successAlert').hide();
            $('#errorAlert').hide();
            if (!data.isError) {
                    // if you want to select database you want add here uncomment this line
                    //bindSelect(data.data, '#selectDatabase', { "Select Database": 0 }, 'Select Database');
                    bindSelect(data.data, '#selectDatabase');
                $('#selectDatabase').change();
                if (movetoNext) {
                    $('.accordion-Dbconnection').hide();
                    $('.accordion-TableSelection').show();
                }
                $('#successAlert').show();
                $('.toast').toast('show');
            }
            else {
                bindSelect([], '#selectDatabase', { "Select Database": 0 }, 0);
                bindSelect([], '#SelectTable', { "Select Table": 0 }, 0);
                $('#errorAlert').show();
                 $('.toast').toast('show');
                // For bind server side error message
                /* document.getElementById("errorAlert").innerHTML = data.error;*/
            }
            $('#preloader').hide();
        });
    }
}
// bind table details and get form Value
function bindTableDetails() {
    DatabaseType = $('#SelectDatabaseType').val();
    txtServerName = $('#txtServerName').val();
    textUserName = $('#textUserName').val();
    textPassword = $('#textPassword').val();
    textDatabase = $('#selectDatabase').val();
    SelectTable = $('#SelectTable').val();
    selectMultipleTables1 = $('#selectMultipleTables1').val();
    selectMultipleTables1 = $('#selectMultipleTables2').val();
    $('#preloader').show();
    var tableNames = [SelectTable, selectMultipleTables1];
    GetTableDetails(DatabaseType, txtServerName, textUserName, textPassword, textDatabase, tableNames, function (data) {
        /* tablecolumnData = data.data;*/
        var tablecolumnData = [];
        for (var i = 0; i < tableDetailsArray.length; i++) {
            tablecolumnData = tablecolumnData.concat(tableDetailsArray[i].data);
        }
        $('#preloader').hide();
        if (!data.error) {
            data = data.data;
            for (i = 0; i < data.length; i++) {
                data[i]["SqlDatatype"] = SqlToDtDataTypeMaping[data[i].datatype1];
                if (data[i].columnName == "IsActive") {
                    data[i]["SqlDatatype"] = "bit";
                }
                else if (data[i].columnName.indexOf("Allow") > -1) {
                    data[i]["SqlDatatype"] = "bit";
                }
                else if (data[i].columnName.indexOf("Password") > -1 && data[i].columnName.indexOf("Is") < 0) {
                    data[i]["SqlDatatype"] = "Password";
                }
                else if (data[i].columnName.indexOf("Password") > -1 && data[i].columnName.indexOf("Is") >= 0) {
                    data[i]["SqlDatatype"] = "bit";
                }
                else if (data[i].columnName.indexOf("Phone") > -1) {
                    data[i]["SqlDatatype"] = "Phone";
                }
                else if (data[i].columnName.indexOf("Email") > -1) {
                    data[i]["SqlDatatype"] = "Email";
                }
            }
            $('#TableDetails').find('tr').remove().end();
            tblMyTable_columns = [
                { ajaxData: "columnName", title: "Column Name", data: "columnName", "sWidth": "100px", "min-width": "100px", "sClass": "text-right columnName", "mData": "columnName", datatype: "text", mRender: dataTablerender },
                { ajaxData: "datatype1", title: "Data Type", data: "SqlDatatype", "sWidth": "20px", "min-width": "20px", datatype: "datatype1", "sClass": "batcher datatype1", bVisible: true, mRender: dataTablerender },
                // { ajaxData: "max_length", title: "Max Length", data: "max_length", "sWidth": "20px", "min-width": "20px", datatype: "integer", "sClass": "batcher max_length", bVisible: false, mRender: dataTablerender },
                { ajaxData: "IsVisibale", title: "Visible", data: "isVisibale", "sWidth": "20px", "min-width": "20px", datatype: "checkbox", "sClass": "batcher IsVisibale", mRender: dataTablerender },
                { ajaxData: "allowfiltering", title: "Allow Filter", data: "allowfiltering", "sWidth": "20px", "min-width": "20px", datatype: "checkbox", "sClass": "batcher allowfiltering", mRender: dataTablerender },
                { ajaxData: "is_nullable", title: "Allow Edit", data: "is_nullable", "sWidth": "20px", "min-width": "20px", datatype: "checkbox", "sClass": "batcher is_nullable", mRender: dataTablerender },
                { ajaxData: "iseditable", title: "Allow Null", data: "iseditable", "sWidth": "20px", "min-width": "20px", datatype: "checkbox", "sClass": "batcher allowfiltering", mRender: dataTablerender }
            ]
            $('#TableDetails').DataTable({
                "serverSide": false,
                "searching": false,
                "scrollX": false,
                "scrollY": '400px',
                "scrollCollapse": false,
                "paging": false,
                "pagingType": "full_numbers",
                "ordering": false,
                "autoWidth": true,
                sServerMethod: 'GET',
                destroy: true,
                responsive: false,
                "info": false,
                "pageLength": 25,
                "lengthChange": true,
                "lengthMenu": [25, 50, 75, 100, 200, 500, 1000],
                stateSave: false,
                fixedColumns: true,
                fixedColumns: {
                    leftColumns: 1
                },
                dom: 'Blfrtip',
                buttons: ['colvis'],
                deferRender: true,
                columns: tblMyTable_columns,
                data: data,
                rowId: 'column_id',
                "drawCallback": function (settings) {
                    if ($('#mainheader').length == 0) {
                        $('#TableDetails_wrapper .dataTables_scrollHeadInner thead tr').before('<tr id="mainheader"><th colspan="2" class="sorting_disabled text-center" style="border-top: 1px solid #111;" ></th><th colspan="2" class="sorting_disabled text-center" style="border-top: 1px solid #111;">Display Options</th><th colspan="2" class="sorting_disabled text-center" style="border-top: 1px solid #111;">Add/Update</th></tr>')
                    }
                }
            });
        }
        else {
            $.alert({ title: 'Error!', content: data.error });
        }
        //bindSelect(data, '#SelectTable', { "Select Table": 0 }, 0);
    });
}
function download(filename, text) {
    var element = document.createElement('a');
    element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(text));
    element.setAttribute('download', filename);

    element.style.display = 'none';
    document.body.appendChild(element);
    element.click();
    document.body.removeChild(element);
}
// Replace Name using @ options
function replacetableSetting(data) {
    data = data.replaceAll("@ReplaceTableName", SelectTable);
    data = data.replaceAll("@pkColumnName", PK.columnName);
    data = data.replaceAll("@ReplaceColumns", JSON.stringify(tablecolumn).replaceAll("@mRender", "mRender").replaceAll("\"@dataTablerender\"", "dataTablerender"));
    data = data.replaceAll("@bServerSide", CodeSettings.bServerSide);
    data = data.replaceAll("@sScrollX", CodeSettings.sScrollX);
    data = data.replaceAll("@sScrollY", CodeSettings.sScrollY);
    data = data.replaceAll("@bInfo", CodeSettings.bInfo);
    data = data.replaceAll("@bSort", CodeSettings.bSort);
    data = data.replaceAll("@bPaginate", CodeSettings.bPaginate);
    data = data.replaceAll("@iDisplayLength", CodeSettings.aLengthMenu);
    return data;
}
// Auto Fill connection using Defails coonection according to DataType
$('#SelectDatabaseType').change(function (s) {
    $('#successAlert').hide();
    $('#errorAlert').hide();
    if ($('#SelectDatabaseType').val() == "1") {
        $('#txtServerName').val('DESKTOP-IONQCEG\SQLEXPRESS');
        $('#textUserName').val('ajay');
        $('#textPassword').val('Ajay@123');
        $('#PGHepler').hide();
        $('#SQlHelper').show();
    }
    else {
        $('#txtServerName').val('192.168.61.227');
        $('#textUserName').val('PMeravat');
        $('#textPassword').val('123456789!');
        $('#PGHepler').show();
        $('#SQlHelper').hide();
    }
});
// Hide show Page Length field according To select pagination
function checkPageLength() {
    if ($('#DTE_Paginate')[0].checked) {
        $("#pageLength").css("display", "block");
    } else {
        $("#pageLength").css("display", "none");
    }
}
// onmouseover function show info USP
function showUSPInfo(data) {
    $("#USPInfo").show();
}
// onmouseout function Hide info USP
function hideUSPInfo(data) {
    $("#USPInfo").hide();
}