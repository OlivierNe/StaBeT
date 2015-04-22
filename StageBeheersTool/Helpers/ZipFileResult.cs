using System.IO;
using System.Web.Mvc;

namespace StageBeheersTool.Helpers
{
    public class ZipFileResult : ActionResult
    {
        private readonly MemoryStream _memoryStream;
        private readonly string _fileName;

        public ZipFileResult(MemoryStream zipFileStream, string fileName)
        {
            _memoryStream = zipFileStream;
            _fileName = fileName + ".zip";
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.AddHeader("content-disposition", "attachment;filename=" + _fileName);
            context.HttpContext.Response.ContentType = "application/zip";
            _memoryStream.WriteTo(context.HttpContext.Response.OutputStream);
            _memoryStream.Close();
            context.HttpContext.Response.End();
        }
    }
}