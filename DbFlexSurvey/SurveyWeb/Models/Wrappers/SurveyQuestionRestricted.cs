using SurveyModel;

namespace SurveyWeb.Models
{
    public class SurveyQuestionRestricted
    {
        public int SurveyQuestionId { get; private set; }
        public string QuestionName { get; private set; }
        public int? QuestionType { get; private set; }
        public string QuestionText { get; private set; }
        public bool MultipleAnswerAllowed { get; private set; }
        public int? AnswerOrdering { get; private set; }
        public int? MaxAnswers { get; private set; }
        public int? MinAnswers { get; private set; }
        public int? MaxRank { get; private set; }
        public int? MinRank { get; private set; }
        public int QuestionOrder { get; private set; }
        public int? BoundTagId { get; private set; }
        public int? ConditionOnTagId { get; private set; }
        public int? ConditionOnTagValue { get; private set; }
        public string ConditionString { get; private set; }
        public int? FilterAnswersTagId { get; private set; }

        public bool IsRankQuestion { get; private set; }
        public bool IsGridQuestion { get; private set; }

        public SurveyQuestionRestricted(SurveyQuestion surveyQuestion) 
        {
            cloneToRestricted(surveyQuestion);
//            QuestionText = surveyQuestion.QuestionText;
        }
/*
        public SurveyQuestionRestricted(SurveyQuestion surveyQuestion, string subitems)
        {
            cloneToRestricted(surveyQuestion);
            QuestionText = QuestionType == null ? surveyQuestion.InstantText + subitems;
        }
*/
        private void cloneToRestricted(SurveyQuestion question)
        {
            SurveyQuestionId = question.SurveyQuestionId;
            QuestionName = question.QuestionName;
            QuestionType = question.QuestionType;
            QuestionText = question.InstantText;
            MultipleAnswerAllowed = question.MultipleAnswerAllowed;
            AnswerOrdering = question.AnswerOrdering;
            MaxAnswers = question.MaxAnswers;
            MinAnswers = question.MinAnswers;
            MaxRank = question.MaxRank;
            MinRank = question.MinRank;
            QuestionOrder = question.QuestionOrder;
            BoundTagId = question.BoundTagId;
            ConditionOnTagId = question.ConditionOnTagId;
            ConditionOnTagValue = question.ConditionOnTagValue;
            ConditionString = question.ConditionString;
            FilterAnswersTagId = question.FilterAnswersTagId;
            IsRankQuestion = question.IsRankQuestion;
            IsGridQuestion = question.IsGridQuestion;
        }
    }
}