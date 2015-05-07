using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Linq;
using DocumentFormat.OpenXml;
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

                        XElement newElement = field.Descendants(Xmlns + "r").FirstOrDefault();
                        if (newElement != null)
                        {
                            var value = GetPropValue(stage, fieldName);

                            string[] pieces = value.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                            if (pieces.Length > 1)
                            {
                                var run = ParseTextFoNewLines(pieces);
                                var paragraph = new Paragraph(run);
                                newElement.Value = "";
                                var openElement = ConvertXElementToOpenXmlElement(newElement);
                                openElement.Append(paragraph);
                                newElement = ConvertOpenxmlElementToXElement(openElement);
                            }
                            else
                            {
                                newElement.Descendants(Xmlns + "t").First().Value = value.ToString();
                            }
                            field.ReplaceWith(newElement);
                            field.Attribute(Xmlns + "instr").Remove();
                        }
                    }
                    document.MainDocumentPart.Document.Body = new Body(body.ToString());
                    document.MainDocumentPart.Document.Save();
                }
                documentData = stream.ToArray();
            }
            return documentData;
        }

        private OpenXmlElement ConvertXElementToOpenXmlElement(XElement xElement)
        {
            OpenXmlElement openXmlElement = null;
            using (StreamWriter sw = new StreamWriter(new MemoryStream()))
            {
                sw.Write(xElement.ToString());
                sw.Flush();
                sw.BaseStream.Seek(0, SeekOrigin.Begin);
                OpenXmlReader re = OpenXmlReader.Create(sw.BaseStream);
                re.Read();
                openXmlElement = re.LoadCurrentElement();
                re.Close();
            }
            return openXmlElement;
        }

        private XElement ConvertOpenxmlElementToXElement(OpenXmlElement openXmlElement)
        {
            return XElement.Parse(openXmlElement.OuterXml);
        }

        private Run ParseTextFoNewLines(IEnumerable<string> pieces)
        {
            RunProperties runProp = new RunProperties();
            RunFonts runFont = new RunFonts();
            runFont.Ascii = "Arial";
            FontSize size = new FontSize();
            size.Val = new StringValue("20");
            runProp.Append(runFont);
            runProp.Append(size);
            Run run = new Run();
            run.PrependChild<RunProperties>(runProp);
            bool first = true;
            foreach (string line in pieces)
            {
                if (!first)
                {
                    run.Append(new Break());
                }
                first = false;
                Text txt = new Text
                {
                    Text = line.Trim()
                };
                run.Append(txt);
            }
            return run;
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

