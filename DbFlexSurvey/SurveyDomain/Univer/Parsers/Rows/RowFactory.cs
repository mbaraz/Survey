using System.Collections.Generic;
using LinqToExcel;

namespace SurveyDomain.Univer.Rows
{
    class RowFactory
    {
        internal BaseRow GetRow(Row row, bool isSpecStage)
        {
            if (isSpecStage)
                return new SpecRow(row);
            return new CourseRow(row);
        }

        internal BaseRow GetRow(List<KeyValuePair<int, string>> rowItems, bool isSpecStage)
        {
            if (isSpecStage)
                return new SpecRow(rowItems);
            return new CourseRow(rowItems);
        }
    }
}
