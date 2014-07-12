using System.Collections.Generic;
using System.Linq;
using SurveyModel;

namespace SurveyDomain.Univer
{
    class ExportComments : UniverExportBase
    {
        public ExportComments(SurveyProject project) : base(project)
        {
        }

        protected override IEnumerable<SurveyQuestion> GetQuestions
        {
            get
            {
                return
                    base.GetQuestions.Where(q => q.QuestionName == "Comments");
            }
        }
    }
}
