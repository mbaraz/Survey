using System;
using System.Collections.Generic;
using LinqToExcel;
using SurveyCommon;

namespace SurveyDomain.Univer.Parsers.Tools
{
    static class ExcelCellTools
    {
        internal const string csvDelimiter = ";";
        private const string questionMarks = " ??????";
        private const string specSeparator = ",";

        internal static Cell EmptyCel { get { return new Cell(string.Empty); } }
        
        internal static bool IsEmptyValue(Cell cell)
        {
            return string.IsNullOrWhiteSpace(cell.Value.ToString());
        }
        
        internal static bool IsTextValueCorrect(Cell cell, bool canBeEmpty)
        {
            return canBeEmpty || !IsEmptyValue(cell);
        }

        internal static bool IsNumericValueCorrect(Cell сell, int[] minmax)
        {
            bool result = сell.Value.ToString().ToNullableInt() != null;
            if (!result || minmax == null)
                return result;
            int value = int.Parse(сell.Value.ToString().Trim());
            return value >= minmax[0] && value <= minmax[1];
        }

        internal static Cell CleanCell(Cell cell, bool isNumeric = false, int[] minmax = null)
        {
            cell = new Cell(cell.Value.ToString().Trim().Replace(csvDelimiter, ""));
            if (isNumeric && !IsNumericValueCorrect(cell, minmax))
                cell = EmptyCel;
            return cell;
        }

        internal static Cell MakeCell(KeyValuePair<int, string> pair)
        {
            return CleanCell(new Cell(pair.Value));
        }

        internal static Cell IsSpecValuesCorrect(Cell cell, List<string> specs, out bool isCorrect)
        {
            isCorrect = true;
            if (IsEmptyValue(cell))
                return EmptyCel;

            string[] groups = cell.Value.ToString().Trim().Split(new string[] { specSeparator }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < groups.Length; i++ ) {
                string group = groups[i].Trim();
                if (group.Length < 6 || (group.ToNullableInt() == null && !questionMarks.Contains(group)))
                    continue;
                isCorrect &= specs.Contains(group);
                if (!isCorrect)
                    groups[i] = questionMarks;
            }
            cell = new Cell(string.Join(specSeparator, groups));
            return cell;
        }
    }
}
