using System.Data;
using VCE.Shared.Dictionary;
using VCE.Utility;

namespace VCE.DAL.Providers
{
    ///---------------------------------------------------------------------------
    /// <summary>
    /// This class will contain the property of table.
    /// </summary>
    /// <remarks>
    /// Created on :- 22-09-2023
    /// Created By :- VCE
    /// Purpose :-provider is responsible for supplying data or services and Sepration of table.
    /// </remarks>
    ///---------------------------------------------------------------------------
    public class DatabaseProvider
    {
        #region Methods

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
            string connectionString = SqlHelper.createConnectionString(dataSource, database, userName, userPassword);
            DataSet objDataSet = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, QueryProvider.qSQL_SysDataBases);
            var result = new Dictionary<string, string>();
            result = objDataSet.Tables[0].AsEnumerable()
                                 .ToDictionary(
                                    row => row.Field<string>(0),
                                    row => row.Field<string>(0)
                                );
            return result;
        }
        /// <summary>
        /// Retrieves a dictionary of databases based on a connection string.
        /// </summary>
        /// <param name="databaseType">The type of the database.</param>
        /// <param name="connectionString">The database connection string.</param>
        /// <returns>A dictionary containing database names and their corresponding connection strings.</returns>
        public virtual async Task<IDictionary<string, string>> GetDatabases(string databaseType, string connectionString)
        {
            connectionString = Decrypt.Base64Decode(connectionString);
            DataSet objDataSet = new DataSet();
            var sqlServer = (int)DatabaseTypes.MicrosoftSQLServer; // add value from enum
            if (databaseType == sqlServer.ToString())
            {
                objDataSet = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, QueryProvider.qSQL_SysDataBases);
            }
            else
            {
                objDataSet = PostgreSQLHelper.ExecuteDataset(connectionString, CommandType.Text, QueryProvider.qPG_SysDataBases);
            }
            var result = new Dictionary<string, string>();
            if (objDataSet != null && objDataSet.Tables.Count > 0)
            {
                result = objDataSet.Tables[0].AsEnumerable()
                                     .ToDictionary(
                                        row => row.Field<string>(0),
                                        row => row.Field<string>(0)
                                    );
            }
            return result;
        } 
        #endregion
    }
}