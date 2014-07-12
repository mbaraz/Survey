using System.Collections.Generic;
using SurveyModel;

namespace SurveyInterfaces
{
    public interface IDataExportSource
    {
        IEnumerable<Interview> GetInterviews();
        IEnumerable<IVariable> GetVariables();
    }
}