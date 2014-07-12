using System.Collections.Generic;
using System.Linq;
using SurveyCommon;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;

namespace SurveyDomain.Repositories
{
    public class AnswerVariantRepository : RepositoryBase<AnswerVariant>, IAnswerVariantRepository
    {
        private IEnumerable<int> _deletedIds;

        public IEnumerable<int> deletedIds { set { _deletedIds = value; } }

        public AnswerVariantRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public AnswerVariant GetById(int id)
        {
            return DataSet.Single(variant => variant.AnswerVariantId == id);
        }

        public void deleteDeleted()
        {
            DeleteRange(GetItemsByIds(_deletedIds));
        }

        public void save(SurveyQuestion question, ICollection<AnswerVariant> variants)
        {
            foreach (var variant in variants)
                if (variant.AnswerVariantId == 0)
                    addAnswer(question, variant);
                else
                    Edit(variant);
            question.AnswerVariants = question.AnswerVariants.Take(variants.Count).ToList();
        }

        private void addAnswer(SurveyQuestion question, AnswerVariant variant)
        {
            if (variant.AnswerVariantId == 0)
                if (_deletedIds.Any()) {
                    var deletedVariant = GetById(_deletedIds.ElementAt(0));
                    deletedVariant.copyFrom(variant);
                    variant = deletedVariant;
                    _deletedIds = _deletedIds.removeLefts();
                }

            if (variant.AnswerVariantId == 0) {
                Add(variant);
                question.AnswerVariants.Add(variant);   //    ????
            } else
                Edit(variant);

            if (question.SurveyQuestionId != 0 && variant.SurveyQuestionId == question.SurveyQuestionId)
                return;

            variant.SurveyQuestion = question;

// ????           question.AnswerVariants.Add(variant);
        }

        private ICollection<AnswerVariant> GetItemsByIds(IEnumerable<int> ids)
        {
            return DataSet.Where(item => ids.Contains(item.AnswerVariantId)).ToList();
        }
    }
}
