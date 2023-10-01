using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace VCE.DAL
{
    ///--------------------------------------------------------------------------
    /// <summary>
    /// Summary description for UserProvider.
    /// </summary>
    /// <remarks>
    /// Created On:- 29/05/2023
    /// Created By:- VCE
    /// Purpose:- This is for performing actions in Users table
    /// </remarks>
    ///---------------------------------------------------------------------------
    public class UserProvider
    {
        #region Methods

        /// <summary>
        /// Inserting User information.
        /// </summary>
        /// <remarks>
        /// <param name="oUserInfo">Passing all User info."</param>
        /// <returns>returning UserId which is generated.</returns>
        /// </remarks>
        public int Insert(UserInfo oUserInfo)
        {
            var conn = new SqlConnection(SqlHelper.GetConnectionString());
            SqlTransaction sqlTrans = null;
            var param = new SqlParameter[34];
            try
            {
                conn.Open();
                sqlTrans = conn.BeginTransaction();
                /*Add the Parameters*/
                param[0] = new SqlParameter();
                param[0].ParameterName = "@FirstName";
                param[0].Value = oUserInfo.FirstName;

                param[1] = new SqlParameter();
                param[1].ParameterName = "@LastName";
                param[1].Value = oUserInfo.LastName;

                param[2] = new SqlParameter();
                param[2].ParameterName = "@EmailId";
                param[2].Value = oUserInfo.EmailId;

                param[3] = new SqlParameter();
                param[3].ParameterName = "@Password";
                param[3].Value = oUserInfo.Password;

                param[4] = new SqlParameter();
                param[4].ParameterName = "@IsEmailConfirmed";
                param[4].Value = oUserInfo.IsEmailConfirmed;

                param[5] = new SqlParameter();
                param[5].ParameterName = "@Gender";
                param[5].Value = oUserInfo.Gender;

                param[6] = new SqlParameter();
                param[6].ParameterName = "@BirthDate";
                param[6].Value = DBNull.Value;

                param[7] = new SqlParameter();
                param[7].ParameterName = "@Address";
                param[7].Value = oUserInfo.Address;

                param[8] = new SqlParameter();
                param[8].ParameterName = "@DescribeYourSelf";
                param[8].Value = oUserInfo.DescribeYourSelf;

                param[9] = new SqlParameter();
                param[9].ParameterName = "@School";
                param[9].Value = oUserInfo.School;

                param[10] = new SqlParameter();
                param[10].ParameterName = "@Profession";
                param[10].Value = oUserInfo.Profession;

                param[11] = new SqlParameter();
                param[11].ParameterName = "@fkTimeZoneId";
                param[11].Value = oUserInfo.FkTimeZoneId;

                param[12] = new SqlParameter();
                param[12].ParameterName = "@IsFaceBookConnect";
                param[12].Value = oUserInfo.IsFaceBookConnect;

                param[13] = new SqlParameter();
                param[13].ParameterName = "@IsTwitterConnect";
                param[13].Value = oUserInfo.IsTwitterConnect;

                param[14] = new SqlParameter();
                param[14].ParameterName = "@IsLinkedInConnect";
                param[14].Value = oUserInfo.IsLinkedInConnect;

                param[15] = new SqlParameter();
                param[15].ParameterName = "@LastLoginDate";
                param[15].Value = DBNull.Value;

                param[16] = new SqlParameter();
                param[16].ParameterName = "@CreatedBy";
                param[16].Value = oUserInfo.CreatedBy;

                param[17] = new SqlParameter();
                param[17].ParameterName = "@fkStatusId";
                param[17].Value = oUserInfo.FkStatusId;

                param[18] = new SqlParameter();
                param[18].ParameterName = "@IPAddress";
                param[18].Value = oUserInfo.IpAddress;

                param[19] = new SqlParameter();
                param[19].ParameterName = "@BrowserType";
                param[19].Value = oUserInfo.BrowserType;

                param[20] = new SqlParameter();
                param[20].ParameterName = "@UserIsHost";
                param[20].Value = oUserInfo.UserIsHost;

                param[21] = new SqlParameter();
                param[21].ParameterName = "@RemainingCredits";
                param[21].Value = oUserInfo.RemainingCredits;

                param[22] = new SqlParameter();
                param[22].ParameterName = "@UserVerifiedDate";
                param[22].Value = DBNull.Value;

                param[23] = new SqlParameter();
                param[23].ParameterName = "@Skype";
                param[23].Value = oUserInfo.Skype;

                param[24] = new SqlParameter();
                param[24].ParameterName = "@PostalCode";
                param[24].Value = oUserInfo.PostalCode;

                param[25] = new SqlParameter();
                param[25].ParameterName = "@City";
                param[25].Value = oUserInfo.City;

                param[26] = new SqlParameter();
                param[26].ParameterName = "@SalesCommissionFromUser";
                param[26].Value = oUserInfo.SalesCommissionFromUser;

                param[27] = new SqlParameter();
                param[27].ParameterName = "@SalesCommissionFromXYZ";
                param[27].Value = oUserInfo.SalesCommissionFromXYZ;

                param[28] = new SqlParameter();
                param[28].ParameterName = "@IsSalesUser";
                param[28].Value = oUserInfo.IsSalesUser;

                param[29] = new SqlParameter();
                param[29].ParameterName = "@fkSourceId";
                param[29].Value = oUserInfo.FkSourceId;

                param[30] = new SqlParameter();
                param[30].ParameterName = "@fkStateId";
                param[30].Value = oUserInfo.FkStateId;

                param[31] = new SqlParameter();
                param[31].ParameterName = "@SalesCommissionRate";
                param[31].Value = oUserInfo.SalesCommissionRate;

                param[32] = new SqlParameter();
                param[32].ParameterName = "@Success";
                param[32].Direction = ParameterDirection.Output;
                param[32].Value = 0;

                param[33] = new SqlParameter();
                param[33].ParameterName = "@IsNewsAboutGFH";
                param[33].Value = oUserInfo.IsNewsAboutGFH;

                SqlHelper.ExecuteScalar(sqlTrans, CommandType.StoredProcedure, "spInsertUser", param);
                //Commit transaction for saving info
                sqlTrans.Commit();

                if (conn.State == ConnectionState.Open)
                    conn.Close();
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
            return Convert.ToInt32(param[32].Value);
        }

        /// <summary>
        /// Updating  User information.
        /// </summary>
        /// <remarks>
        /// <param name="oUserInfo">Passing all User info.</param>
        /// <returns>returning updated record status. </returns>
        /// </remarks>
        public bool Update(UserInfo oUserInfo)
        {
            var conn = new SqlConnection(SqlHelper.GetConnectionString());
            SqlTransaction sqlTrans = null;
            var param = new SqlParameter[45];
            try
            {
                conn.Open();
                sqlTrans = conn.BeginTransaction();
                /*Add the Parameters*/
                param[0] = new SqlParameter();
                param[0].ParameterName = "@pkUserId";
                param[0].Value = oUserInfo.PkUserId;

                param[1] = new SqlParameter();
                param[1].ParameterName = "@FirstName";
                param[1].Value = oUserInfo.FirstName;

                param[2] = new SqlParameter();
                param[2].ParameterName = "@LastName";
                param[2].Value = oUserInfo.LastName;

                param[3] = new SqlParameter();
                param[3].ParameterName = "@EmailId";
                param[3].Value = oUserInfo.EmailId;

                param[4] = new SqlParameter();
                param[4].ParameterName = "@Password";
                param[4].Value = oUserInfo.Password;

                param[5] = new SqlParameter();
                param[5].ParameterName = "@IsEmailConfirmed";
                param[5].Value = oUserInfo.IsEmailConfirmed;

                param[6] = new SqlParameter();
                param[6].ParameterName = "@Gender";
                param[6].Value = oUserInfo.Gender;

                param[7] = new SqlParameter();
                param[7].ParameterName = "@BirthDate";
                if (oUserInfo.BirthDate.Year == 1)
                    param[7].Value = DBNull.Value;
                else
                    param[7].Value = oUserInfo.BirthDate;

                param[8] = new SqlParameter();
                param[8].ParameterName = "@fkStateId";
                if (oUserInfo.FkStateId == 0)
                {
                    param[8].Value = DBNull.Value;
                }
                else
                {
                    param[8].Value = oUserInfo.FkStateId;
                }

                param[9] = new SqlParameter();
                param[9].ParameterName = "@Address";
                param[9].Value = oUserInfo.Address;

                param[10] = new SqlParameter();
                param[10].ParameterName = "@DescribeYourSelf";
                param[10].Value = oUserInfo.DescribeYourSelf;

                param[11] = new SqlParameter();
                param[11].ParameterName = "@School";
                param[11].Value = oUserInfo.School;

                param[12] = new SqlParameter();
                param[12].ParameterName = "@Profession";
                param[12].Value = oUserInfo.Profession;

                param[13] = new SqlParameter();
                param[13].ParameterName = "@fkTimeZoneId";
                param[13].Value = oUserInfo.FkTimeZoneId;

                param[14] = new SqlParameter();
                param[14].ParameterName = "@IsFaceBookConnect";
                param[14].Value = oUserInfo.IsFaceBookConnect;

                param[15] = new SqlParameter();
                param[15].ParameterName = "@IsTwitterConnect";
                param[15].Value = oUserInfo.IsTwitterConnect;

                param[16] = new SqlParameter();
                param[16].ParameterName = "@IsLinkedInConnect";
                param[16].Value = oUserInfo.IsLinkedInConnect;

                param[17] = new SqlParameter();
                param[17].ParameterName = "@LastLoginDate";
                if (oUserInfo.LastLoginDate.Year != 1)
                {
                    param[17].Value = oUserInfo.LastLoginDate;
                }
                else
                {
                    param[17].Value = DBNull.Value;
                }

                param[18] = new SqlParameter();
                param[18].ParameterName = "@ModifiedBy";
                param[18].Value = oUserInfo.ModifiedBy;

                param[19] = new SqlParameter();
                param[19].ParameterName = "@CreatedDate";
                if (oUserInfo.CreatedDate.Year == 1)
                    param[19].Value = DBNull.Value;
                else
                    param[19].Value = oUserInfo.CreatedDate;

                param[20] = new SqlParameter();
                param[20].ParameterName = "@ModifyDate";

                if (oUserInfo.ModifyDate.Year == 1)
                    param[20].Value = DBNull.Value;
                else
                    param[20].Value = oUserInfo.ModifyDate;

                param[21] = new SqlParameter();
                param[21].ParameterName = "@DeletedBy";
                param[21].Value = oUserInfo.DeletedBy;

                param[22] = new SqlParameter();
                param[22].ParameterName = "@DeletedDate";
                if (oUserInfo.DeletedDate.Year == 1)
                    param[22].Value = DBNull.Value;
                else
                    param[22].Value = oUserInfo.DeletedDate;

                param[23] = new SqlParameter();
                param[23].ParameterName = "@fkStatusId";
                param[23].Value = oUserInfo.FkStatusId;

                param[24] = new SqlParameter();
                param[24].ParameterName = "@IPAddress";
                param[24].Value = oUserInfo.IpAddress;

                param[25] = new SqlParameter();
                param[25].ParameterName = "@BrowserType";
                param[25].Value = oUserInfo.BrowserType;

                param[26] = new SqlParameter();
                param[26].ParameterName = "@UserIsHost";
                param[26].Value = oUserInfo.UserIsHost;

                param[27] = new SqlParameter();
                param[27].ParameterName = "@RemainingCredits";
                param[27].Value = oUserInfo.RemainingCredits;

                param[28] = new SqlParameter();
                param[28].ParameterName = "@UserFacebookId";
                param[28].Value = oUserInfo.UserFacebookId;

                param[29] = new SqlParameter();
                param[29].ParameterName = "@UserVerifiedDate";
                if (oUserInfo.UserVerifiedDate.Year == 1)
                    param[29].Value = DBNull.Value;
                else
                    param[29].Value = oUserInfo.UserVerifiedDate;

                param[30] = new SqlParameter();
                param[30].ParameterName = "@Skype";
                param[30].Value = oUserInfo.Skype;

                param[31] = new SqlParameter();
                param[31].ParameterName = "@PostalCode";
                param[31].Value = oUserInfo.PostalCode;

                param[32] = new SqlParameter();
                param[32].ParameterName = "@City";
                param[32].Value = oUserInfo.City;

                param[33] = new SqlParameter();
                param[33].ParameterName = "@SalesCommissionFromUser";
                param[33].Value = oUserInfo.SalesCommissionFromUser;

                param[34] = new SqlParameter();
                param[34].ParameterName = "@SalesCommissionFromXYZ";
                param[34].Value = oUserInfo.SalesCommissionFromXYZ;

                param[35] = new SqlParameter();
                param[35].ParameterName = "@IsSalesUser";
                param[35].Value = oUserInfo.IsSalesUser;

                param[36] = new SqlParameter();
                param[36].ParameterName = "@fkSourceId";
                param[36].Value = oUserInfo.FkSourceId;

                param[37] = new SqlParameter();
                param[37].ParameterName = "@UserGUID";
                param[37].Value = oUserInfo.UserGuid;

                param[38] = new SqlParameter();
                param[38].ParameterName = "@SalesCommissionRate";
                param[38].Value = oUserInfo.SalesCommissionRate;

                param[39] = new SqlParameter();
                param[39].ParameterName = "@Affinity";
                param[39].Value = oUserInfo.Affinity;

                param[40] = new SqlParameter();
                param[40].ParameterName = "@BusinessAddress";
                param[40].Value = oUserInfo.BusinessAddress;

                param[41] = new SqlParameter();
                param[41].ParameterName = "@Success";
                param[41].Direction = ParameterDirection.Output;
                param[41].Value = 0;

                param[42] = new SqlParameter();
                param[42].ParameterName = "@OtherKnownLanguages";
                param[42].Value = oUserInfo.OtherKnownLanguages;

                param[43] = new SqlParameter();
                param[43].ParameterName = "@fkCummnicationLanguageId";
                param[43].Value = oUserInfo.fkCummnicationLanguageId;

                param[44] = new SqlParameter();
                param[44].ParameterName = "@UserMembershipType";
                param[44].Value = oUserInfo.UserMembershipType;

                SqlHelper.ExecuteScalar(sqlTrans, CommandType.StoredProcedure, "spUpdateUser", param);
                //Commit transaction for saving info
                sqlTrans.Commit();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            catch (SqlException sqlEx)
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
            return Convert.ToBoolean(param[41].Value);
        }

        /// <summary>
        /// This will delete User table record based on  UserId.
        /// </summary>
        /// <remarks/>
        /// <param name="oUserid">Passing deleteId.</param>
        /// <returns>returning delete status.</returns>
        /// <remarks/>
        public bool Delete(int oUserid)
        {
            var conn = new SqlConnection(SqlHelper.GetConnectionString());
            SqlTransaction sqlTrans = null;
            var param = new SqlParameter[2];
            try
            {
                conn.Open();
                sqlTrans = conn.BeginTransaction();
                /*Add the Parameters*/
                param[0] = new SqlParameter();
                param[0].ParameterName = "@pkUserId";
                param[0].Value = oUserid;

                param[1] = new SqlParameter();
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
        public DataTable GetUserInfo(string whereCondition, string orderBy)
        {
            var param = new SqlParameter[2];
            param[0] = new SqlParameter();
            param[0].ParameterName = "@whereCondition";
            param[0].Value = whereCondition;

            param[1] = new SqlParameter();
            param[1].ParameterName = "@orderBy";
            param[1].Value = orderBy;

            var ds = SqlHelper.ExecuteDataset(SqlHelper.GetConnectionString(), CommandType.StoredProcedure, "spGetUser", param);
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// Getting User info based on Userid.
        /// </summary>
        /// <remarks>
        /// <param name="userId">Passing OrderBy.</param>
        /// </remarks>
        public UserInfo GetUserInfoById(int userId)
        {
            var param = new SqlParameter[1];
            param[0] = new SqlParameter();
            param[0].ParameterName = "@pkUserid";
            param[0].Value = userId;

            var ds = SqlHelper.ExecuteDataset(SqlHelper.GetConnectionString(), CommandType.StoredProcedure, "spGetUserById", param);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var oUserInfo = new UserInfo();
                    return GetUserInfoByDataTable(ds.Tables[0]);
                }
            }
            return null;
        }

        /// <summary>
        /// Getting userinfo from datatable
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private UserInfo GetUserInfoByDataTable(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                var oUserInfo = new UserInfo();

                foreach (var dr in dt.AsEnumerable().ToList())
                {
                    oUserInfo.PkUserId = Convert.ToInt32(dr["pkUserId"]);
                    oUserInfo.FirstName = dr["FirstName"].ToString();
                    oUserInfo.LastName = dr["LastName"].ToString();
                    oUserInfo.EmailId = dr["EmailId"].ToString();
                    oUserInfo.Password = dr["Password"].ToString();
                    oUserInfo.IsEmailConfirmed = Convert.ToBoolean(dr["IsEmailConfirmed"]);
                    oUserInfo.FkStateId = 0;
                    oUserInfo.FkCountryId = 0;
                    if (dr["fkStateId"] != DBNull.Value)
                    {
                        oUserInfo.FkStateId = Convert.ToInt32(dr["fkStateId"]);
                    }
                    if (dr["fkCountryId"] != DBNull.Value)
                    {
                        oUserInfo.FkCountryId = Convert.ToInt32(dr["fkCountryId"]);
                    }
                    oUserInfo.Gender = dr["Gender"].ToString();
                    if (dr["BirthDate"] != DBNull.Value)
                    {
                        oUserInfo.BirthDate = Convert.ToDateTime(dr["BirthDate"]);
                    }
                    oUserInfo.Address = dr["Address"].ToString();
                    oUserInfo.DescribeYourSelf = dr["DescribeYourSelf"].ToString();
                    oUserInfo.School = dr["School"].ToString();
                    oUserInfo.Profession = dr["Profession"].ToString();
                    oUserInfo.FkTimeZoneId = Convert.ToInt32(dr["fkTimeZoneId"]);
                    oUserInfo.IsFaceBookConnect = Convert.ToBoolean(dr["IsFaceBookConnect"]);
                    oUserInfo.IsTwitterConnect = Convert.ToBoolean(dr["IsTwitterConnect"]);
                    oUserInfo.IsLinkedInConnect = Convert.ToBoolean(dr["IsLinkedInConnect"]);
                    if (dr["LastLoginDate"] != DBNull.Value)
                    {
                        oUserInfo.LastLoginDate = Convert.ToDateTime(dr["LastLoginDate"]);
                    }
                    if (dr["CreatedBy"] != DBNull.Value)
                    {
                        oUserInfo.CreatedBy = Convert.ToInt32(dr["CreatedBy"]);
                    }
                    if (dr["ModifiedBy"] != DBNull.Value)
                    {
                        oUserInfo.ModifiedBy = Convert.ToInt32(dr["ModifiedBy"]);
                    }
                    if (dr["CreatedDate"] != DBNull.Value)
                    {
                        oUserInfo.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);
                    }
                    if (dr["ModifyDate"] != DBNull.Value)
                    {
                        oUserInfo.ModifyDate = Convert.ToDateTime(dr["ModifyDate"]);
                    }
                    if (dr["DeletedBy"] != DBNull.Value)
                    {
                        oUserInfo.DeletedBy = Convert.ToInt32(dr["DeletedBy"]);
                    }
                    if (dr["DeletedDate"] != DBNull.Value)
                    {
                        oUserInfo.DeletedDate = Convert.ToDateTime(dr["DeletedDate"]);
                    }
                    if (dr["fkStatusId"] != DBNull.Value)
                    {
                        oUserInfo.FkStatusId = Convert.ToInt32(dr["fkStatusId"]);
                    }
                    if (dr["IPAddress"] != DBNull.Value)
                    {
                        oUserInfo.IpAddress = dr["IPAddress"].ToString();
                    }
                    if (dr["BrowserType"] != DBNull.Value)
                    {
                        oUserInfo.BrowserType = dr["BrowserType"].ToString();
                    }
                    if (dr["UserIsHost"] != DBNull.Value)
                    {
                        oUserInfo.UserIsHost = Convert.ToBoolean(dr["UserIsHost"]);
                    }
                    if (dr["RemainingCredits"] != DBNull.Value)
                    {
                        oUserInfo.RemainingCredits = Convert.ToInt32(dr["RemainingCredits"]);
                    }
                    if (dr["UserVerifiedDate"] != DBNull.Value)
                    {
                        oUserInfo.UserVerifiedDate = Convert.ToDateTime(dr["UserVerifiedDate"]);
                    }
                    if (dr["Skype"] != DBNull.Value)
                    {
                        oUserInfo.Skype = dr["Skype"].ToString();
                    }
                    if (dr["PostalCode"] != DBNull.Value)
                    {
                        oUserInfo.PostalCode = dr["PostalCode"].ToString();
                    }
                    if (dr["City"] != DBNull.Value)
                    {
                        oUserInfo.City = dr["City"].ToString();
                    }
                    if (dr["SalesCommissionFromUser"] != DBNull.Value)
                    {
                        oUserInfo.SalesCommissionFromUser = Convert.ToDouble(dr["SalesCommissionFromUser"]);
                    }
                    if (dr["SalesCommissionFromXYZ"] != DBNull.Value)
                    {
                        oUserInfo.SalesCommissionFromXYZ = Convert.ToDouble(dr["SalesCommissionFromXYZ"]);
                    }
                    if (dr["IsSalesUser"] != DBNull.Value)
                    {
                        oUserInfo.IsSalesUser = Convert.ToBoolean(dr["IsSalesUser"]);
                    }
                    if (dr["fkSourceId"] != DBNull.Value)
                    {
                        oUserInfo.FkSourceId = Convert.ToInt32(dr["fkSourceId"]);
                    }
                    if (dr["fkCountryId"] != DBNull.Value)
                    {
                        oUserInfo.FkCountryId = Convert.ToInt32(dr["fkCountryId"]);
                    }
                    if (dr["UserGUID"] != DBNull.Value)
                    {
                        oUserInfo.UserGuid = dr["UserGUID"].ToString();
                    }
                    if (dr["ListingCounts"] != DBNull.Value)
                    {
                        oUserInfo.ListingCounts = Convert.ToInt32(dr["ListingCounts"]);
                    }
                    if (dr["ReviewCounts"] != DBNull.Value)
                    {
                        oUserInfo.ReviewCounts = Convert.ToInt32(dr["ReviewCounts"]);
                    }
                    if (dr["PhotoIds"] != DBNull.Value)
                    {
                        oUserInfo.PhotoIds = dr["PhotoIds"].ToString();
                    }
                    if (dr["PhoneVerified"] != DBNull.Value)
                    {
                        oUserInfo.PhoneVerified = Convert.ToBoolean(Convert.ToInt32(dr["PhoneVerified"]));
                    }
                    if (dr["StateName"] != DBNull.Value)
                    {
                        oUserInfo.State = dr["StateName"].ToString();
                    }
                    if (dr["country"] != DBNull.Value)
                    {
                        oUserInfo.Country = dr["country"].ToString();
                    }
                    if (dr["ResponseRate"] != DBNull.Value)
                    {
                        oUserInfo.ResponseRate = Convert.ToDecimal(dr["ResponseRate"]);
                    }
                    if (dr["SalesCommissionRate"] != DBNull.Value)
                    {
                        oUserInfo.SalesCommissionRate = Convert.ToDouble(dr["SalesCommissionRate"]);
                    }
                    if (dr["UserFacebookId"] != DBNull.Value)
                    {
                        oUserInfo.UserFacebookId = dr["UserFacebookId"].ToString();
                    }
                    if (dr["NoOfTimePromotionalPageViewed"] != DBNull.Value)
                    {
                        oUserInfo.NoOfTimePromotionalPageViewed = Convert.ToInt32(dr["NoOfTimePromotionalPageViewed"]);
                    }
                    if (dr["Affinity"] != DBNull.Value)
                    {
                        oUserInfo.Affinity = Convert.ToString(dr["Affinity"]);
                    }
                    if (dr["BusinessAddress"] != DBNull.Value)
                    {
                        oUserInfo.BusinessAddress = Convert.ToString(dr["BusinessAddress"]);
                    }
                    if (dr["OtherKnownLanguages"] != DBNull.Value)
                    {
                        oUserInfo.OtherKnownLanguages = Convert.ToString(dr["OtherKnownLanguages"]);
                    }
                    if (dr["fkCummnicationLanguageId"] != DBNull.Value)
                    {
                        oUserInfo.fkCummnicationLanguageId = Convert.ToInt32(dr["fkCummnicationLanguageId"]);
                    }
                    if (dr["UserMembershipType"] != DBNull.Value)
                    {
                        oUserInfo.UserMembershipType = Convert.ToInt32(dr["UserMembershipType"]);
                    }

                    break;
                }
                return oUserInfo;
            }
            return null;
        }

        #endregion Methods
    }

    ///--------------------------------------------------------------------------
    /// <summary>
    /// Summary description for UserInfo.
    /// </summary>
    /// <remarks>
    /// Created On:- 29/05/2023
    /// Created By:- VCE
    /// Purpose:- This class will contain the property of User table.
    /// </remarks>
    ///---------------------------------------------------------------------------
    public class UserInfo
    {
        #region Variable  Declaration

        public int PkUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public int FkCountryId { get; set; }
        public string Address { get; set; }
        public string DescribeYourSelf { get; set; }
        public string School { get; set; }
        public string Profession { get; set; }
        public int FkTimeZoneId { get; set; }
        public bool IsFaceBookConnect { get; set; }
        public bool IsTwitterConnect { get; set; }
        public bool IsLinkedInConnect { get; set; }
        public DateTime LastLoginDate { get; set; }
        public int FkUserId { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public int DeletedBy { get; set; }
        public DateTime DeletedDate { get; set; }
        public int FkStatusId { get; set; }
        public string IpAddress { get; set; }
        public string BrowserType { get; set; }
        public bool? UserIsHost { get; set; }
        public int UserMembershipType { get; set; }
        public int TotalCredits { get; set; }
        public int RemainingCredits { get; set; }
        public string RequestFor { get; set; }
        public DateTime UserVerifiedDate { get; set; }
        public string Skype { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public double SalesCommissionFromUser { get; set; }
        public double SalesCommissionFromXYZ { get; set; }
        public bool IsSalesUser { get; set; }
        public int FkSourceId { get; set; }
        public int NoOfProfileVideoUploadLimit { get; set; }
        public int NoOfProfilePhotoUploadLimit { get; set; }
        public string UserGuid { get; set; }
        public string UserFacebookId { get; set; }
        public int FkStateId { get; set; }
        public int ReviewCounts { get; set; }
        public int ListingCounts { get; set; }
        public string PhotoIds { get; set; }
        public ArrayList UrlArray { get; set; }
        public bool PhoneVerified { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Status { get; set; }
        public decimal ResponseRate { get; set; }
        public double SalesCommissionRate { get; set; }
        public int NoOfTimePromotionalPageViewed { get; set; }
        public string Affinity { get; set; }
        public string BusinessAddress { get; set; }
        public string OtherKnownLanguages { get; set; }
        public int fkCummnicationLanguageId { get; set; }

        public bool IsNewsAboutGFH { get; set; }

        #endregion Variable  Declaration
    }
}