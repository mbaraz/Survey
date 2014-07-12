using System.Collections.Generic;
using SurveyModel;

namespace SurveyInterfaces
{
    public interface IVariable
    {
        string Name { get; }
        string Label { get; }
        bool IsString { get; }
        bool HasValueLabels { get; }
        IEnumerable<KeyValuePair<int, string>> ValueLabels { get; }
        string GetValueForInterview(Interview interview);
    }
}