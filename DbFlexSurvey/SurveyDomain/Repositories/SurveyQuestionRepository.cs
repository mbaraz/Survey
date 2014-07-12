using System.Collections.Generic;
using System.Linq;
using SurveyCommon;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;

namespace SurveyDomain.Repositories
{
    public class SurveyQuestionRepository : RepositoryBase<SurveyQuestion>, ISurveyQuestionRepository
    {
        private IEnumerable<int> _deletedIds;

        public IEnumerable<int> deletedIds { set { _deletedIds = value; } }
        
        public SurveyQuestionRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public SurveyQuestion GetById(int id)
        {
            return DataSet.Single(question => question.SurveyQuestionId == id);
        }

        public ICollection<SurveyQuestion> GetQuestionsBoundToTag(int tagId)
        {
            return DataSet.Where(question => question.BoundTagId == tagId).ToArray();
        }

        public ICollection<SurveyQuestion> GetQuestionsConditionalOnTag(int tagId)
        {
            return DataSet.Where(question => question.ConditionOnTagId == tagId).ToArray();
        }

        public ICollection<SurveyQuestion> GetFilteredQuestionsOnTag(int tagId)
        {
            return DataSet.Where(question => question.FilterAnswersTagId == tagId).ToArray();
        }

        public SurveyQuestion GetQuestion(int surveyProjectId, int order)
        {
            return DataSet.SingleOrDefault(q => q.SurveyProjectId == surveyProjectId && q.QuestionOrder == order);
        }

        public SurveyQuestion GetQuestionAfter(int surveyProjectId, int order, ICollection<int?> tagValues)
        {
            return DataSet.Where(q => q.SurveyProjectId == surveyProjectId && q.QuestionOrder > order && (q.ConditionOnTagId == null || tagValues.Contains(q.ConditionOnTagValue))).OrderBy(
                    q => q.QuestionOrder).FirstOrDefault();
        }


        public SurveyQuestion GetQuestionAfterFx(int surveyProjectId, int order, ICollection<int?> tagValues, ICollection<int> tagIds)
        {
            return DataSet.Where(q => q.SurveyProjectId == surveyProjectId && q.QuestionOrder > order && (q.ConditionOnTagId == null || !tagIds.Contains(q.ConditionOnTagId.Value) || tagValues.Contains(q.ConditionOnTagValue))).OrderBy(
                    q => q.QuestionOrder).FirstOrDefault();
        }

        public ICollection<SurveyQuestion> GetByProject(int project)
        {
            return DataSet.Where(q => q.SurveyProjectId == project).ToArray();
        }

        public void save(ref SurveyQuestion question)
        {
            if (question.SurveyQuestionId == 0) {
                if (_deletedIds.Any()) {
                    var deletedQuestion = GetById(_deletedIds.ElementAt(0));
                    deletedQuestion.copyFrom(question);
                    question = deletedQuestion;
                    Edit(question);
                    _deletedIds = _deletedIds.removeLefts();
                } else
                    Add(question);

            } else
                Edit(question);
        }

        public void deleteDeleted()
        {
            DeleteRange(GetItemsByIds(_deletedIds));
        }

        private ICollection<SurveyQuestion> GetItemsByIds(IEnumerable<int> ids)
        {
            return DataSet.Where(item => ids.Contains(item.SurveyQuestionId)).ToList();
        }
    }
}
