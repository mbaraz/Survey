using System.Collections.Generic;
using SurveyModel;

namespace SurveyInterfaces.Repositories
{
    public interface ISurveyQuestionRepository : IRepository<SurveyQuestion>
    {
        IEnumerable<int> deletedIds { set; }
        ICollection<SurveyQuestion> GetQuestionsBoundToTag(int tagId);
        ICollection<SurveyQuestion> GetQuestionsConditionalOnTag(int tagId);
        ICollection<SurveyQuestion> GetFilteredQuestionsOnTag(int tagId);
        SurveyQuestion GetQuestion(int surveyProjectId, int order);
        SurveyQuestion GetQuestionAfter(int surveyProjectId, int order, ICollection<int?> unknown);
        SurveyQuestion GetQuestionAfterFx(int surveyProjectId, int order, ICollection<int?> unknown, ICollection<int> tagIds);
        ICollection<SurveyQuestion> GetByProject(int project);
        void save(ref SurveyQuestion question);
        void deleteDeleted();
    }
}
