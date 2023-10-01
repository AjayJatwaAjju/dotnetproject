using System.Data;
using System.IO;
using System.Web;

namespace VCE.Utility
{
    public class ExportToExcelBuilder
    {
        #region "ExportExcel"
        /// <summary>
        /// Export Excel shit with data table 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="excelHeader"></param>
        /// <param name="dtExcel"></param>
        /// <returns></returns>
        public static void GetExportExcel(string filename, string excelHeader, DataTable dtExcel)
        {
            string attachment = string.Format("attachment; filename={0}", filename);
            var tw = new StringWriter();
            var hw = new HtmlTextWriter(tw);
            var dgGrid = new DataGrid { DataSource = dtExcel };
            dgGrid.DataBind();  // Report Header
            hw.WriteLine("<b><u><font size='3'> " + excelHeader + " </font></u></b><br/><br/>");    //Get the HTML for the control.
            dgGrid.RenderControl(hw);  //Write the HTML back to the browser.
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            HttpContext.Current.Response.AppendHeader("Content-Disposition", attachment);
            //this.EnableViewState = false;
            HttpContext.Current.Response.Write(tw.ToString());
            HttpContext.Current.Response.End();
        }
        /// <summary>
        /// Export Excel shit with HTML 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="excelHeader"></param>
        /// <param name="strGrid"></param>
        public static void GetExportExcel(string filename, string excelHeader, string strGrid)
        {
            string attachment = string.Format("attachment; filename={0}", filename);
            var tw = new StringWriter();
            var hw = new HtmlTextWriter(tw);
            HttpContext.Current.Response.Charset = "";
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.AddHeader("content-disposition", attachment);
            HttpContext.Current.Response.ContentType = "application/ vnd.xls";
            HttpContext.Current.Response.Write(strGrid);
            HttpContext.Current.Response.End();
        }
        #endregion
    }
}
