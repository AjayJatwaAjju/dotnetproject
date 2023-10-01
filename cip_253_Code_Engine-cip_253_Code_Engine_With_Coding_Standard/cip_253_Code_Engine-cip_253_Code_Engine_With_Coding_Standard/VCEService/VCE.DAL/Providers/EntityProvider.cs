using System.Data;
using System.Data.SqlClient;

namespace VCE.DAL
{
    ///---------------------------------------------------------------------------
    /// <summary>
    /// This class will contain the property of  table.
    /// </summary>
    /// <remarks>
    /// Created on :- 22-09-2023
    /// Created By :- VCE
    /// Purpose :- Provider is responsible for supplying data or services and Sepration of table.
    /// </remarks>
    ///---------------------------------------------------------------------------
    public class EntityProvider
    {
        #region Methods

        /// <summary>
        /// Inserting User information.
        /// </summary>
        /// <remarks>
        /// <param name="oUserInfo">Passing all User info."</param>
        /// <returns>returning UserId which is generated.</returns>
        /// </remarks>
        public int Insert(DataTable oUserInfo)
        {
            int Result = 0;
            if (oUserInfo != null)
            {
                if (oUserInfo.Rows.Count > 0)
                {
                    var param1 = new SqlParameter[oUserInfo.Columns.Count + 1];

                    for (int i = 0; i < oUserInfo.Columns.Count; i++)
                    {
                        var column = oUserInfo.Columns[i];
                        param1[i] = new SqlParameter();
                        param1[i].ParameterName = "@" + column.ColumnName;
                        param1[i].Value = oUserInfo.Rows[0][column.ColumnName];
                    }
                    param1[oUserInfo.Columns.Count] = new SqlParameter();
                    param1[oUserInfo.Columns.Count].ParameterName = "@Success";
                    param1[oUserInfo.Columns.Count].Direction = ParameterDirection.Output;
                    param1[oUserInfo.Columns.Count].Value = 0;

                    var conn = new SqlConnection(SqlHelper.GetConnectionString());
                    SqlTransaction sqlTrans = null;
                    try
                    {
                        conn.Open();
                        sqlTrans = conn.BeginTransaction();

                        SqlHelper.ExecuteScalar(sqlTrans, CommandType.StoredProcedure, "spInsertUser", param1);
                        //Commit transaction for saving info
                        sqlTrans.Commit();

                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                        Result = Convert.ToInt32(param1[oUserInfo.Columns.Count].Value);
                    }
                    catch (SqlException sqlEx)  //Error...
                    {
                        if (sqlTrans != null)
                            if (sqlTrans.Connection.State == ConnectionState.Open)
                            {
                                //rollback transaction
                                sqlTrans.Rollback();
                                conn.Close();
                            }
                        throw sqlEx;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            //closing connection
                            conn.Close();
                        }
                    }
                }
            }
            return Result;
        }

