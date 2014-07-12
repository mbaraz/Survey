using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SurveyModel;

namespace SurveyWeb.Models
{
    public class TagViewModel
    {
        public Tag Tag { get; set; }
        [UIHint("TagBoundQuestionList")]
        [Display(Name = "Переменная устанавливается по результатам следующих вопросов")]
        public ICollection<SurveyQuestion> BoundQuestions { get; set; }

        [UIHint("TagBoundQuestionList")]
        [Display(Name = "Вопросы, которые задаются под условием данной переменной")]
        public ICollection<SurveyQuestion> ConditionalQuestions { get; set; }

        [UIHint("TagBoundQuestionList")]
        [Display(Name = "Вопросы, ответы которых фильтруются данной переменной")]
        public ICollection<SurveyQuestion> FilteredQuestions { get; set; }
    }
}