using System.Collections.Generic;
using LinqToExcel;
using SurveyDomain.Univer.Parsers.Tools;

namespace SurveyDomain.Univer.Rows
{
    public class SpecRow : BaseRow
    {
        private const int columnQuantity = 3;
        private const int codeColumnNumber = 0;
        private const int nameColumnNumber = 1;

        private static readonly string[] parts = new string[] { "код", "название", "бакалавриат/специалитет/магистратура" };
        private static readonly string[] defaultColumnNames = new string[] { "", "Код специализации", "Название специализации", "бакалавриат/ специалитет/ магистратура" };
        
        override public string[] DefaultColumnNames { get { return defaultColumnNames; } }
        override internal int ColumnCount { get { return columnQuantity; } }
        override protected string[] defaultParts { get {  return parts; } }
        override protected int[] emptyCells { get { return new int[] {}; } }
        override protected int[] numericCells { get { return new int[] { codeColumnNumber }; } }
        override protected int[] longText { get { return new int[] {}; } }
        override protected int[] longInput { get { return new int[] { nameColumnNumber }; } }
        override protected int[] oneDigitCells { get { return new int[] {}; } }

        internal SpecRow(Row row) : base(row) {}

        internal SpecRow(List<KeyValuePair<int, string>> rowItems) : base(rowItems){}

        override internal string ParceToCsv(string facilityName)
        {
            string result = facilityName + ExcelCellTools.csvDelimiter;
            for (int i = 0; i < ColumnCount; i++)
            {
                result += ExcelRow[i].Value;
                if (i < ColumnCount - 1)
                    result += ExcelCellTools.csvDelimiter;
            }
            return result;
        }

        override public int[] getMinMax(int i)
        {
            if (!IsNumericCell(i))
                return null;
            return new int[] { 100000, 999999 };
        }
    }
}
