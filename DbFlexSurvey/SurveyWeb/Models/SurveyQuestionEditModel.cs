using System.ComponentModel.DataAnnotations;

namespace SurveyWeb.Models
{
    public class SurveyQuestionEditModel : SurveyQuestionModelBase
    {
        public int SurveyQuestionId { get; set; }

        [Display(Name = "Текст вопроса")]
        [DataType(DataType.MultilineText)]
        public string QuestionText { get; set; }
    }
}