using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SurveyModel;
using System.ComponentModel.DataAnnotations;

namespace SurveyWeb.Models
{
    public class SurveyQuestionCreateModel : SurveyQuestionModelBase
    {
        [Display(Name = "Вопрос будет располагаться")]
        public int QuestionOrder { get; set; }
        public IEnumerable<SelectListItem> Questions { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Введите текст вопроса. Первая строка — вопрос, остальные строки — варианты ответа.")]
        [Required(ErrorMessage = "Необходимо ввести текст вопроса")]
        public string QuestionText { get; set; }

    }
}