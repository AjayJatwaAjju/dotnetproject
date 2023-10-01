using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using VCE.Shared.Dictionary;
using VCE.Shared.Models;
using VCE.Utility;

namespace VCE.DAL.Providers
{
    ///--------------------------------------------------------------------------
    /// <summary>
    /// Summary description for TableProvider.
    /// </summary>
    /// <remarks>
    /// Created On:- 04/05/2023
    /// Created By:- Menter
    /// Purpose:- This is for performing actions in database Tables
    /// </remarks>
    ///---------------------------------------------------------------------------
    public class TableProvider
    {
        #region Methods

        /// <summary>
        /// This will return all Table records based on applied condition.
        /// </summary>
        /// <remarks>
        /// <param name="whereCondition">Passing condition for filtering Table records.</param>
        /// <param name="orderBy">Passing OrderBy.</param>
        /// </remarks>
        public DataTable GetTableInfo(string whereCondition, string orderBy)
        {
            var param = new SqlParameter[2];
            param[0] = new SqlParameter();
            param[0].ParameterName = "@whereCondition";
            param[0].Value = whereCondition;

            param[1] = new SqlParameter();
            param[1].ParameterName = "@orderBy";
            param[1].Value = orderBy;

            var dataSet = SqlHelper.ExecuteDataset(SqlHelper.GetConnectionString(), CommandType.StoredProcedure, "spGetTable", param);
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                return dataSet.Tables[0];
            }
            return null;
        }
        /// <summary>
        /// Get Data table List to select data type
        /// </summary>
        /// <param name="DatabaseType"></param>
        /// <param name="DataSource"></param>
        /// <param name="UserName"></param>
        /// <param name="UserPassword"></param>
        /// <param name="Database"></param>
        /// <returns></returns>
        public virtual async Task<IDictionary<string, string>> GetTableList(string databaseType, string connectionString)
        {
            connectionString = Decrypt.Base64Decode(connectionString);
            DataSet ds = new DataSet();
            var sqlServer = (int)DatabaseTypes.MicrosoftSQLServer; // add value from enum
            if (databaseType == sqlServer.ToString())
                ds = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, QueryProvider.qSQL_TableSchema);
            else
                ds = PostgreSQLHelper.ExecuteDataset(connectionString, CommandType.Text, QueryProvider.qPG_TableSchema);

