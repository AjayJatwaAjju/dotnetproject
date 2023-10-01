using Elmah;
using Newtonsoft.Json;
using RoutingModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace @CNameSpace
{
    /// <summary>
    /// Summary description for RequestData
    /// </summary>
    public class RequestData : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            string results = "";


            if (!string.IsNullOrEmpty(context.Request["getmode"]))
            {

                string mode = context.Request["getmode"].ToString().ToLower();
                switch (mode)
                {
                    case "get@ReplaceTableNamedetails":
                        try
                        {
                            int JsonObject = "";
                            if (!string.IsNullOrEmpty(context.Request["JsonObject"]))
                                JsonObject = context.Request["JsonObject"].ToString();

                            DataTableSearchData dt = JsonConvert.DeserializeObject<DataTableSearchData>(JsonObject);

                            @ReplaceTableNameProvider p = new @ReplaceTableNameProvider();
                            var dataTableOutput = new DataTableOutput();
                            string whereCondition = "";
                            string orderBy = "";
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
                                //column
                            }
                            string pageSize = Data.length.ToString();
                            string pageStart = Data.start.ToString();
                            var output = p.Get@ReplaceTableNameInfo(whereCondition, orderBy, pageStart, pageSize);
                            dataTableOutput.data = JsonConvert.SerializeObject(output);
                            results = JsonConvert.SerializeObject(dataTableOutput);

                        }
                        catch (Exception ex)
                        {
                            results = "[]";
                        }
                        break;
                    case "get@ReplaceTableNamedetailById":
                        try
                        {
                            int id = "";
                            if (!string.IsNullOrEmpty(context.Request["id"]))
                                id = context.Request["id"].ToString();

                            @ReplaceTableNameProvider p = new @ReplaceTableNameProvider();
                            var data = p.Get@ReplaceTableNameInfoById(id);
                            results = JsonConvert.SerializeObject(data);
                        }
                        catch (Exception ex)
                        {
                            results = "[]";
                        }
                        break;
                    case "insert@ReplaceTableNamedetail":
                        try
                        {
                            int JsonObject = "";
                            if (!string.IsNullOrEmpty(context.Request["JsonObject"]))
                                JsonObject = context.Request["JsonObject"].ToString();

                            DataTable dt = JsonConvert.DeserializeObject<DataTable>(JsonObject);
                            @ReplaceTableNameProvider p = new @ReplaceTableNameProvider();
                            var data = p.Update(dt);
                            return Ok(data);

                        }
                        catch (Exception ex)
                        {
                            results = "[]";
                        }
                        break;
                    case "update@ReplaceTableNamedetail":
                        try
                        {
                            int JsonObject = "";
                            if (!string.IsNullOrEmpty(context.Request["JsonObject"]))
                                JsonObject = context.Request["JsonObject"].ToString();

                            DataTable dt = JsonConvert.DeserializeObject<DataTable>(JsonObject);
                            @ReplaceTableNameProvider p = new @ReplaceTableNameProvider();
                            var data = p.Insert(dt);
                            return Ok(data);

                        }
                        catch (Exception ex)
                        {
                            results = "[]";
                        }
                        break;
                    case "delete@ReplaceTableNamedetail":
                        try
                        {
                            int id = "";
                            if (!string.IsNullOrEmpty(context.Request["id"]))
                                id = context.Request["id"].ToString();

                            @ReplaceTableNameProvider p = new @ReplaceTableNameProvider();
                            var data = p.Delete(id);
                            results = JsonConvert.SerializeObject(data);
                        }
                        catch (Exception ex)
                        {
                            results = "[]";
                        }
                        break;
                }
            }
            context.Response.Write(results);
        }        
    }    

}