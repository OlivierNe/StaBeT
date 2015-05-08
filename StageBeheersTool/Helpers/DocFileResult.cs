using System.IO;
using System.Web.Mvc;

namespace StageBeheersTool.Helpers
{
    public class DocFileResult : ActionResult
    {
        private readonly byte[] _documentData;
        private readonly string _fileName;

        public DocFileResult(byte[] documentData, string fileName)
        {
            _documentData = documentData;
            _fileName = fileName + ".docx";
        }

        public DocFileResult(string path, string fileName)
            : this(File.ReadAllBytes(path), fileName)
        {
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.AddHeader("content-disposition", "attachment;filename=" + _fileName);
            context.HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            context.HttpContext.Response.OutputStream.Write(_documentData, 0, _documentData.Length);
            context.HttpContext.Response.End();
        }
    }
}

