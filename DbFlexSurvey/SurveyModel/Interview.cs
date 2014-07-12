﻿using System.Collections.Generic;
using System.Linq;

namespace SurveyModel
{
    public class Interview
    {
        public int InterviewId { get; set; }
        public int RespondentId { get; set; }
        public virtual Respondent Respondent { get; set; }

        public int? InterviewerId { get; set; }
        public virtual Respondent Interviewer { get; set; }

        public bool TestInterview { get; set; }

        public int SurveyProjectId { get; set; }
        public virtual SurveyProject SurveyProject { get; set; }

        public virtual ICollection<InterviewAnswer> Answers { get; set; }
        public virtual ICollection<TagValue> TagValues { get; set; }
        public bool Completed { get; set; }

        public int GetLastQuestionOrder()
        {
            return Answers.Any() ? Answers.Max(answer => answer.SurveyQuestion.QuestionOrder) : int.MinValue;
        }

        public bool ShouldSkip(int order)
        {
            return ShouldSkip(SurveyProject.Questions.Single(question =>question.QuestionOrder == order));
        }

        public bool ShouldSkip(SurveyQuestion question)
        {
            return ShouldSkip(question.ConditionOnTagId, question.ConditionOnTagValue) || (question.FilterAnswersTagId != null && !GetFilteredAnswers(question).Any() || DispStudentSkip(question));
        }

        // Необходимо только для опроса студентов, чтобы пропускать вопросы о дисциплинах, которых либо не было, либо  вел другой преподаватель
        private bool DispStudentSkip(SurveyQuestion question)
        {
            if (!question.QuestionName.Contains("Disp") || Answers.Count == 0)
                return false;

            InterviewAnswer lastAnswer = Answers.ElementAt(Answers.Count - 1);
            string[] lastQuestionNames = lastAnswer.SurveyQuestion.QuestionName.Split('_');
            string[] currentQuestionNames = question.QuestionName.Split('_');
            if (lastQuestionNames.Length != 3 || currentQuestionNames.Length != 3 || lastQuestionNames[2] != "Attend")
                return false;

            return lastQuestionNames[1] == currentQuestionNames[1] && lastAnswer.Answers.ElementAt(0) > 4;
        }

        private bool ShouldSkip(int? tagId, int? requiredValue)
        {
            return tagId != null && TagValues.All(tagValue => tagValue.TagId != tagId || tagValue.Value != requiredValue);
        }

        public int? GetPrevQuestion(int? lastOrder)
        {
            do {
                lastOrder = SurveyProject.GetOrderBefore(lastOrder);
            } while (lastOrder != null && ShouldSkip((int)lastOrder));
            return lastOrder;
        }

        public ICollection<QuoteDimensionValue> QuoteDimensionValues { get; set; }

        public Interview()
        {
            QuoteDimensionValues = new HashSet<QuoteDimensionValue>();
        }

        public IEnumerable<AnswerVariant> GetFilteredAnswers(SurveyQuestion surveyQuestion)
        {
            return surveyQuestion.OrderedAnswerVariants.Where(variant => !ShouldSkip(surveyQuestion.FilterAnswersTagId, variant.AnswerCode));
        }

        public IEnumerable<AnswerVariant> GetFilteredAnswersFx(SurveyQuestion surveyQuestion, int startOrder)
        {
            if (notSkipForTestFx(surveyQuestion.FilterAnswersTagOrder, startOrder))
                return surveyQuestion.OrderedAnswerVariants;

            return surveyQuestion.OrderedAnswerVariants.Where(variant => !ShouldSkip(surveyQuestion.FilterAnswersTagId, variant.AnswerCode));
        }

        public IEnumerable<SubQuestion> GetFilteredSubitemsFx(SurveyQuestion surveyQuestion, int startOrder)
        {
            return surveyQuestion.IsOldFashioned ? GetFilteredSubitemsOld(surveyQuestion, startOrder) : surveyQuestion.SubQuestions;    // TEMP
        }

        public InterviewAnswer GetInterviewAnswer(SurveyQuestion surveyQuestion)
        {
            return Answers.SingleOrDefault(answer => answer.SurveyQuestionId == surveyQuestion.SurveyQuestionId);
        }

        public bool shouldSkipForTestFx(SurveyQuestion question, int startOrder)
        {
            return TestInterview && question.ConditionOnTagOrder >= startOrder && question.FilterAnswersTagOrder >= startOrder;
        }

/*
        public bool shouldSkipForTestFx(SurveyQuestion question)
        {
            if (question.ConditionOnTagId == null && question.FilterAnswersTagId == null)
                return true;

            return TagValues.Any(tagValue => tagValue.TagId == question.ConditionOnTagId || tagValue.TagId == question.FilterAnswersTagId); ;
        }

        private bool notSkipForTestFx(int? requiredValue)
        {
            var hasNoTagValues = requiredValue == null || TagValues.Count == 0 || TagValues.All(tagValue => tagValue.TagId != requiredValue);
            return TestInterview && hasNoTagValues;
        }
*/
        private IEnumerable<SubQuestion> GetFilteredSubitemsOld(SurveyQuestion surveyQuestion, int startOrder)
        {
            var result = new SubQuestion[surveyQuestion.SubitemsStrings.Count()];
            for (var i = 0; i < result.Count(); i++)
                if (notSkipForTestFx(surveyQuestion.FilterAnswersTagOrder, startOrder) || !ShouldSkip(surveyQuestion.FilterAnswersTagId, i + 1))
                    result[i] = new SubQuestion() {
                                                    SurveyQuestionId = surveyQuestion.SurveyQuestionId,
                                                    QuestionText = surveyQuestion.SubitemsStrings.ElementAt(i),
                                                    SubOrder = i + 1,
                                                  };

            return result;
        }
        
        private bool notSkipForTestFx(int filterTagOrder, int startOrder)
        {
            return TestInterview && filterTagOrder < startOrder;
        }
    }
}