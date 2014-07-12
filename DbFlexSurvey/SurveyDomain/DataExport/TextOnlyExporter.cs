using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SurveyInterfaces;

namespace SurveyDomain.DataExport
{
    class TextOnlyExporter : ExporterBase
    {
        protected override void DoExport()
        {
            WriteLines(GetInterviews().SelectMany(GetVariablesForInterview).Select(str => str.Trim()).Where(str => str != ""));
        }

        protected override IEnumerable<IVariable> GetVariables()
        {
            return base.GetVariables().Where(v => v.IsString);
        }
    }
}
