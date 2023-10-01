namespace VCE.Utility
{
    public class ExportToPDFBuilder
    {
        #region "ExportPDF Using iTextSharp"

        //public static byte[] GetExportPdfWithContent(string Content)
        //{
        //    //Page sizes are found in iTextSharp.text.PageSize
        //    var builder = new HtmlToPdfBuilder(PageSize.LETTER);
        //    var first = builder.AddPage();
        //    first.AppendHtml(Content);
        //    return builder.RenderPdf();
        //}
        ///// <summary>
        ///// Export PDF shit with data table
        ///// </summary>
        ///// <param name="filename"></param>
        ///// <param name="pdfHeader"></param>
        ///// <param name="dtPdf"></param>
        //public static void GetExportPdf(string filename, string pdfHeader, DataTable dtPdf)
        //{
        //    HttpContext.Current.Response.Clear();
        //    HttpContext.Current.Response.ContentType = "Application/pdf";
        //    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + filename + "");
        //    var sw = new StringWriter();
        //    var htw = new HtmlTextWriter(sw);
        //    htw.WriteLine("<b><u><font size='3'> " + pdfHeader + " </font></u></b><br/><br/>");
        //    var gridView1 = new GridView { AllowPaging = false, DataSource = dtPdf };
        //    gridView1.DataBind();
        //    gridView1.RenderControl(htw);
        //    //Page sizes are found in iTextSharp.text.PageSize
        //    var builder = new HtmlToPdfBuilder(PageSize.LETTER);
        //    var first = builder.AddPage();
        //    first.AppendHtml(sw.ToString());
        //    HttpContext.Current.Response.BinaryWrite(builder.RenderPdf());
        //    HttpContext.Current.Response.Flush();
        //    HttpContext.Current.Response.Close();
        //    HttpContext.Current.Response.End();
        //}
        ///// <summary>
        ///// Export PDF shit with HTML
        ///// </summary>
        ///// <param name="filename"></param>
        ///// <param name="pdfHeader"></param>
        ///// <param name="content"></param>
        //public static void GetExportPdf(string filename, string pdfHeader, string content)
        //{
        //    HttpContext.Current.Response.Clear();
        //    HttpContext.Current.Response.ContentType = "Application/pdf";
        //    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + filename + "");
        //    //Page sizes are found in iTextSharp.text.PageSize
        //    var builder = new HtmlToPdfBuilder(PageSize.LETTER);
        //    var first = builder.AddPage();
        //    first.AppendHtml(content);

        //    HttpContext.Current.Response.BinaryWrite(builder.RenderPdf());
        //    HttpContext.Current.Response.Flush();
        //    HttpContext.Current.Response.Close();
        //    HttpContext.Current.Response.End();
        //}

        #endregion "ExportPDF Using iTextSharp"
    }
}