using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using VCE.DAL.Providers;
using VCE.Shared;
using VCEWeb.Services;
using static VCE.Shared.Error;
using WebHttp = System.Web.Http;

namespace VCEWeb.Controllers
{
    ///---------------------------------------------------------------------------
    /// <summary>
    /// This class will contain the property of table.
    /// </summary>
    /// <remarks>
    /// Created on :- 22-09-2023
    /// Created By :- VCE
    /// Purpose :- Sepration of table and This controller is responsible for handling user-related actions..
    /// </remarks>
    ///---------------------------------------------------------------------------
    [Route("api/[controller]")]
    [ApiController]
    [WebHttp.Authorize]
    public class LookupController : ControllerBase
    {
        #region Methods

        /// <summary>
        /// Endpoint to retrieve filtered DataType.
        /// </summary>
        /// <param name="Data">Get DataType like SQL And PGSQL</param>
        /// <returns>Returning Valye of Datatype</returns>
        [EnableQuery]
        [Route("DatabaseTypes")]
        [WebHttp.Authorize]
        public virtual IActionResult Get()
        {
            LookupService objLookupService = new LookupService();
            var output = objLookupService.GetDatabaseTypes().Result;
            return Ok(output);
        }

        /// <summary>
        /// Endpoint to retrieve filtered DataType.
        /// </summary>
        /// <param name="Data">Get DataType like SQL And PGSQL</param>
        /// <returns>Returning Valye of Datatype</returns>
        [EnableQuery]
        [Route("DatabasesOld")]
        public virtual async Task<ActionResult> Get(string databaseType, string dataSource, String userName, string userPassword)
        {
            try
            {
                LookupService objLookupService = new LookupService();
                var output = await objLookupService.GetDatabases(databaseType, dataSource, userName, userPassword);
                OutputDetails outputDetails = new OutputDetails();
                outputDetails.isError = false;
                outputDetails.error = "";
                outputDetails.data = output;
                return Ok(outputDetails);
            }
            catch (Exception ex)
            {
                OutputDetails error = new OutputDetails();
                error.isError = true;
                error.error = ex.Message;
                return Ok(error);
            }
        }

        /// <summary>
        /// Endpoint to retrieve Getting Database.
        /// </summary>
        /// <param name="Data">Get Database and tables</param>
        /// <returns>Returning Valye of tables</returns>
        [EnableQuery]
        [Route("Databases")]
        public virtual async Task<ActionResult> GetDatabases(string databaseType)
        {
            try
            {
                string connectionString = Request.Headers[Config.GetConnectionString].ToString();

                LookupService objLookupService = new LookupService();
                var output = await objLookupService.GetDatabases(databaseType, connectionString);
                OutputDetails outputDetails = new OutputDetails();
                outputDetails.isError = false;
                outputDetails.error = "";
                outputDetails.data = output;
                return Ok(outputDetails);
            }
            catch (Exception ex)
            {
                OutputDetails error = new OutputDetails();
                error.isError = true;
                error.error = ex.Message;
                return Ok(error);
            }
        }
        /// <summary>
        /// Endpoint to retrieve Getting Table List.
        /// </summary>
        /// <param name="Data">Get Database and tablesList</param>
        /// <returns>Returning Valye of tablesList</returns>
        [EnableQuery]
        [Route("TableList")]
        public virtual async Task<ActionResult> GetTableList(string databaseType)
        {
            try
            {
                string connectionString = Request.Headers[Config.GetConnectionString].ToString();

                LookupService objLookupService = new LookupService();
                var output = await objLookupService.GetTableList(databaseType, connectionString);
                OutputDetails outputDetails = new OutputDetails();
                outputDetails.isError = false;
                outputDetails.error = "";
                outputDetails.data = output;
                return Ok(outputDetails);
            }
            catch (Exception ex)
            {
                OutputDetails error = new OutputDetails();
                error.isError = true;
                error.error = ex.Message;
                return Ok(error);
            }
        }

