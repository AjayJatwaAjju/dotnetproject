//Creating Coonection String here replace @ parameter
let ConnectionStrings = {
    1: "Data Source=@DataSource; Database=@Database; User ID=@UserID; Connection Timeout=500; Password=@Password;",
    2: "Server=@DataSource;Port=5432;Database=@Database;User Id=@UserID;Password=\"@Password\";CommandTimeout=180"
}
// Ger DataBase type here like SQL or PGSQL
function GetDatabaseTypes(cb_func) {
    $.ajax({
        type: "Get",
        url: "api/lookup/DatabaseTypes",
        success: cb_func,
        error: function (request, error) {
            $('#preloader').show();
            alert('An error occurred attempting to get new database type');
        }
    });
}
function GetDatabasesOld(DatabaseType, DataSource, UserName, UserPassword, cb_func) {
    $.ajax({
        type: "Get",
        url: "api/lookup/Databases?DatabaseType=" + DatabaseType + "&DataSource=" + DataSource + "&UserName=" + UserName + "&UserPassword=" + UserPassword,
        success: cb_func,
        error: function (request, error) {
            $('#preloader').show();
            alert('An error occurred attempting to get databases');
        }
    });
}

function GetDatabases(DatabaseType, DataSource, UserName, UserPassword, cb_func) {
    let ConnectionString = GetConnectionString(DataSource, UserName, UserPassword, DatabaseType == "1" ? 'master' : 'postgres');
    $.ajax({
        type: "Get",
        url: "api/lookup/Databases?DatabaseType=" + DatabaseType,
        headers: { 'UTI5dWJtVmpkR2x2YmxOMGNtbHVadz09': ConnectionString },
        success: cb_func,
        error: function (request, error) {
            $('#preloader').show();
            alert('An error occurred attempting to get databases');
        }
    });
}
function GetTableList(DatabaseType, DataSource, UserName, UserPassword, textDatabase, cb_func) {
    let ConnectionString = GetConnectionString(DataSource, UserName, UserPassword, textDatabase);
    $.ajax({
        type: "Get",
        url: "api/lookup/TableList?DatabaseType=" + DatabaseType,
        headers: { 'UTI5dWJtVmpkR2x2YmxOMGNtbHVadz09': ConnectionString },
        success: cb_func,
        error: function (request, error) {
            $('#preloader').show();
            alert('An error occurred attempting to get new tablelist');
        }
    });
}
/*function GetTableDetails(DatabaseType, DataSource, UserName, UserPassword, textDatabase, TableName, cb_func) {
    let ConnectionString = GetConnectionString(DataSource, UserName, UserPassword, textDatabase);
    $.ajax({
        type: "Get",
        url: "api/lookup/TableDetail?DatabaseType=" + DatabaseType
            + "&TableName=" + TableName,
        headers: { 'UTI5dWJtVmpkR2x2YmxOMGNtbHVadz09': ConnectionString },

        // ------v-------use it as the callback function
        success: cb_func,
        error: function (request, error) {
            $('#preloader').show();
            alert('An error occurred attempting to get tabledetail');
            // console.log(request, error);
        }
    });
}*/
function GetTableDetails(DatabaseType, DataSource, UserName, UserPassword, textDatabase, TableNames, cb_func) {
    let ConnectionString = GetConnectionString(DataSource, UserName, UserPassword, textDatabase);

    // Create an array to store the results for multiple tables
    let results = [];

    // Function to fetch table details for a single table
    function fetchTableDetails(index) {
        if (index >= TableNames.length) {
            // All tables have been fetched, call the callback function with the results
            cb_func(results);
            return;
        }

        let TableName = TableNames[index];

        $.ajax({
            type: "Get",
            url: "api/lookup/TableDetail?DatabaseType=" + DatabaseType + "&TableName=" + TableName,
            headers: { 'UTI5dWJtVmpkR2x2YmxOMGNtbHVadz09': ConnectionString },
            success: function (data) {
                // Add the result to the results array
                results.push(data);

                // Fetch details for the next table
                fetchTableDetails(index + 1);
            },
            error: function (request, error) {
                $('#preloader').show();
                alert('An error occurred attempting to get tabledetail');
                // console.log(request, error);
            }
        });
    }

    // Start fetching details for the first table
    fetchTableDetails(0);
}

