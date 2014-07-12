using SurveyDomain.DataExport;
using SurveyModel;

namespace SurveyDomain.Univer
{
    internal abstract class UniverExportBase : ExportSourceCompletedOnly
    {
        protected UniverExportBase(SurveyProject project) : base(project)
        {
            
        }
    }
}