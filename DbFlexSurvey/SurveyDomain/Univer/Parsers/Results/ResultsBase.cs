using System.Collections.Generic;
using SurveyDomain.Univer.Rows;
using SurveyInterfaces;

namespace SurveyDomain.Univer.Results
{
    public class ResultsBase : IParseResult
    {
        public string ErrorMessage { get; protected set; }
        public List<BaseRow> ErrorRows { get; protected set; }
        public bool NoError { get { return errorsCount == 0; } }
        virtual public bool ShowErrorTextOnly { get { return true; } }

        protected int errorsCount = 0;

        virtual public void AddErrorRow(int rowCount, BaseRow course, string errorMessage = "")
        {
            UpdateMessage(rowCount, course, errorMessage);
        }

        virtual protected void UpdateMessage(int number = 0, BaseRow course = null, string message = "")
        {
            ErrorMessage += string.Format("{0}. " + message + "<br/>", ++errorsCount);
        }
    }
}
