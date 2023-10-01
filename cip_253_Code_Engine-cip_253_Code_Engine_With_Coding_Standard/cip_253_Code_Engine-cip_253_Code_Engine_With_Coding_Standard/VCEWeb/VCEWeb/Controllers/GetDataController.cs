using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using VCE.Shared.Models;
using VCEWeb.Services;

namespace VCEWeb.Controllers
{
    ///---------------------------------------------------------------------------
    /// <summary>
    /// This class will contain the property of table.
    /// </summary>
    /// <remarks>
    /// Created on :- 22-09-2023
    /// Created By :- VCE
    /// Purpose :- Sepration of Table.
    /// </remarks>
    ///---------------------------------------------------------------------------
    [Route("api/[controller]")]
    [ApiController]
    public class GetDataController : ControllerBase
    {
        #region Methods

        ///---------------------------------------------------------------------------
        /// <summary>
        /// Endpoint to insert Data information.
        /// </summary>
        /// <param name="JsonObject">The JSON object containing Data information.</param>
        /// <returns>The RoleID generated for the inserted data.</returns>
        ///---------------------------------------------------------------------------
        [HttpPost]
        public virtual async Task<object> POST([FromBody] object JsonObject)
        {
            return Ok(JsonObject);
        }
        ///---------------------------------------------------------------------------
        /// <summary>
        /// Endpoint to retrieve filtered table data.
        /// </summary>
        /// <param name="Data">The DataTableSearchData object containing filter and sorting criteria.</param>
        /// <returns>The DataTableOutput object with filtered data.</returns>
        ///-----------------------------------------------------------------------
        [HttpPost]
        [Route("getfiltereddata")]
        public virtual async Task<object> GetTableData(DataTableSearchData Data)
        {
            LookupService objLookupService = new LookupService();
            string connectionstring = HttpContext.Session.GetString("connectionstring");
            string DatabaseType = HttpContext.Session.GetString("DatabaseType");
            var dataTableOutput = new DataTableOutput();
            string whereCondition = string.Empty;
            string orderBy = string.Empty;
            foreach (var a in Data.columns)
            {
                if (DatabaseType == "1")
                    whereCondition = whereCondition + ((!string.IsNullOrEmpty(whereCondition)) ? " or " : "") + a.data + " like '%" + a.search.value + "%'";
                else
                {
                    whereCondition = whereCondition + "\n or cast(basetable.\"" + a.data + "\"  as CHARACTER VARYING) like '%'||fltr.search||'%'";
                }
            }
            foreach (var a in Data.order)
            {
                orderBy = Data.columns[a.column].data + " " + a.dir;
            }
            var output = await objLookupService.GetTableData(Data.tableName, connectionstring, whereCondition, orderBy, Data.length.ToString(), Data.start.ToString(), DatabaseType, Data);
            dataTableOutput.data = JsonConvert.SerializeObject(output);
            return dataTableOutput;
        }
        ///---------------------------------------------------------------------------
        /// <summary>
        /// Endpoint to retrieve Scriptfile.
        /// </summary>
        /// <param name="GetScript">Get Scripts Datatable object.</param>
        ///-----------------------------------------------------------------------
        [HttpGet]
        [Route("Scriptfile")]
        public virtual async Task<object> GetScript(string DatabaseType, string FileName)
        {
            LookupService objLookupService = new LookupService();
            return await objLookupService.GetScript(DatabaseType, FileName);
        }
        ///---------------------------------------------------------------------------
        /// <summary>
        /// Endpoint to retrieve FileString.
        /// </summary>
        /// <Discription >Get GetFileString object.</Discription>
        ///-----------------------------------------------------------------------
        [HttpGet]
        [Route("FileString")]
        public virtual async Task<object> GetFileString(string FileName)
        {
            LookupService objLookupService = new LookupService();
            return await objLookupService.GetFileString(FileName);
        } 
        #endregion
    }
}