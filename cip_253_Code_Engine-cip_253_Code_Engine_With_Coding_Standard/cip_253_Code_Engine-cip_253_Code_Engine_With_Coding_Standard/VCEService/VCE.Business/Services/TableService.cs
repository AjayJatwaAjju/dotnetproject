using System.Data;
using VCE.DAL.Providers;

namespace VCE.Business.Services
{
    ///--------------------------------------------------------------------------
    /// <summary>
    /// Summary description for TableService.
    /// </summary>
    /// <remarks>
    /// Created On:- 04/05/2023
    /// Created By:- Menter
    /// Purpose:- This class will provide service for business logic for managing user-related operations.
    /// </remarks>
    ///---------------------------------------------------------------------------
    public class TableService
    {
        #region Methods

        //Creating a TableProvider object
        private TableProvider objTableProvider = new TableProvider();

        /// <summary>
        /// This will return all Table records based on applied condition.
        /// </summary>
        /// <remarks>
        /// <param name="whereCondition">Passing condition for filtering Table records.</param>
        /// <param name="orderBy">Passing OrderBy.</param>
        /// </remarks>
        public DataTable GetTableInfo(string whereCondition, string orderBy)
        {
            whereCondition = whereCondition + string.Format("Country={0} and Lang={1}", "US", "EN");
            return objTableProvider.GetTableInfo(whereCondition, orderBy);
        } 
        #endregion
    }
}