using System.Collections.Generic;
using System.Linq;
using SurveyCommon;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;

namespace SurveyDomain.Repositories
{
    public class SubQuestionRepository : RepositoryBase<SubQuestion>, ISubQuestionRepository
    {
        private IEnumerable<int> _deletedIds;

        public IEnumerable<int> deletedIds { set { _deletedIds = value; } }

        public SubQuestionRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public SubQuestion GetById(int id)
        {
            return DataSet.Single(sq => sq.SubQuestionId == id);
        }

        public void deleteDeleted()
        {
            DeleteRange(GetItemsByIds(_deletedIds));
        }

        public void save(SurveyQuestion question, ICollection<SubQuestion> subQuestions)
        {
            foreach (var subQuestion in subQuestions)
                if (subQuestion.SubQuestionId == 0)
                    addSubQuestion(question, subQuestion);
                else
                    Edit(subQuestion);

            question.SubQuestions = question.SubQuestions.Take(subQuestions.Count).ToList();
        }

        private void addSubQuestion(SurveyQuestion question, SubQuestion subQuestion)
        {
            if (subQuestion.SubQuestionId == 0)
                if (_deletedIds.Any()) {
                    var deletedSubQuestion = GetById(_deletedIds.ElementAt(0));
                    deletedSubQuestion.copyFrom(subQuestion);
                    subQuestion = deletedSubQuestion;
                    _deletedIds = _deletedIds.removeLefts();
                }

            if (subQuestion.SubQuestionId == 0)
                Add(subQuestion);
//                question.SubQuestions.Add(subQuestion);   //    ????
            else
                Edit(subQuestion);

            if (question.SurveyQuestionId != 0 && subQuestion.SurveyQuestionId == question.SurveyQuestionId)
                return;

            subQuestion.SurveyQuestion = question;

// ????           question.AnswerVariants.Add(variant);
        }

        private ICollection<SubQuestion> GetItemsByIds(IEnumerable<int> ids)
        {
            return DataSet.Where(item => ids.Contains(item.SubQuestionId)).ToList();
        }
    }
}