        /// <summary>
        /// Endpoint to retrieve Getting Table Detalis Like Type and all.
        /// </summary>
        /// <param name="Data">Get Database and tablesList</param>
        /// <returns>Returning Valye of tablesList</returns>
        /* [EnableQuery]
         [Route("TableDetail")]
         public virtual async Task<object> GetTableDetails(string databaseType, string tableName)
         {
             try
             {
                 string connectionString = Request.Headers[Config.GetConnectionString].ToString();
                 LookupService objLookupService = new LookupService();
                 var output = await objLookupService.GetTableDetails(databaseType, connectionString, tableName);

                 HttpContext.Session.SetString("connectionstring", connectionString);
                 HttpContext.Session.SetString("DatabaseType", databaseType);
                 var columneList = (from DataRow dr in output.Rows
                                    select new
                                    {
                                        column_id = dr["column_id"].ToString(),
                                        ColumnName = dr["ColumnName"].ToString(),
                                        Datatype1 = dr["Datatype1"].ToString(),
                                        max_length = dr["MaxLength"].ToString(),
                                        is_nullable = Convert.ToBoolean(dr["is_nullable"].ToString()),
                                        PrimaryKey = Convert.ToBoolean(dr["PrimaryKey"].ToString()),
                                        scale = dr["scale"].ToString(),
                                        IsVisibale = Convert.ToBoolean(dr["IsVisibale"].ToString()),
                                        allowfiltering = Convert.ToBoolean(dr["allowfiltering"].ToString()),
                                        Iseditable = Convert.ToBoolean(dr["Iseditable"].ToString()),
                                    }).ToList();

                 OutputDetails outputDetails = new OutputDetails();
                 outputDetails.isError = false;
                 outputDetails.error = "";
                 outputDetails.data = columneList;
                 return Ok(outputDetails);
             }
             catch (Exception ex)
             {
                 OutputDetails error = new OutputDetails();
                 error.isError = true;
                 error.error = ex.Message;
                 return Ok(error);
             }
         } */
        [EnableQuery]
        [Route("TableDetail")]
        public class TableDetailController : ControllerBase
        {
            private readonly IHttpContextAccessor _httpContextAccessor;

            public TableDetailController(IHttpContextAccessor httpContextAccessor)
            {
                _httpContextAccessor = httpContextAccessor;
            }

            [HttpGet]
            public async Task<IActionResult> GetTableDetails(string databaseType, List<string> tableNames)
            {
                try
                {
                    string connectionString = _httpContextAccessor.HttpContext.Request.Headers[Config.GetConnectionString].ToString();

                    TableProvider tableProvider = new TableProvider(); // Instantiate your TableProvider
                    var tableData = await tableProvider.GetMultipleTableData(databaseType, connectionString, tableNames, "", "", "", "", null);

                    HttpContext.Session.SetString("connectionstring", connectionString);
                    HttpContext.Session.SetString("DatabaseType", databaseType);

                    // Create a list to store the results for multiple tables
                    List<OutputDetails> resultDetails = new List<OutputDetails>();

                    foreach (var tableName in tableNames)
                    {
                        DataTable output = tableData[tableName];
                        var columneList = (from DataRow dr in output.Rows
                                           select new
                                           {
                                               column_id = dr["column_id"].ToString(),
                                               ColumnName = dr["ColumnName"].ToString(),
                                               Datatype1 = dr["Datatype1"].ToString(),
                                               max_length = dr["MaxLength"].ToString(),
                                               is_nullable = Convert.ToBoolean(dr["is_nullable"].ToString()),
                                               PrimaryKey = Convert.ToBoolean(dr["PrimaryKey"].ToString()),
                                               scale = dr["scale"].ToString(),
                                               IsVisibale = Convert.ToBoolean(dr["IsVisibale"].ToString()),
                                               allowfiltering = Convert.ToBoolean(dr["allowfiltering"].ToString()),
                                               Iseditable = Convert.ToBoolean(dr["Iseditable"].ToString()),
                                           }).ToList();

                        OutputDetails outputDetails = new OutputDetails();
                        outputDetails.isError = false;
                        outputDetails.error = "";
                        outputDetails.data = columneList;

                        resultDetails.Add(outputDetails);
                    }

                    return Ok(resultDetails);
                }
                catch (Exception ex)
                {
                    OutputDetails error = new OutputDetails();
                    error.isError = true;
                    error.error = ex.Message;
                    return Ok(error);
                }
            }
        }
        #endregion
    }
}