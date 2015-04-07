using System.IO;
using System.Web.Mvc;

namespace StageBeheersTool.Helpers
{
    public class ExcelFileResult : ActionResult
    {
        private readonly MemoryStream _excelFileStream;
        private readonly string _fileName;

        public ExcelFileResult(MemoryStream spreadsheetStream, string fileName)
        {
            _excelFileStream = spreadsheetStream;
            _fileName = fileName;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.AddHeader("content-disposition", "attachment;filename=" + _fileName);
            context.HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            _excelFileStream.WriteTo(context.HttpContext.Response.OutputStream);
            _excelFileStream.Close();
            context.HttpContext.Response.End();
        }
    }
}