            var result = new Dictionary<string, string>();
            result = ds.Tables[0].AsEnumerable()
                                 .ToDictionary(
                                    row => row.Field<string>(0),
                                    row => row.Field<string>(1)
                                );
            return result;
        }

        /// <summary>
        /// This will return all GetTableDetails the table is SQL or PGSQL
        /// </summary>
        /// <remarks>
        /// <param name="databaseType == getDatabase.ToString()">Passing condition for get table details.</param>
        /// </remarks>
        /*public virtual async Task<DataTable> GetTableDetails(string databaseType, string connectionString, string tableName)
        {
            connectionString = Decrypt.Base64Decode(connectionString);
            string sqlScript = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "sqlScript\\GetTableInfo.sql");
            var getDatabase = (int)DatabaseTypes.PostgreSQL; // add value from enum
            if (databaseType == getDatabase.ToString())
            {
                sqlScript = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "PgsqlScript\\GetTableInfo.sql");
                sqlScript = sqlScript.Replace("Yourtableschema", tableName.Split('.')[0]);
            }
            sqlScript = sqlScript.Replace("YourTableName", tableName.Split('.')[1]);
            DataSet objDataSet = new DataSet();
            var sqlServer = (int)DatabaseTypes.MicrosoftSQLServer; // add value from enum
            if (databaseType == sqlServer.ToString())
            {
                objDataSet = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, sqlScript);
            }
            else
            {
                objDataSet = PostgreSQLHelper.ExecuteDataset(connectionString, CommandType.Text, sqlScript);
            }
            var result = objDataSet.Tables[0];

            return result;
        }*/
        public virtual async Task<DataTable> GetTableDetails(string databaseType, string connectionString, string tableName)
        {
            connectionString = Decrypt.Base64Decode(connectionString);
            string sqlScript = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "sqlScript\\GetTableInfo.sql");
            var getDatabase = (int)DatabaseTypes.PostgreSQL; // add value from enum
            if (databaseType == getDatabase.ToString())
            {
                sqlScript = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "PgsqlScript\\GetTableInfo.sql");
                sqlScript = sqlScript.Replace("Yourtableschema", tableName.Split('.')[0]);
            }
            sqlScript = sqlScript.Replace("YourTableName", tableName.Split('.')[1]);
            DataSet objDataSet = new DataSet();
            var sqlServer = (int)DatabaseTypes.MicrosoftSQLServer; // add value from enum
            if (databaseType == sqlServer.ToString())
            {
                objDataSet = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, sqlScript);
            }
            else
            {
                objDataSet = PostgreSQLHelper.ExecuteDataset(connectionString, CommandType.Text, sqlScript);
            }
            var result = objDataSet.Tables[0];

            return result;
        }
        public async Task<Dictionary<string, DataTable>> GetMultipleTableData(string databaseType, string connectionString, List<string> tableNames, string whereCondition, string orderBy, string pageSize, string pageStart, DataTableSearchData inputJson)
        {
            connectionString = Decrypt.Base64Decode(connectionString);
            var tableData = new Dictionary<string, DataTable>();

            foreach (string tableName in tableNames)
            {
                DataTable result = await GetTableData(databaseType, connectionString, tableName, whereCondition, orderBy, pageSize, pageStart, inputJson);
                tableData.Add(tableName, result);
            }

            return tableData;
        }
        /// <summary>
        /// This will return all Roles records based on applied condition.
        /// </summary>
        /// <remarks>
        /// <param name="@ReplaceTableName">Replace data from script using like @ReplaceOrderBy.</param>
        /// </remarks>
        public virtual async Task<DataTable> GetTableData(string tableName, string connectionString, string whereCondition, string orderBy, string pageSize, string pageStart, string databaseType, DataTableSearchData inputJson)
        {
            try
            {
                var result = new DataTable();
                connectionString = Decrypt.Base64Decode(connectionString);
                if (databaseType == "1")
                {
                    string sqlScript = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "sqlScript\\GetTableData.sql");
                    sqlScript = sqlScript.Replace("@ReplaceOrderBy", orderBy);
                    sqlScript = sqlScript.Replace("@ReplacePageStart", pageStart);
                    sqlScript = sqlScript.Replace("@ReplacePageSize", pageSize);
                    sqlScript = sqlScript.Replace("@ReplaceTableName", tableName);
                    SqlParameter[] parameter = new SqlParameter[1];
                    parameter[0] = new SqlParameter("@whereCondition", SqlDbType.VarChar, 50000);
                    parameter[0].Value = whereCondition;
                    DataSet objDataSet = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, sqlScript, parameter);
                    result = objDataSet.Tables[0];
                }
                else if (databaseType == "2")
                {
                    string sqlScript = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "pgsqlScript\\GetTableData.sql");
                    sqlScript = sqlScript.Replace("@replaceWhereConditions", whereCondition);
                    sqlScript = sqlScript.Replace("@pkid", inputJson.pkid);
                    sqlScript = sqlScript.Replace("@replaceinputJson", "[" + JsonConvert.SerializeObject(inputJson) + "]");
                    sqlScript = sqlScript.Replace("@ReplaceTableName", tableName);
                    DataSet objDataSet = PostgreSQLHelper.ExecuteDataset(connectionString, CommandType.Text, sqlScript);
                    result = objDataSet.Tables[0];
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        /// <summary>
        /// This will return all convert scripts file.
        /// </summary>
        /// <remarks>
        /// <param name="sqlScript">returning table.</param>
        /// </remarks>
        public virtual async Task<string> GetScript(string databaseType, string fileName)
        {
            return await File.ReadAllTextAsync(AppDomain.CurrentDomain.BaseDirectory + (databaseType == DatabaseTypes.MicrosoftSQLServer.ToString() ? "sqlScript\\" : "PgSqlScript\\") + fileName);
        }
        /// <summary>
        /// This will return all convert scripts file.
        /// </summary>
        /// <remarks>
        /// <param name="sqlScript">returning table.</param>
        /// </remarks>
        public virtual async Task<string> GetFileString(string fileName)
        {
            return await File.ReadAllTextAsync(AppDomain.CurrentDomain.BaseDirectory + fileName);
        }

        #endregion Methods
    }
}