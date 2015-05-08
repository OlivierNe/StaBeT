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

        public byte[] StagecontractTemplate
        {
            get
            {
                return _stagecontractTemplate;
            }
            set { _stagecontractTemplate = value; }
        }

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
                    IList<SdtElement> controls = document.MainDocumentPart.RootElement.
                        Descendants<SdtElement>().Where(e => e is SdtBlock || e is SdtRun).ToList();

                    foreach (var control in controls)
                    {
                        var tag = control.SdtProperties.GetFirstChild<Tag>().Val.Value;
                        var value = GetPropValue(stage, tag);
                        var newRun = ParseTextFoNewLines(value);
                        var run = control.Descendants<Run>().First();
                        run.Append(newRun);

                        control.Descendants<Text>().First().Text = "";

                        if (control is SdtRun)
                        {
                            SdtRun sdtRun = (SdtRun)control;
                            foreach (var elem in sdtRun.SdtContentRun.Elements())
                            {
                                control.Parent.InsertBefore(elem.CloneNode(true), sdtRun);
                            }
                            sdtRun.Remove();
                        }
                        else if (control is SdtBlock)
                        {
                            SdtBlock sdtBlock = (SdtBlock)control;
                            foreach (var elem in sdtBlock.SdtContentBlock.Elements())
                            {
                                control.Parent.InsertBefore(elem.CloneNode(true), sdtBlock);
                            }
                            sdtBlock.Remove();
                        }
                    }
                    document.MainDocumentPart.Document.Save();
                }
                documentData = stream.ToArray();
            }
            return documentData;
        }

        private Run ParseTextFoNewLines(object value)
        {
            string text = (value == null) ? "" : value.ToString();

            Run run = new Run();
            string[] pieces = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            if (pieces.Length <= 0)
            {
                run.Append(new Text(text));
                return run;
            }
            RunProperties runProp = new RunProperties();
            RunFonts runFont = new RunFonts();
            runFont.Ascii = "Arial";
            FontSize size = new FontSize();
            size.Val = new StringValue("20");
            runProp.Append(runFont);
            runProp.Append(size);
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

