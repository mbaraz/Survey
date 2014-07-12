using System.Collections.Generic;
using SurveyDomain.Univer.Rows;

namespace SurveyDomain.Univer.Results
{
    public class ContentResults : ResultsBase
    {
        private const int maxErrors = 40;

        override public bool ShowErrorTextOnly { get { return IsEnough; } }

        internal bool IsEnough
        {
            get { return errorsCount > maxErrors; }
        }

        internal ContentResults()
        {
            ErrorRows = new List<BaseRow>();
        }

        override public void AddErrorRow(int rowCount, BaseRow row, string errorMessage = "")
        {
            if (row.IsRowEmpty)
                return;
            row.CleanCellValues();
            ErrorRows.Add(row);
            base.AddErrorRow(rowCount, row, errorMessage);
        }
    }
}
