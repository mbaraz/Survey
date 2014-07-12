using System.Collections.Generic;
using LinqToExcel;

namespace SurveyDomain.Univer.Rows
{
    public class CourseRow : BaseRow
    {
        private const int columnQuantity = 7;
        private const string partsDelimiter = ";";
        private const int courseColumnNumber = 1;
        private const int facilityColumnNumber = 2;
        private const int groupColumnNumber = 5;
        private const int amountColumnNumber = 6;
        private static readonly string[] defaultColumnNames = new string[] { "", "ФИО преподавателя", "Название дисциплины", "Курс", "бакалавриат/ специалитет/ магистратура", "Вид занятий", "Каким группам/специализациям читается?", "Сколько студентов записалось на дисциплину" };

        override public string[] DefaultColumnNames { get { return defaultColumnNames; } }
        override internal int ColumnCount { get { return columnQuantity; } }
        override protected string[] defaultParts { get {  return new string[] { "фио", "название", "курс",
                                                                        "бакалавриат/специалитет/магистратура", "вид",
                                                                        "групп/спец", "записа" }; } }
        override protected int[] emptyCells { get { return new int[] { groupColumnNumber }; } }
        override protected int[] numericCells { get { return new int[] { facilityColumnNumber, amountColumnNumber }; } }
        override protected int[] longText { get { return new int[] { courseColumnNumber }; } }
        override protected int[] longInput { get { return new int[] {}; } }
        override protected int[] oneDigitCells { get { return new int[] { facilityColumnNumber }; } }
        override protected int[] groupCells { get { return new int[] { groupColumnNumber }; } }

        internal CourseRow(Row row) : base(row) {}

        internal CourseRow(List<KeyValuePair<int, string>> rowItems) : base(rowItems) {}

        override internal string ParceToCsv(string facilityName)
        {
            string[] resultParts = new string[ColumnCount];
            int partNumber = 0;
            for (int i = 0; i < ColumnCount - 2; i++)
            {
                if (i == facilityColumnNumber)
                    resultParts[partNumber++] = facilityName;
                resultParts[partNumber++] = ExcelRow[i].Value.ToString();
            }
            resultParts[courseColumnNumber] += partsDelimiter + ExcelRow[groupColumnNumber].Value;
            resultParts[amountColumnNumber] = ExcelRow[amountColumnNumber].Value.ToString();
            return string.Join(";", resultParts);
        }

        override public int[] getMinMax(int i)
        {
            if (!IsNumericCell(i))
                return null;
            if (IsOneDigitCell(i))
                return new int[] { 1, 5 };
            return new int[] { 1, 1000 };
        }
    }
}
