
using System.Collections.Generic;
using System.IO;

namespace StageBeheersTool.Models.Domain
{
    public interface ISpreadsheetService
    {
        void CreateSpreadsheet(string sheetName);
        void CreateColumnWidth(uint startIndex, uint endIndex, double width);
        void AddHeaders(List<string> headers);
        void AddRow(List<string> dataItems);
        MemoryStream GetStream();
        void CloseSpreadsheet();
    }
}