// Here the List to geting API list
function GetApiList() {
    return {
        "Controller": 1,
        "ASHX": 2,
        /*"ASMX": 2*/
    }
}
// get Connection string
function GetConnectionString(DataSource, UserName, UserPassword, Database) {
    let ConnectionString = ConnectionStrings[$('#SelectDatabaseType').val()];
    ConnectionString = ConnectionString.replace('@DataSource', DataSource);
    ConnectionString = ConnectionString.replace('@UserID', UserName);
    ConnectionString = ConnectionString.replace('@Password', UserPassword);
    ConnectionString = ConnectionString.replace('@Database', Database);
    ConnectionString = Base64.encode(ConnectionString);

    return ConnectionString;
}
// Here The file Path which is use to show file
var LookupServiceSettings = {
    // SQL Procedure files
    "defaultSchemaMSSQL": "dbo",
    "setOprationsMSSQL": "sqlScript\\spSetTable.sql",
    "getTableInfoMSSQL": "sqlScript\\spGetTable.sql",
    //5 USP Procedure
    "errorLogTableMSSQL": "sqlScript\\USP\\spErrorLog.sql",
    "insertIntoTableMSSQL": "sqlScript\\USP\\spInsertTableData.sql",
    "updateTableMSSQL": "sqlScript\\USP\\spUpdateTableData.sql",
    "deleteFromTableMSSQL": "sqlScript\\USP\\spDeleteTableData.sql",
    "getTableInfoByIdMSSQL": "sqlScript\\USP\\spGetTableDataByID.sql",
    "fnGetUserNameByIdMSSQL": "sqlScript\\USP\\fnGetUserNameById.sql",

    // SQL Controller, Business, Provider
    "EntityControllerSQL": "classFile\\USPClass\\SQL\\EntityControllerSQL.txt",  //Contoller
    "EntityServiceSQL": "classFile\\USPClass\\SQL\\SQLBusinessClass.txt", // Service SQL
    "EntityProviderMSSQL": "classFile\\USPClass\\SQL\\EntityProvider.txt",  // DAL SQL 2+5
    "EntityProviderMSSQL2USP": "classFile\\EntityProvider.txt",  // DAL SQL 2

    // PGSQL Procedure files
    "defaultSchemaPostgreSQL": "public",
    "setOprationsPostgreSQL": "PgSqlScript\\spSetTable.sql",
    "getTableInfoPostgreSQL": "PgSqlScript\\spGetTable.sql",

    // 5 USP Procedure
    "errorLogTablePostgreSQL": "PgSqlScript\\PGUSP\\spErrorLogPg.sql",
    "insertIntoTablePostgreSQL": "PgSqlScript\\PGUSP\\spInsertTableDataPg.sql",
    "updateTablePostgreSQL": "PgSqlScript\\PGUSP\\spUpdateTableDataPg.sql",
    "deleteFromTablePostgreSQL": "PgSqlScript\\PGUSP\\spDeleteTableDataPg.sql",
    "getTableInfoByIdPostgreSQL": "PgSqlScript\\PGUSP\\spGetTableDataByIDPg.sql",
    "fnGetUserNameByIdPostgreSQL": "PgSqlScript\\PGUSP\\fnGetUserNameByIdPg.sql",

    // PGSQL  Controller, Business, Provider :-
    "EntityControllerNew": "classFile\\USPClass\\NewEntityController.txt",      // New Controller
    "EntityServicePostgreSQL": "classFile\\USPClass\\NewBusinessService.txt", // Service PostgreSQL
    "EntityBusinessPostgreSQLUSP": "classFile\\USPClass\\EntityProviderSplit.txt",  // DAL 5 lear Provider PGSQL

    // Preview Section files
    "RequestHandler": "classFile\\RequestData.ashx.txt",
    "DataTableSearchData": "classFile\\DataTableSearchData.txt", // Model
    "EntityController": "classFile\\EntityController.txt",      //Old  Controller PGSQL
    "EntityControllerSQL2USP": "classFile\\EntityControllerSQL2USP.txt",      //Old  Controller SQL
    "EntityProviderPostgreSQL": "classFile\\EntityProviderPgSql.txt",    // DAL  2 USP PGSQL
    "htmlDatatableExample": "HtmlFiles\\DatatableExample.html",
    "javascriptPageSample": "jsFiles\\SQLPageSample.js",
    "javascriptPageSampleRshx": "jsFiles\\PageSample.js",

    //File use for PGSQL For Pagination
    "javascriptPageSampleNew": "jsFiles\\New\\PageSample.js",
    "javascriptPageSampleRshxNew": "jsFiles\\PageSample.js",
    "javascriptPageSampleSQL": "jsFiles\\New\\SQLPageSample.js",
}