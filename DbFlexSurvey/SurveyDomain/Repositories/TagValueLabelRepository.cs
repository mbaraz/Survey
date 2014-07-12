using System;
using System.Collections.Generic;
using System.Linq;
using SurveyCommon;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;

namespace SurveyDomain.Repositories
{
    public class TagValueLabelRepository : RepositoryBase<TagValueLabel>, ITagValueLabelRepository
    {

        private IEnumerable<int> _deletedIds;

        public IEnumerable<int> deletedIds { set { _deletedIds = GetLabelsForTags(value).Select(label => label.TagValueLabelId); } }
        
        public TagValueLabelRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public TagValueLabel GetById(int id)
        {
            return DataSet.Single(label => label.TagValueLabelId == id);
        }

        public IEnumerable<TagValueLabel> makeTagValues(SurveyQuestion question, int ? subBoundTagId = null)
        {
            if (question.BoundTagId.HasValue || subBoundTagId.HasValue)
                return updateTagValues(question.AnswerVariants, subBoundTagId.HasValue ? subBoundTagId.Value : question.BoundTagId.Value);

            return replaceTagValues(question.AnswerVariants);
        }

        public void deleteDeleted()
        {
            DeleteRange(GetItemsByIds(_deletedIds));
        }

        public void removeFromDeleted(int tagId)
        {
            _deletedIds = _deletedIds.Except(GetLabelsForTag(tagId).Select(label => label.TagValueLabelId));
        }

        private IEnumerable<TagValueLabel> GetLabelsForTag(int tagId)
        {
            return DataSet.Where(label => label.TagId == tagId).ToArray();
        }

        private IEnumerable<TagValueLabel> GetLabelsForTags(IEnumerable<int> tagIds)
        {
            return DataSet.Where(label => tagIds.Contains(label.TagId)).ToArray();
        }

        private ICollection<TagValueLabel> GetItemsByIds(IEnumerable<int> lblIds)
        {
            return DataSet.Where(label => lblIds.Contains(label.TagValueLabelId)).ToList();
        }

        private IEnumerable<TagValueLabel> updateTagValues(IEnumerable<AnswerVariant> answerVariants, int tagId)
        {
            var dbTagValues = GetLabelsForTag(tagId);
            int cnt = 0;
            foreach (var tagValue in dbTagValues) {
                if (cnt == answerVariants.Count())
                    break;

                var variant = answerVariants.ElementAt(cnt++);
                tagValue.updateValueLabel(variant.AnswerCode, variant.InstantText);
            }
            if (answerVariants.Count() > cnt) {
                var av = answerVariants.Where((variant, index) => index >= cnt);
                var tagValues = replaceTagValues(av.ToList(), tagId);
                return dbTagValues.Concat(tagValues);
            }
            if (dbTagValues.Count() > cnt)
                addToDeletedIds(dbTagValues.Where((label, index) => index >= cnt));
 
            return dbTagValues.Take(cnt);
        }

        private IEnumerable<TagValueLabel> replaceTagValues(IEnumerable<AnswerVariant> answerVariants, int? tagId = null)
        {
            var length = Math.Min(answerVariants.Count(), _deletedIds.Count());
            var result = new TagValueLabel[length].ToList();
            for (var i = 0; i < length; i++) {
                var tagValue = GetById(_deletedIds.ElementAt(i));
                var variant = answerVariants.ElementAt(i);
                tagValue.updateValueLabel(variant.AnswerCode, variant.InstantText, tagId);
                result[i] = tagValue;
            }
            _deletedIds = _deletedIds.removeLefts(length);
            if (answerVariants.Count() != length) {
                var tagValues = createTagValues(answerVariants.Where((variant, index) => index >= length).ToList(), tagId);
                return result.Concat(tagValues);
            }
            return result;
        }

        private IEnumerable<TagValueLabel> createTagValues(ICollection<AnswerVariant> answerVariants, int? tagId)
        {
            var result = new TagValueLabel[answerVariants.Count];
            var indx = 0;
            foreach (var variant in answerVariants) {
                var tagValue = new TagValueLabel();
                tagValue.updateValueLabel(variant.AnswerCode, variant.InstantText, tagId);
                result[indx++] = tagValue;
            }
            AddRange(result);
            return result;
        }

        private void addToDeletedIds(IEnumerable<TagValueLabel> tagValues)
        {
            var ids = tagValues.Select(label => label.TagValueLabelId);
            _deletedIds = _deletedIds.Concat(ids);
        }
    }
}
