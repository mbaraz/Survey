using System.Collections.Generic;
using System.Linq;
using SurveyDomain.Univer.Rows;

namespace SurveyDomain.Univer.Parsers.Tools
{
    public static class ExcelRowTools
    {
        internal static List<BaseRow> MakeRowList(List<KeyValuePair<int, string>> rowItems, bool isSpecStage)
        {
            List<BaseRow> result = new List<BaseRow>();
            var factory = new RowFactory();
            int i = 0;
            int rangeCapacity = rowItems.Count();
            while (i < rowItems.Count())
            {
                BaseRow item = factory.GetRow(rowItems.GetRange(i, rangeCapacity), isSpecStage);
                result.Add(item);
                rangeCapacity = item.ColumnCount;
                i += rangeCapacity;
            }
            return result;
        }
    }
}
