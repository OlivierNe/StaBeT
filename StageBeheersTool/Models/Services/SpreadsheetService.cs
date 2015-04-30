using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Models.Services
{
    public class SpreadsheetService : ISpreadsheetService
    {
        private SpreadsheetDocument _spreadSheet;
        private Columns _cols;

        private Worksheet CurrentWorkSheet
        {
            get { return _spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet; }
        }

        public MemoryStream SpreadsheetStream { get; set; }

        /// <summary>
        ///                        Spreadsheet
        ///                              |         
        ///                         WorkbookPart    
        ///                   /         |             \
        ///           Workbook WorkbookStylesPart WorksheetPart
        ///                 |          |               |
        ///            Sheets     StyleSheet        Worksheet
        ///                |                        /        \       
        ///          (refers to               SheetData        Columns  
        ///           Worksheetparts)            |   
        ///                                     Rows 
        /// 
        /// </summary>
        public void CreateSpreadsheet(string sheetName)
        {
            SpreadsheetStream = new MemoryStream();
            _spreadSheet = SpreadsheetDocument.Create(SpreadsheetStream, SpreadsheetDocumentType.Workbook);

            var workbookpart = _spreadSheet.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            var workbookStylesPart = _spreadSheet.WorkbookPart.AddNewPart<WorkbookStylesPart>();
            workbookStylesPart.Stylesheet = GenerateStyleSheet();

            var sheets = _spreadSheet.WorkbookPart.Workbook.AppendChild(new Sheets());
            var sheet = new Sheet()
            {
                Id = _spreadSheet.WorkbookPart.
                    GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = sheetName
            };
            sheets.Append(sheet);
            _cols = new Columns();
        }

        public MemoryStream GetStream()
        {
            return SpreadsheetStream;
        }

        public void CreateColumnWidth(uint startIndex, uint endIndex, double width)
        {
            if (CurrentWorkSheet.Count(x => x.LocalName == "cols") > 0)
                CurrentWorkSheet.RemoveChild(_cols);
            var column = new Column
            {
                Min = startIndex,
                Max = endIndex,
                Width = width,
                CustomWidth = true
            };
            _cols.Append(column);

            CurrentWorkSheet.InsertBefore(_cols, CurrentWorkSheet.GetFirstChild<SheetData>());
        }

        public void AddHeaders(List<string> headers)
        {
            var sheetData = CurrentWorkSheet.GetFirstChild<SheetData>();
            var row = new Row
            {
                RowIndex = Convert.ToUInt32(sheetData.ChildElements.Count()) + 1
            };
            sheetData.Append(row);
            foreach (var header in headers)
            {
                AppendCell(row, row.RowIndex, header, 1);
            }
        }

        public void AddRow(List<string> dataItems)
        {
            var sheetData = CurrentWorkSheet.GetFirstChild<SheetData>();
            var row = new Row
            {
                RowIndex = Convert.ToUInt32(sheetData.ChildElements.Count()) + 1
            };
            sheetData.Append(row);
            foreach (string item in dataItems)
            {
                AppendCell(row, row.RowIndex, item, 0);
            }
        }

        public void CloseSpreadsheet()
        {
            CurrentWorkSheet.Save();
            _spreadSheet.Close();
        }

        public IList<Student> ImportStudenten(Stream inputStream)
        {
            var studenten = new List<Student>();
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(inputStream, false))
            {
                SharedStringTable sharedStringTable = document.WorkbookPart.SharedStringTablePart.SharedStringTable;

                WorksheetPart worksheetPart = document.WorkbookPart.WorksheetParts.First();
                var columnHeadings = new List<string>();

                foreach (SheetData sheetData in worksheetPart.Worksheet.Elements<SheetData>())
                {
                    if (sheetData.HasChildren)
                    {

                        foreach (Row row in sheetData.Elements<Row>())
                        {
                            var studentData = new List<string>();
                            foreach (Cell cell in row.Elements<Cell>())
                            {
                                string cellValue = cell.InnerText;
                                string value;
                                if (cell.DataType != null && cell.DataType == CellValues.SharedString)
                                {
                                    value = sharedStringTable.ElementAt(Int32.Parse(cellValue)).InnerText;
                                }
                                else
                                {
                                    value = cellValue;
                                }
                                if (row == sheetData.Elements<Row>().First())
                                {
                                    columnHeadings.Add(value);
                                }
                                else
                                {
                                    studentData.Add(value);
                                }
                            }
                            if (studentData.Count > 0)
                            {
                                Student student = new Student();
                                for (int i = 0; i < columnHeadings.Count; i++)
                                {
                                    SetStudentProperty(student, columnHeadings[i], studentData[i]);
                                }
                                studenten.Add(student);
                            }
                        }
                    }
                }
            }
            return studenten;
        }

        private void SetStudentProperty(Student student, string columnName, string studentProperty)
        {
            switch (columnName)
            {
                case "Student":
                    var idx = studentProperty.LastIndexOf(' ');
                    student.Voornaam = studentProperty.Substring(idx).Trim();
                    student.Familienaam = studentProperty.Substring(0, idx).Trim();
                    break;
                case "Geboortedatum":
                    student.Geboortedatum = DateTime.Parse(studentProperty,
                        CultureInfo.CreateSpecificCulture("nl-BE"));
                    break;
                case "Adres":
                    student.Straat = studentProperty;
                    break;
                case "Gemeente":
                    var indx = studentProperty.IndexOf(' ');
                    student.Gemeente = studentProperty.Substring(indx);
                    student.Postcode = studentProperty.Substring(0, indx);
                    break;
                case "School e-mailadres":
                    student.HogentEmail = studentProperty;
                    break;
                case "Privé e-mailadres":
                    student.Email = studentProperty;
                    break;
                case "Telefoon":
                    var telInx = studentProperty.IndexOf('(');
                    student.Telefoon = telInx == -1 ? studentProperty :
                        studentProperty.Substring(0, telInx).Trim();
                    break;
                case "GSM":
                    var gsmIdx = studentProperty.IndexOf('(');
                    student.Gsm = gsmIdx == -1 ? studentProperty :
                        studentProperty.Substring(0, gsmIdx).Trim();
                    break;
            }
        }

        #region helpers
        private void AppendCell(Row row, uint rowIndex, string value, uint styleIndex)
        {
            var cell = new Cell
            {
                DataType = CellValues.InlineString,
                StyleIndex = styleIndex
            };
            var text = new Text
            {
                Text = value
            };

            var inlineString = new InlineString();
            inlineString.AppendChild(text);
            cell.AppendChild(inlineString);

            var nextCol = "A";
            var c = (Cell)row.LastChild;
            if (c != null)
            {
                int numIndex =
                    c.CellReference.ToString().IndexOfAny(new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                string lastCol = c.CellReference.ToString().Substring(0, numIndex);
                nextCol = IncrementColRef(lastCol);
            }

            cell.CellReference = nextCol + rowIndex;

            row.AppendChild(cell);
        }

        // Increment the column reference in an Excel fashion, i.e. A, B, C...Z, AA, AB etc.
        private string IncrementColRef(string lastRef)
        {
            char[] characters = lastRef.ToUpperInvariant().ToCharArray();
            int sum = 0;
            foreach (char c in characters)
            {
                sum *= 26;
                sum += (c - 'A' + 1);
            }
            sum++;

            string columnName = String.Empty;
            while (sum > 0)
            {
                int modulo = (sum - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                sum = (int)((sum - modulo) / 26);
            }
            return columnName;
        }

        private Stylesheet GenerateStyleSheet()
        {
            return new Stylesheet(
                new Fonts(
                    new Font( // Index 0 - The default font.
                        new FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Calibri" }),
                    new Font( // Index 1 - The bold font.
                        new Bold(),
                        new FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Calibri" }),
                    new Font( // Index 2 - The Italic font.
                        new Italic(),
                        new FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Calibri" }),
                    new Font( // Index 2 - The Times Roman font. with 16 size
                        new FontSize() { Val = 16 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Times New Roman" })
                    ),
                new Fills(
                    new Fill( // Index 0 - The default fill.
                        new PatternFill() { PatternType = PatternValues.None }),
                    new Fill( // Index 1 - The default fill of gray 125 (required)
                        new PatternFill() { PatternType = PatternValues.Gray125 }),
                    new Fill( // Index 2 - The yellow fill.
                        new PatternFill(
                            new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "FFFFFF00" } }
                            ) { PatternType = PatternValues.Solid })
                    ),
                new Borders(
                    new Border( // Index 0 - The default border.
                        new LeftBorder(),
                        new RightBorder(),
                        new TopBorder(),
                        new BottomBorder(),
                        new DiagonalBorder()),
                    new Border( // Index 1 - Applies a Left, Right, Top, Bottom border to a cell
                        new LeftBorder(
                            new Color() { Auto = true }
                            ) { Style = BorderStyleValues.Thin },
                        new RightBorder(
                            new Color() { Auto = true }
                            ) { Style = BorderStyleValues.Thin },
                        new TopBorder(
                            new Color() { Auto = true }
                            ) { Style = BorderStyleValues.Thin },
                        new BottomBorder(
                            new Color() { Auto = true }
                            ) { Style = BorderStyleValues.Thin },
                        new DiagonalBorder())
                    ),
                new CellFormats(
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 0 },
                // Index 0 - The default cell style.  If a cell does not have a style index applied it will use this style combination instead
                    new CellFormat() { FontId = 1, FillId = 0, BorderId = 0, ApplyFont = true }, // Index 1 - Bold 
                    new CellFormat() { FontId = 2, FillId = 0, BorderId = 0, ApplyFont = true }, // Index 2 - Italic
                    new CellFormat() { FontId = 3, FillId = 0, BorderId = 0, ApplyFont = true }, // Index 3 - Times Roman
                    new CellFormat() { FontId = 0, FillId = 2, BorderId = 0, ApplyFill = true }, // Index 4 - Yellow Fill
                    new CellFormat( // Index 5 - Alignment
                        new Alignment()
                        {
                            Horizontal = HorizontalAlignmentValues.Center,
                            Vertical = VerticalAlignmentValues.Center
                        }
                        ) { FontId = 0, FillId = 0, BorderId = 0, ApplyAlignment = true },
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true } // Index 6 - Border
                    )
                );
        }

        #endregion

    }
}