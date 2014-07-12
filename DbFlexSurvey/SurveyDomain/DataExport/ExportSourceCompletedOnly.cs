using System.Collections.Generic;
using System.Linq;
using SurveyModel;

namespace SurveyDomain.DataExport
{
    internal class ExportSourceCompletedOnly : ExportSourceProjectBase
    {
        public ExportSourceCompletedOnly(SurveyProject project) : base(project)
        {
        }

        public override IEnumerable<Interview> GetInterviews()
        {
            return Project.ActualInterviews.Where(interview => interview.Completed);
        }
    }
}