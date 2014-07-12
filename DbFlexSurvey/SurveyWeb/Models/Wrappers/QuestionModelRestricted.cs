using System.Collections.Generic;

namespace SurveyWeb.Models.Wrappers
{
    public class QuestionModelRestricted
    {
        public SurveyQuestionRestricted Question { get; set; }
        public ICollection<SubQuestionRestricted> SubQuestions { get; set; }
        public ICollection<AnswerVariantRestricted> AnswerVariants { get; set; }
    }
}