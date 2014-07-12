using SurveyDomain.Univer.Rows;

namespace SurveyDomain.Univer.Results
{
    class NamesResults : ResultsBase
    {
        protected override void UpdateMessage(int columnNumber = 0, BaseRow course = null, string message = "")
        {
            base.UpdateMessage();
            string defaultName = course.DefaultColumnNames[columnNumber];
            ErrorMessage += string.Format("Неправильное название {0}-го столбца. Столбец должен называться: <i>\"{1}\"</i>.<br>", columnNumber, defaultName);
        }
    }
}
