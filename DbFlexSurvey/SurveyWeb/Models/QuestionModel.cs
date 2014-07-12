using System.Collections.Generic;
using SurveyModel;

namespace SurveyWeb.Models
{
    public class QuestionModel
    {
        public SurveyQuestion Question { get; set; }
        public ICollection<SubQuestion> SubQuestions { get; set; }
        public ICollection<AnswerVariant> AnswerVariants { get; set; }
    }
}