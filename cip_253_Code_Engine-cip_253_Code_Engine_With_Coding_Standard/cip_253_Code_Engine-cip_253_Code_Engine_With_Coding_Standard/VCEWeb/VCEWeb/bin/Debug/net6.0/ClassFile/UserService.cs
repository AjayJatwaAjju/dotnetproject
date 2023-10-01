using SIPL.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web;
using VCE.DAL;
using VCE.Shared.Enum;

namespace VCE.Business
{
    ///--------------------------------------------------------------------------
    /// <summary>
    /// Summary description for User Service.
    /// </summary> 
    /// <remarks>
    /// Created On:- 29/05/2023
    /// Created By:- VCE
    /// Purpose:- This class will provide service for UserProvider.
    /// </remarks>
    ///---------------------------------------------------------------------------
    public class UserService
    {
        // Getting path for project
        public string OAppPath = System.Configuration.ConfigurationManager.AppSettings["ProjectName"].Trim();
        //Creating a UserProvider object
        UserProvider oUserProvider = new UserProvider();
         /// <summary>
        /// Inserting User information.
        /// </summary>
        /// <remarks>
        /// <param name="oUserInfo">Passing all User info."</param> 
        /// <returns>returning UserId which is generated.</returns>
        /// </remarks>
        public int Insert(UserInfo oUserInfo)
         {
             return oUserProvider.Insert(oUserInfo);
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
            return oUserProvider.Update(oUserInfo);
         }

        /// <summary>
        ///This will return all user records based on applied condition.
        /// </summary>
        /// <param name="whereCondition">Passing condition for filtering user records</param>
        /// <param name="orderBy">Passing OrderBy</param>
        /// <returns></returns>
        public DataTable GetUserInfo(string whereCondition, string orderBy)
        {
            return oUserProvider.GetUserInfo(whereCondition, orderBy);
        }

        /// <summary>
        /// Get user info by userid
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public UserInfo GetUserInfoById(int userid)
        {
            return oUserProvider.GetUserInfoById(userid);
        }

        /// <summary>
        /// This will delete User table record based on UserId.
        /// </summary>
        /// <remarks/>
        /// <param name="oUserid">Passing deleteId.</param>
        /// <returns>returning delete status.</returns>
        /// <remarks/>
        public bool Delete(int userid)
        {
            return oUserProvider.Delete(userid);
        }

        /// <summary>
        /// To get User Requests Xml list
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetUserList(HttpContext context)
        {
            string whereCondition = "";
            //making condition
            if ((context.Request.Params["CountryId"] != null) && (Convert.ToString(context.Request.Params["CountryId"]) != "0") && (Convert.ToString(context.Request.Params["CountryId"]) != "-1"))
            {
                if (whereCondition != string.Empty)
                {
                    whereCondition = whereCondition + " and ";
                }
                whereCondition += string.Format("fkCountryId='{0}' and IsActive=1", Convert.ToString(context.Request.Params["CountryId"]));
            }
            //making condition
            if (context.Request.Params["StateId"] != null && Convert.ToString(context.Request.Params["StateId"]) != "0")
            {
                if (whereCondition != string.Empty)
                {
                    whereCondition = whereCondition + " and ";
                }
                whereCondition += string.Format("IsApproved=1 and fkStateid='{0}'", Convert.ToString(context.Request.Params["StateId"]));
            }

            //making condition
            if (context.Request.Params["FromDate"] != null && Convert.ToString(context.Request.Params["ToDate"]) != null && context.Request.Params["FromDate"] != "" && Convert.ToString(context.Request.Params["ToDate"]) != "")
            {
                if (whereCondition != string.Empty)
                {
                    whereCondition = whereCondition + " and ";
                }
                whereCondition += string.Format("Convert(datetime,Convert(varchar,CreatedDate,101)) between '{0}' and '{1}'", Convert.ToString(context.Request.Params["FromDate"]), Convert.ToString(context.Request.Params["ToDate"]));
            }
         
            //making condition
            if (context.Request.Params["UserStatus"] != null && Convert.ToString(context.Request.Params["UserStatus"]) != "0")
            {
                if (whereCondition != string.Empty)
                {
                    whereCondition = whereCondition + " and ";
                }
                whereCondition += string.Format("fkStatusid={0}", Convert.ToString(context.Request.Params["UserStatus"]));
            }
          
            //making condition
            if (context.Request.Params["UserType"] != null && Convert.ToString(context.Request.Params["UserType"]) != "0")
            {
                if (whereCondition != string.Empty)
                {
                    whereCondition = whereCondition + " and ";
                }
                whereCondition += string.Format("(UserType='{0}')", Convert.ToString(context.Request.Params["UserType"]));
            }

            string filterText = string.Empty;
            if (context.Request.Params["FilterText"] != null)
            {
                filterText = Convert.ToString(context.Request.Params["FilterText"]).Trim();
                whereCondition += "and FirstName+' '+LastName  like '%" + filterText + "%' ";
            }
            var dt = GetUserInfo(whereCondition, string.Empty);
            return BuildUserList(dt, context);
        }

