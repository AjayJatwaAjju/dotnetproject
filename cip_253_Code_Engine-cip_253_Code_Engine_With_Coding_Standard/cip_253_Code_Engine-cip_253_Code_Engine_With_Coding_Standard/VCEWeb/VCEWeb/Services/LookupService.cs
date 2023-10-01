using System.Data;
using VCE.Business.Repository;
using VCE.Shared.Models;

namespace VCEWeb.Services
{
    ///--------------------------------------------------------------------------
    /// <summary>
    /// Summary description for Service.
    /// </summary>
    /// <remarks>
    /// Created On:- 20/09/2023
    /// Created By:- VCE
    /// Purpose:- This class will provide service for Provider.
    /// </remarks>
    ///---------------------------------------------------------------------------
    public class LookupService
    {
        #region Methods

        /// <summary>
        /// Retrieves a dictionary of database types.
        /// </summary>
        /// <returns>A dictionary containing database types and their corresponding integer values.</returns>
        public virtual async Task<IDictionary<string, int>> GetDatabaseTypes()
        {
            LookupRepository lookupRepository = new LookupRepository();
            return await lookupRepository.GetDatabaseTypes();
        }
        /// <summary>
        /// Retrieves a dictionary of databases based on connection parameters.
        /// </summary>
        /// <param name="databaseType">The type of the database.</param>
        /// <param name="dataSource">The data source (e.g., server).</param>
        /// <param name="userName">The database user name.</param>
        /// <param name="userPassword">The user's password.</param>
        /// <param name="database">The name of the database (default is "master").</param>
        /// <returns>A dictionary containing database names and their corresponding connection strings.</returns>
        public virtual async Task<IDictionary<string, string>> GetDatabases(string databaseType, string dataSource, String userName, string userPassword, string database = "master")
        {
            LookupRepository lookupRepository = new LookupRepository();
            return await lookupRepository.GetDatabases(databaseType, dataSource, userName, userPassword, database);
        }
        /// <summary>
        /// Retrieves a dictionary of databases based on a connection string.
        /// </summary>
        /// <param name="databaseType">The type of the database.</param>
        /// <param name="connectionString">The database connection string.</param>
        /// <returns>A dictionary containing database names and their corresponding connection strings.</returns>
        public virtual async Task<IDictionary<string, string>> GetDatabases(string databaseType, string connectionString)
        {
            LookupRepository lookupRepository = new LookupRepository();
            return await lookupRepository.GetDatabases(databaseType, connectionString);
        }
        /// <summary>
        /// Retrieves a dictionary of table names for a given database.
        /// </summary>
        /// <param name="databaseType">The type of the database.</param>
        /// <param name="connectionString">The database connection string.</param>
        /// <returns>A dictionary containing table names and their corresponding values.</returns>
        public virtual async Task<IDictionary<string, string>> GetTableList(string databaseType, string connectionString)
        {
            LookupRepository lookupRepository = new LookupRepository();
            return await lookupRepository.GetTableList(databaseType, connectionString);
        }
        /// <summary>
        /// Retrieves details of a table from a database.
        /// </summary>
        /// <param name="databaseType">The type of the database.</param>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>A DataTable containing details of the table.</returns>
        public virtual async Task<DataTable> GetTableDetails(string databaseType, string connectionString, string tableName)
        {
            LookupRepository lookupRepository = new LookupRepository();
            return await lookupRepository.GetTableDetails(databaseType, connectionString, tableName);
        }
        /// <summary>
        /// Retrieves data from a table in a database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="whereCondition">A WHERE clause to filter data.</param>
        /// <param name="orderBy">An ORDER BY clause for sorting.</param>
        /// <param name="pageSize">The number of records per page.</param>
        /// <param name="pageStart">The starting page index.</param>
        /// <param name="databaseType">The type of the database.</param>
        /// <param name="inputjson">Search data in DataTable format.</param>
        /// <returns>A DataTable containing the requested data.</returns>
        public virtual async Task<DataTable> GetTableData(string tableName, string connectionString, string whereCondition, string orderBy, string pageSize, string pageStart, string databaseType, DataTableSearchData inputjson)
        {
            LookupRepository lookupRepository = new LookupRepository();
            return await lookupRepository.GetTableData(tableName, connectionString, whereCondition, orderBy, pageSize, pageStart, databaseType, inputjson);
        }
        /// <summary>
        /// Retrieves a database script based on the database type and file name.
        /// </summary>
        /// <param name="databaseType">The type of the database.</param>
        /// <param name="fileName">The name of the script file.</param>
        /// <returns>The contents of the script as a string.</returns>
        public virtual async Task<string> GetScript(string databaseType, string fileName)
        {
            LookupRepository lookupRepository = new LookupRepository();
            return await lookupRepository.GetScript(databaseType, fileName);
        }
        /// <summary>
        /// Retrieves the contents of a file based on the file name.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The contents of the file as a string.</returns>
        public virtual async Task<string> GetFileString(string fileName)
        {
            LookupRepository lookupRepository = new LookupRepository();
            return await lookupRepository.GetFileString(fileName);
        } 
        #endregion
    }
}