using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SurveyModel;

namespace SurveyDomain.DataExport
{
    class ExportSourceFullProject : ExportSourceProjectBase
    {
        public ExportSourceFullProject(SurveyProject project) : base(project)
        {
        }

        public override IEnumerable<Interview> GetInterviews()
        {
            return Project.ActualInterviews;
        }
    }
}
