using System;
using System.Collections.Generic;
using System.Linq;
using LinqToExcel;
using SurveyDomain.Univer.Parsers.Tools;

namespace SurveyDomain.Univer.Rows
{
    abstract public class BaseRow
    {
        public readonly Row ExcelRow;

        abstract public string[] DefaultColumnNames { get; }
        abstract internal int ColumnCount { get; }
        
        abstract protected string[] defaultParts { get; }
        abstract protected int[] emptyCells { get; }
        abstract protected int[] numericCells { get; }
        abstract protected int[] longText { get; }
        abstract protected int[] longInput { get; }
        abstract protected int[] oneDigitCells { get; }

        abstract public int[] getMinMax(int i);

        abstract internal string ParceToCsv(string facilityName);

        virtual protected int[] groupCells { get { return new int[] {}; } }

        internal bool IsRowEmpty 
        {
            get
            {
                var whiteCells = from c in ExcelRow where ExcelCellTools.IsEmptyValue(c) select c;
                return whiteCells.Count() >= ColumnCount;
            }
        }

        internal BaseRow(Row row)
        {
            ExcelRow = row;
        }

        internal BaseRow(List<KeyValuePair<int, string>> rowItems)
        {
            ExcelRow = new Row();
            for (int i = 0; i < ColumnCount; i++)
                ExcelRow.Add(ExcelCellTools.MakeCell(rowItems.ElementAt(i)));
        }

        internal List<int> CheckColumnNames()
        {
            List<int> result = new List<int>();
            for (int i = 0; i < ColumnCount; i++)
                if (ExcelRow.ColumnNames.Count() <= i || !CompareColumnNames(ExcelRow.ColumnNames.ElementAt(i), i))
                    result.Add(i + 1);
            return result;
        }

        internal bool IsCorrect(out string errorMessage, List<string> specsCodes)
        {
            bool result = checkTexts(specsCodes);
            if (result) {
                errorMessage = "во 2-м и 6-м столбцах должны быть числа";
                result = checkNumbers();
            }
            else
                errorMessage = "не заполнены ячейки";

            return result;
        }

        internal void CleanCellValues()
        {
            for (int i = 0; i < ColumnCount; i++)
                ExcelRow[i] = ExcelCellTools.CleanCell(ExcelRow[i], IsNumericCell(i), getMinMax(i));
        }

        internal bool CompareColumnNames(string columnName, int columnNumber)
        {
            bool isCorrect = true;
            string[] nameParts = defaultParts[columnNumber].Split('/');
            for (int i = 0; i < nameParts.Length && isCorrect; i++)
                isCorrect = columnName.ToLower().Contains(nameParts[i]);
            return isCorrect;
        }

        protected bool checkTexts(List<string> specsCodes)
        {
            bool result = true;
            for (int i = 0; i < ColumnCount && result; i++)
                result = ExcelCellTools.IsTextValueCorrect(ExcelRow[i], CanBeEmptyCell(i));
/*
            if (result && groupCells.Length > 0) {
                ExcelRow[groupCells[0]] = ExcelCellTools.IsSpecValuesCorrect(ExcelRow[groupCells[0]], specsCodes, out result);
                clleanCourseIfNeeded(!result);
            }*/
            return result && checkSpecsIfNeeded(specsCodes, result);
        }

        protected bool checkNumbers()
        {
            bool isCorrect = true;
            for (int i = 0; i < numericCells.Length && isCorrect; i++)
                isCorrect = ExcelCellTools.IsNumericValueCorrect(ExcelRow[numericCells[i]], getMinMax(i));
            
            return isCorrect;
        }

        public bool CanBeEmptyCell(int сellNumber)
        {
            return Array.IndexOf(emptyCells, сellNumber) != -1;
        }

        public bool IsNumericCell(int сellNumber)
        {
            return Array.IndexOf(numericCells, сellNumber) != -1;
        }

        public bool IsLongText(int сellNumber)
        {
            return Array.IndexOf(longText, сellNumber) != -1;
        }

        public bool IsLongInput(int сellNumber)
        {
            return Array.IndexOf(longInput, сellNumber) != -1;
        }

        public bool IsOneDigitCell(int сellNumber)
        {
            return Array.IndexOf(oneDigitCells, сellNumber) != -1;
        }

        private bool checkSpecsIfNeeded(List<string> specsCodes, bool isNeeded)
        {
            if (!isNeeded || groupCells.Length == 0)
                return true;
            bool result;
            ExcelRow[groupCells[0]] = ExcelCellTools.IsSpecValuesCorrect(ExcelRow[groupCells[0]], specsCodes, out result);
            clleanCourseIfNeeded(!result);
            return result;
        }

        private void clleanCourseIfNeeded(bool isNeeded)
        {
            if (!isNeeded)
                return;
            ExcelRow[oneDigitCells[0]] = ExcelCellTools.EmptyCel;
        }

    }
}
