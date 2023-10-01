using System.Data;
using VCE.DAL.Providers;
using VCE.Shared.Dictionary;
using VCE.Shared.Models;

namespace VCE.Business.Repository
{
    ///---------------------------------------------------------------------------
    /// <summary>
    /// Repository class for performing database operations. which contains table property. 
    /// </summary>
    /// <remarks>
    /// Created on :- 22-09-2023
    /// Created By :- VCE
    /// Purpose :- Sepration of table.
    /// </remarks>
    ///---------------------------------------------------------------------------
    public class LookupRepository
    {
        #region Methods
        /// <summary>
        /// Retrieves a dictionary of database types.
        /// </summary>
        /// <returns>A dictionary containing database types and their corresponding integer values.</returns>
        public virtual async Task<IDictionary<string, int>> GetDatabaseTypes()
        {  
            DatabaseType objDatabaseType = new DatabaseType();           
            return await objDatabaseType.GetDatabaseType();
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
        public virtual async Task<IDictionary<string, string>> GetDatabases(string databaseType, string dataSource, string userName, string userPassword, string database = "master")
        {
            DatabaseProvider objDatabaseProvider = new DatabaseProvider();
            return await objDatabaseProvider.GetDatabases(databaseType, dataSource, userName, userPassword, database);
        }
        /// <summary>
        /// Retrieves a dictionary of databases based on a connection string.
        /// </summary>
        /// <param name="databaseType">The type of the database.</param>
        /// <param name="connectionString">The database connection string.</param>
        /// <returns>A dictionary containing database names and their corresponding connection strings.</returns>
        public virtual async Task<IDictionary<string, string>> GetDatabases(string databaseType, string connectionString)
        {
            DatabaseProvider objDatabaseProvider = new DatabaseProvider();
            return await objDatabaseProvider.GetDatabases(databaseType, connectionString);
        }
        /// <summary>
        /// Retrieves a dictionary of table names for a given database.
        /// </summary>
        /// <param name="databaseType">The type of the database.</param>
        /// <param name="connectionString">The database connection string.</param>
        /// <returns>A dictionary containing table names and their corresponding values.</returns>
        public virtual async Task<IDictionary<string, string>> GetTableList(string databaseType, string connectionString)
        {
            TableProvider objTableProvider = new TableProvider();
            return await objTableProvider.GetTableList(databaseType, connectionString);
        }
        /// <summary>
        /// Retrieves details of a table from a database.
        /// </summary>
        /// <param name="databaseType">The type of the database.</param>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="TableName">The name of the table.</param>
        /// <returns>A DataTable containing details of the table.</returns>
        public virtual async Task<DataTable> GetTableDetails(string databaseType, string connectionString, string TableName)
        {
            TableProvider objTableProvider = new TableProvider();
            return await objTableProvider.GetTableDetails(databaseType, connectionString, TableName);
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
        /// <param name="inputJson">Search data in DataTable format.</param>
        /// <returns>A DataTable containing the requested data.</returns>
        public virtual async Task<DataTable> GetTableData(string tableName, string connectionString, string whereCondition, string orderBy, string pageSize, string pageStart, string databaseType, DataTableSearchData inputJson)
        {
            TableProvider objTableProvider = new TableProvider();
            return await objTableProvider.GetTableData(tableName, connectionString, whereCondition, orderBy, pageSize, pageStart, databaseType, inputJson);
        }
        /// <summary>
        /// Retrieves a database script based on the database type and file name.
        /// </summary>
        /// <param name="databaseType">The type of the database.</param>
        /// <param name="fileName">The name of the script file.</param>
        /// <returns>The contents of the script as a string.</returns>
        public virtual async Task<string> GetScript(string databaseType, string fileName)
        {
            TableProvider objTableProvider = new TableProvider();
            return await objTableProvider.GetScript(databaseType, fileName);
        }
        /// <summary>
        /// Retrieves the contents of a file based on the file name.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The contents of the file as a string.</returns>
        public virtual async Task<string> GetFileString(string fileName)
        {
            TableProvider objTableProvider = new TableProvider();
            return await objTableProvider.GetFileString(fileName);
        } 
        #endregion
    }
}