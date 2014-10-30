using System.Collections.Generic;
using SurveyModel;

namespace SurveyInterfaces.Repositories
{
    public interface ITagRepository : IRepository<Tag>
    {
        IEnumerable<int> deletedIds { set; }
        IEnumerable<Tag> GetTagsForProject(int projectId);
        void Delete(int id);
        void deleteDeleted();
        void save(SurveyQuestion question, ICollection<SubQuestion> subQuestions);
        void updateConditionString(SurveyQuestion question);
    }
}
