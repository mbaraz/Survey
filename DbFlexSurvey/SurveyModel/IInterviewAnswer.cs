using System.Collections.Generic;

namespace SurveyModel
{
    public interface IInterviewAnswer
    {
        ICollection<int> Answers { get; }
        IDictionary<int, string> OpenAnswers { get; }
        Dictionary<int, int> Rank { get; }
    }
}