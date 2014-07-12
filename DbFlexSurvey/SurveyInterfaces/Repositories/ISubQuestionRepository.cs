using System.Collections.Generic;
using SurveyModel;

namespace SurveyInterfaces.Repositories
{
    public interface ISubQuestionRepository : IRepository<SubQuestion>
    {
        IEnumerable<int> deletedIds { set; }
        void save(SurveyQuestion question, ICollection<SubQuestion> subQuestions);
        void deleteDeleted();
    }
}
