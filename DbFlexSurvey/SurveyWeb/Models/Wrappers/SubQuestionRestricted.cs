using SurveyModel;

namespace SurveyWeb.Models.Wrappers
{
    public class SubQuestionRestricted
    {
        public int SubQuestionId { get; private set; }
        public int SurveyQuestionId { get; private set; }
//        public string QuestionName { get; private set; }
        public string QuestionText { get; private set; }
        public int SubOrder { get; private set; }
        public int? BoundTagId { get; private set; }
        public string ConditionString { get; private set; }
        public int? FilterAnswersTagId { get; private set; }

        public SubQuestionRestricted(SubQuestion subQuestion) 
        {
            cloneToRestricted(subQuestion);
            QuestionText = subQuestion.QuestionText;
        }

        private void cloneToRestricted(SubQuestion subQuestion)
        {
            SubQuestionId = subQuestion.SubQuestionId;
            SurveyQuestionId = subQuestion.SurveyQuestionId;
//            QuestionName = subQuestion.QuestionName;
            QuestionText = subQuestion.QuestionText;
            SubOrder = subQuestion.SubOrder;
            BoundTagId = subQuestion.BoundTagId;
            ConditionString = subQuestion.ConditionString;
            FilterAnswersTagId = subQuestion.FilterAnswersTagId;
        }
    }
}