using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using VCE.Business;
using VCE.Shared.Models;

namespace VCEWeb.Controllers
{
    ///---------------------------------------------------------------------------
    /// <summary>
    /// This class will contain the property of table.
    /// </summary>
    /// <remarks>
    /// Created on :- 22-09-2023
    /// Created By :- VCE
    /// Purpose :- Connects all the classes , models and controllers  defined in it.
    /// </remarks>
    ///---------------------------------------------------------------------------
    [Route("api/[controller]")]
    [ApiController]
    public class EntityController : ControllerBase
    {
        #region Methods

        private readonly EntityService _entityService;

        public EntityController(EntityService entityService)
        {
            _entityService = entityService;
        }

        /// <summary>
        /// Endpoint to retrieve filtered table data.
        /// </summary>
        /// <param name="Data">The DataTableSearchData object containing filter and sorting criteria.</param>
        /// <returns>The DataTableOutput object with filtered data.</returns>
        [HttpPost]
        [Route("getfiltereddata")]
        public virtual async Task<object> GetTableData(DataTableSearchData Data)
        {
            var dataTableOutput = new DataTableOutput();
            string whereCondition = string.Empty;
            string orderBy = string.Empty;
            foreach (var a in Data.columns)
            {
                if (!string.IsNullOrEmpty(a.search.value))
                {
                    whereCondition = whereCondition + ((!string.IsNullOrEmpty(whereCondition)) ? " or " : "") + a.data + " like '%" + a.search.value + "%'";
                }
            }
            foreach (var a in Data.order)
            {
                orderBy = Data.columns[a.column].data + " " + a.dir;
            }
            var output = _entityService.GetUserInfo(whereCondition, orderBy, Data.length.ToString(), Data.start.ToString());
            dataTableOutput.data = JsonConvert.SerializeObject(output);
            return dataTableOutput;
        }
        /// <summary>
        /// Endpoint to Get base_exportData by ID.
        /// </summary>
        /// <param name="id">The base_export used for deletion.</param>
        /// <returns>The status of the Get operation.</returns>
        [HttpGet("id")]
        public virtual async Task<object> Get(string Id)
        {
            return Ok(_entityService.GetUserInfoById(Id));
        }
        /// <summary>
        /// Endpoint to insert base_exportData information.
        /// </summary>
        /// <param name="JsonObject">The JSON object containing base_exportData information.</param>
        /// <returns>The base_export_id generated for the inserted data.</returns>
        [HttpPost]
        public virtual async Task<object> POST([FromBody] DataTable JsonObject)
        {
            return Ok(_entityService.InsertUser(JsonObject));
        }
        /// <summary>
        /// Endpoint to update base_exportData information.
        /// </summary>
        /// <param name="JsonObject">The JSON object containing base_exportData information to update.</param>
        /// <returns>The status of the updated record.</returns>
        [HttpPut]
        public virtual async Task<object> Put([FromBody] DataTable JsonObject)
        {
            return Ok(_entityService.UpdateUser(JsonObject));
        }

        /// <summary>
        /// Endpoint to delete base_exportData by ID.
        /// </summary>
        /// <param name="id">The base_export used for deletion.</param>
        /// <returns>The status of the delete operation.</returns>
        [HttpDelete("id")]
        public virtual async Task<object> Delete(string id)
        {
            return Ok(_entityService.DeleteUser(id));
        } 
        #endregion
    }
}