        /// <summary>
        /// Updating  User information.
        /// </summary>
        /// <remarks>
        /// <param name="oUserInfo">Passing all User info.</param>
        /// <returns>returning updated record status. </returns>
        /// </remarks>
        public int Update(DataTable oUserInfo)
        {
            int Result = 0;
            if (oUserInfo != null)
            {
                if (oUserInfo.Rows.Count > 0)
                {
                    var param1 = new SqlParameter[oUserInfo.Columns.Count + 1];

                    for (int i = 0; i < oUserInfo.Columns.Count; i++)
                    {
                        var column = oUserInfo.Columns[i];
                        param1[i] = new SqlParameter();
                        param1[i].ParameterName = "@" + column.ColumnName;
                        param1[i].Value = oUserInfo.Rows[0][column.ColumnName];
                    }
                    param1[oUserInfo.Columns.Count] = new SqlParameter();
                    param1[oUserInfo.Columns.Count].ParameterName = "@Success";
                    param1[oUserInfo.Columns.Count].Direction = ParameterDirection.Output;
                    param1[oUserInfo.Columns.Count].Value = 0;

                    var conn = new SqlConnection(SqlHelper.GetConnectionString());
                    SqlTransaction sqlTrans = null;
                    try
                    {
                        conn.Open();
                        sqlTrans = conn.BeginTransaction();

                        SqlHelper.ExecuteScalar(sqlTrans, CommandType.StoredProcedure, "spUpdateUser", param1);
                        //Commit transaction for saving info
                        sqlTrans.Commit();

                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                        Result = Convert.ToInt32(param1[oUserInfo.Columns.Count].Value);
                    }
                    catch (SqlException sqlEx)  //Error...
                    {
                        if (sqlTrans != null)
                            if (sqlTrans.Connection.State == ConnectionState.Open)
                            {
                                //rollback transaction
                                sqlTrans.Rollback();
                                conn.Close();
                            }
                        throw sqlEx;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            //closing connection
                            conn.Close();
                        }
                    }
                }
            }
            return Result;
        }

        /// <summary>
        /// This will delete User table record based on  UserId.
        /// </summary>
        /// <remarks/>
        /// <param name="oUserid">Passing deleteId.</param>
        /// <returns>returning delete status.</returns>
        /// <remarks/>
        public bool Delete(string ID)
        {
            var conn = new SqlConnection(SqlHelper.GetConnectionString());
            SqlTransaction sqlTrans = null;
            var param = new SqlParameter[2];
            try
            {
                conn.Open();
                sqlTrans = conn.BeginTransaction();
                /*Add the Parameters*/
                param[0].ParameterName = "@pkUserid";
                param[0].Value = ID;

                param[1].ParameterName = "@Success";
                param[1].Direction = ParameterDirection.Output;
                param[1].Value = 0;

                SqlHelper.ExecuteScalar(sqlTrans, CommandType.StoredProcedure, "spDeleteUser", param);
                //Commit transaction for saving info
                sqlTrans.Commit();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            catch //Error...
            {
                if (sqlTrans != null)
                    if (sqlTrans.Connection.State == ConnectionState.Open)
                    {
                        //rollback transaction
                        sqlTrans.Rollback();
                        conn.Close();
                    }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    //closing connection
                    conn.Close();
                }
            }
            return Convert.ToBoolean(param[1].Value);
        }

        /// <summary>
        /// This will return all User records based on applied condition.
        /// </summary>
        /// <remarks>
        /// <param name="whereCondition">Passing condition for filtering User records.</param>
        /// <param name="orderBy">Passing OrderBy.</param>
        /// </remarks>
        public DataTable GetUserInfo(string whereCondition, string orderBy, string PageStart, string PageSize)
        {
            var param = new SqlParameter[4];

            param[0].ParameterName = "@whereCondition";
            param[0].Value = whereCondition;

            param[1].ParameterName = "@orderBy";
            param[1].Value = orderBy;

            param[2].ParameterName = "@PageStart";
            param[2].Value = PageStart;

            param[3].ParameterName = "@PageSize";
            param[3].Value = PageSize;

            var dataSet = SqlHelper.ExecuteDataset(SqlHelper.GetConnectionString(), CommandType.StoredProcedure, "spGetAppss", param);
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                return dataSet.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// Getting User info based on Userid.
        /// </summary>
        /// <remarks>
        /// <param name="userId">Passing OrderBy.</param>
        /// </remarks>
        public DataTable GetUserInfoById(string userId)
        {
            var param = new SqlParameter[1];
            param[0].ParameterName = "@pkUserid";
            param[0].Value = userId;
            var dataSet = SqlHelper.ExecuteDataset(SqlHelper.GetConnectionString(), CommandType.StoredProcedure, "spGetUserById", param);
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    return dataSet.Tables[0];
                }
            }
            return null;
        }

        #endregion Methods
    }
}