        /// <summary>
        /// Function for showing User List Xml
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string BuildUserList(DataTable dt, HttpContext context)
        {
            var xmlString = "";
            var page = Convert.ToInt32(context.Request.Params["page"]);
            var rows = Convert.ToInt32(context.Request.Params["rows"]);
            var pageIndex = Convert.ToInt32(page) - 1;
            var pageSize = rows;
            string status = string.Empty;
            var newdt = new DataTable();
            if (dt.Rows.Count > 0)
            {
                var dv = new DataView(dt)
                {
                    Sort = " " + context.Request.Params["sidx"] + " " + context.Request.Params["sord"] + " "
                };
                IEnumerable<DataRow> UserRequestsTypeList = dv.ToTable().AsEnumerable().Skip(pageIndex * pageSize).Take(pageSize).ToList();
                var totalPages = (int)Math.Ceiling((float)dt.Rows.Count / (float)pageSize);
                if (UserRequestsTypeList.Count() > 0)
                {
                    xmlString += "<rows>";
                    xmlString += "<page>" + page + "</page>";
                    xmlString += "<total>" + totalPages + "</total>";
                    xmlString += "<records>" + UserRequestsTypeList.Count() + "</records>";
                    //Add data in new data table for ExportUtility
                    newdt.Columns.Add("User", typeof(string));
                    newdt.Columns.Add("Skype", typeof(string));
                    newdt.Columns.Add("Email", typeof(string));
                    newdt.Columns.Add("AssignedCountry", typeof(string));
                    newdt.Columns.Add("AssignedRegion", typeof(string));
                    newdt.Columns.Add("Status", typeof(string));
                    newdt.Columns.Add("LastLoginDate", typeof(string));
                    //newdt.Columns.Add("Type", typeof(string));
                    foreach (var dr in UserRequestsTypeList)
                    {
                        xmlString += "<row >";
                        DataRow row = newdt.NewRow();
                        row["User"] = dr["FirstName"] + " " + dr["LastName"];
                        row["Skype"] = dr["Skype"];
                        row["Email"] = dr["EmailId"];
                        row["AssignedCountry"] = dr["AssignedCountry"];
                        row["AssignedRegion"] = dr["AssignedRegion"];
                        row["LastLoginDate"] = String.Format("{0:MM/dd/yyyy hh:mm}", dr["LastLoginDate"]);
                        xmlString += "<cell><![CDATA[" + dr["FirstName"] + " " + dr["LastName"] + "]]></cell>";
                        xmlString += "<cell><![CDATA[" + dr["Skype"] + "]]></cell>";
                        xmlString += "<cell><![CDATA[" + dr["EmailId"] + "]]></cell>";
                        xmlString += "<cell><![CDATA[" + dr["AssignedCountry"] + "]]></cell>";
                        xmlString += "<cell><![CDATA[" + dr["AssignedRegion"] + "]]></cell>";
                        status = ""; // EUserStatus.GetName(typeof(EUserStatus), dr["fkStatusid"]);
                        if (status == "Enabled") { status = "Active"; }
                        else if (status == "Disabled") { status = "InActive"; }
                        xmlString += "<cell><![CDATA[" + status + "]]></cell>";
                        row["Status"] = status;
                        var roleName = "";
                        xmlString += "<cell><![CDATA[" + roleName + "]]></cell>";
                        if ((dr["LastLoginDate"] != null))
                        {
                            xmlString += "<cell><![CDATA[" + String.Format("{0:MM/dd/yyyy hh:mm}", dr["LastLoginDate"]) + "]]></cell>";
                        }
                        else
                        {
                            xmlString += "<cell><![CDATA[]]></cell>";
                        }
                        //row["Type"] = roleName;
                        xmlString += "<cell><![CDATA[<img src='" + OAppPath + "/images/Detail.jpg'  onclick='javascript:fnUserDetail(" + dr["pkUserId"] + ");' style='cursor:pointer;' />]]></cell>";
                        xmlString += "<cell><![CDATA[<img src='" + OAppPath + "/images/status.png'  onclick='javascript:fnUserpromotionalpage(" + dr["pkUserId"] + ");' style='cursor:pointer;' />]]></cell>";
                        xmlString += "</row>";
                        newdt.Rows.Add(row);
                    }
                    xmlString += "</rows>";
                }
                context.Session.Add("dtUsersList", newdt);
            }
            else
            {
                xmlString += "<rows>";
                xmlString += "<page>0</page>";
                xmlString += "<total>0</total>";
                xmlString += "<records>0</records></rows>";
            }
            return xmlString;
        }
        /// <summary>
        /// To get user list as per json
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetUserListJson(HttpContext context)
        {
            string strUserTypeJson = "[";
            string autoCompleteText = string.Empty;
            if (context.Request.Params["autoCompleteText"] != null)
            {
                autoCompleteText = Convert.ToString(context.Request.Params["autoCompleteText"]).Trim();
            }
            string whereCondition = string.Empty;
            //getting language wise BedType
            //whereCondition = string.Format("fkLanguageId={0}", (int)ELanguage.English);
            if (autoCompleteText != string.Empty)
            {
                //apply search filter
                whereCondition = whereCondition + "FirstName+' '+LastName like '%" + autoCompleteText + "%' ";
                whereCondition += string.Format(" and pkUserid in (Select fkUserid From GFH_UserRoles Where (fkRoleid='{0}') or (fkRoleid='{1}'))", Convert.ToString((int)ERoleType.Agent), Convert.ToString((int)ERoleType.Partner));
            }
            var dt = GetUserInfo(whereCondition, "Name");

            if (dt.Rows.Count == 0)
            {
                strUserTypeJson += "{\"value\":0,\"text\":\"\"}";
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    strUserTypeJson += "{\"value\":" + dr["pkUserId"] + ",\"text\":\"" + dr["FirstName"] + " " + dr["LastName"] + "\"},";
                }
                strUserTypeJson = strUserTypeJson.Substring(0, strUserTypeJson.Length - 1);
            }
            strUserTypeJson += "]";
            return strUserTypeJson;
        }
    }
}
