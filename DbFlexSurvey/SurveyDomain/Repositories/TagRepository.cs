using System.Collections.Generic;
using System.Linq;
using SurveyCommon;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;

namespace SurveyDomain.Repositories
{
    public class TagRepository : RepositoryBase<Tag>, ITagRepository
    {
        private readonly ISurveyQuestionRepository _surveyQuestionRepository;
        private readonly ITagValueLabelRepository _tagValueLabelRepository;

        private Dictionary<int, Tag> _unsavedTags;
        private IEnumerable<int> _deletedIds;

        public IEnumerable<int> deletedIds
        {
            set {
                _deletedIds = value;
                _tagValueLabelRepository.deletedIds = value;
                _unsavedTags = new Dictionary<int, Tag>();
            }
        }

        public TagRepository(IUnitOfWork unitOfWork, ISurveyQuestionRepository surveyQuestionRepository, ITagValueLabelRepository tagValueLabelRepository) : base(unitOfWork)
        {
            _surveyQuestionRepository = surveyQuestionRepository;
            _tagValueLabelRepository = tagValueLabelRepository;
        }

        public IEnumerable<Tag> GetTagsForProject(int projectId)
        {
            return GetAll().Where(tag => tag.SurveyProjectId == projectId || tag.SurveyProjectId == null);
        }

        public Tag GetById(int id)
        {
            return DataSet.Single(tag => tag.TagId == id);
        }

        public override void Delete(Tag obj)
        {
            base.Delete(obj);
            foreach (var surveyQuestion in _surveyQuestionRepository.GetQuestionsBoundToTag(obj.TagId))
                surveyQuestion.BoundTag = null;

            foreach (var surveyQuestion in _surveyQuestionRepository.GetQuestionsConditionalOnTag(obj.TagId)) {
                surveyQuestion.ConditionOnTag = null;
                surveyQuestion.ConditionOnTagValue = null;
            }
        }

        public void Delete(int id)
        {
            base.Delete(GetById(id));
        }

        public void deleteDeleted()
        {
            _tagValueLabelRepository.deleteDeleted();
            DeleteRange(GetItemsByIds(_deletedIds));
        }

        public void save(SurveyQuestion question, ICollection<SubQuestion> subQuestions)
        {
            save(question);
            foreach (var subQuestion in subQuestions)
                saveSub(question, subQuestion);
        }

        private void save(SurveyQuestion question)
        {
            updateConditionAndFilterIds(question);
            if (!question.BoundTagId.HasValue)
                return;

            if (question.BoundTagId < 0 && _deletedIds.Any()) {
                updateBoundTag(ref question, GetById(_deletedIds.ElementAt(0)));
                _deletedIds = _deletedIds.removeLefts();
            }
            Edit(question);
        }

        private void Edit(SurveyQuestion question)
        {
            var tagValues = _tagValueLabelRepository.makeTagValues(question);
            if (question.BoundTagId > 0)
                editTag(question, tagValues);
            else
                addTag(question, tagValues);
        }

        private void editTag(SurveyQuestion question, IEnumerable<TagValueLabel> tagValueLabels)
        {
            question.updateBoundTag(GetById(question.BoundTagId.Value), tagValueLabels);
            Edit(question.BoundTag);
        }

        private void addTag(SurveyQuestion question, IEnumerable<TagValueLabel> tagValueLabels)
        {
            var tag = Tag.Create(question.QuestionName, question.SurveyProjectId, tagValueLabels);
            Add(tag);
            updateBoundTag(ref question, tag);
        }

        private ICollection<Tag> GetItemsByIds(IEnumerable<int> ids)
        {
            return DataSet.Where(item => ids.Contains(item.TagId)).ToList();
        }

        private void updateConditionAndFilterIds(SurveyQuestion question)
        {
            if (question.ConditionOnTagId < 0)
                question.ConditionOnTag = _unsavedTags[question.ConditionOnTagId.Value];
//                question.ConditionOnTagId = _unsavedTags[question.ConditionOnTagId.Value];   // ????
            if (question.FilterAnswersTagId < 0)
                question.FilterAnswersTag = _unsavedTags[question.FilterAnswersTagId.Value];
//                question.FilterAnswersTagId = _unsavedTags[question.FilterAnswersTagId.Value];   // ????

        }

        private void updateBoundTag(ref SurveyQuestion question, Tag tag)
        {
            _unsavedTags[question.BoundTagId.Value] = tag;
            _tagValueLabelRepository.removeFromDeleted(tag.TagId);
            //            question.BoundTag = tag;
            if (tag.TagId != 0)
                question.BoundTagId = tag.TagId;
            else
                question.BoundTag = tag;
        }


        private void saveSub(SurveyQuestion question, SubQuestion subQuestion)
        {
// ?????            updateConditionAndFilterIds(subQuestion);
            if (!subQuestion.BoundTagId.HasValue)
                return;

            if (subQuestion.BoundTagId < 0 && _deletedIds.Any()) {
                updateBoundSubTag(ref subQuestion, GetById(_deletedIds.ElementAt(0)));
                _deletedIds = _deletedIds.removeLefts();
            }
            subEdit(question, subQuestion);
        }

        private void updateBoundSubTag(ref SubQuestion subQuestion, Tag tag)
        {
            _unsavedTags[subQuestion.BoundTagId.Value] = tag;
            _tagValueLabelRepository.removeFromDeleted(tag.TagId);
//            question.BoundTag = tag;
            if (tag.TagId != 0)
                subQuestion.BoundTagId = tag.TagId;
            else
                subQuestion.BoundTag = tag;
        }

        private void subEdit(SurveyQuestion question, SubQuestion subQuestion)
        {
            var tagValues = _tagValueLabelRepository.makeTagValues(question, subQuestion.BoundTagId);
            if (subQuestion.BoundTagId > 0)
                editSubTag(question, subQuestion, tagValues);
            else
                addSubTag(question, subQuestion, tagValues);
        }

        private void editSubTag(SurveyQuestion question, SubQuestion subQuestion, IEnumerable<TagValueLabel> tagValueLabels)
        {
            subQuestion.updateBoundTag(GetById(subQuestion.BoundTagId.Value), tagValueLabels, question.QuestionName + "." + subQuestion.SubOrder);
            Edit(subQuestion.BoundTag);
        }

        private void addSubTag(SurveyQuestion question, SubQuestion subQuestion, IEnumerable<TagValueLabel> tagValueLabels)
        {
            var tag = Tag.Create(question.QuestionName + "." + subQuestion.SubOrder, question.SurveyProjectId, tagValueLabels);
            Add(tag);
            updateBoundSubTag(ref subQuestion, tag);
        }
    }
}
