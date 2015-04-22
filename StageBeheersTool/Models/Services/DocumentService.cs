using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Models.Services
{
    public class DocumentService : IDocumentService
    {
        public const string Wordmlns = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        public XNamespace Xmlns { get { return XNamespace.Get(Wordmlns); } }

        private byte[] _stagecontractTemplate;

        public byte[] GenerateStagecontract(Stage stage)
        {
            if (_stagecontractTemplate == null)
            {
                var templatePath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"),
                    Path.GetFileName("Stagecontract Template.docx"));
                var template = new FileInfo(templatePath);
                _stagecontractTemplate = File.ReadAllBytes(template.FullName);
            }

            byte[] documentData;
            using (var stream = new MemoryStream())
            {
                stream.Write(_stagecontractTemplate, 0, _stagecontractTemplate.Length);
                using (WordprocessingDocument document = WordprocessingDocument.Open(stream, true))
                {
                    XElement body = XElement.Parse(document.MainDocumentPart.Document.Body.OuterXml);
                    IList<XElement> mailMergeFields =
                        (from el in body.Descendants()
                         where el.Attribute(Xmlns + "instr") != null
                         select el).ToList();

                    foreach (XElement field in mailMergeFields)
                    {
                        string fieldName = field.Attribute(Xmlns + "instr")
                            .Value.Replace("MERGEFIELD", string.Empty).Trim().Split(' ')[0];

                        XElement newElement = field.Descendants(Xmlns + "r").First();
                        var value = GetPropValue(stage, fieldName);
                        newElement.Descendants(Xmlns + "t").First().Value = value == null ? "" : value.ToString();
                        field.ReplaceWith(newElement);
                    }
                    document.MainDocumentPart.Document.Body = new Body(body.ToString());
                    document.MainDocumentPart.Document.Save();
                }
                documentData = stream.ToArray();
            }
            return documentData;
        }

        private static Object GetPropValue(Object obj, string name)
        {
            foreach (string part in name.Split('.'))
            {
                if (obj == null)
                {
                    return null;
                }
                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null)
                {
                    return null;
                }
                obj = info.GetValue(obj, null);
            }
            return obj;
        }

    }
}

