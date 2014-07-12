using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace SurveyWeb.Models
{
    public class SurveyQuestionModelBase
    {
       
        [Display(Name = "Множественный")]
        public bool Multiple { get; set; }

        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public ICollection<SelectListItem> Tags { get; set; }

        [Display(Name = "Согласно ответу установить значение переменной")]
        public int? BoundTagId { get; set; }

        public IEnumerable<SelectListItem> BoundTags
        {
            get
            {
                yield return new SelectListItem
                                 {
                                     Text = " — Не устанавливать",
                                     Value = "",
                                     Selected = true
                                 };
                yield return new SelectListItem
                                 {
                                     Text = " — Новая переменная",
                                     Value = "0"
                                 };
                foreach (var selectListItem in TagCopy())
                {
                    yield return selectListItem;
                }
            }
        }

        private IEnumerable<SelectListItem> TagCopy()
        {
            return Tags.Select(tag => new SelectListItem {Text = tag.Text, Value = tag.Value});
        }

        [Display(Name = "Задавать вопрос только если переменная...")]
        public int? ConditionalTagId { get; set; }

        public IEnumerable<SelectListItem> ConditionTags
        {
            get
            {
                yield return new SelectListItem
                                 {
                                     Text = " — всегда задавать",
                                     Value = "",
                                     Selected = true
                                 };
                foreach (var tag in TagCopy())
                {
                    yield return tag;
                }
            }
        }

        [Display(Name = "...равна")]
        public int? ConditionalValue { get; set; }

        [Display(Name = "Фильтровать варианты ответов по переменной...")]
        public int? FilterAnswersTagId { get; set; }

        public IEnumerable<SelectListItem> FilterAnswersTags
        {
            get
            {
                yield return new SelectListItem
                {
                    Text = " — не фильтровать",
                    Value = "",
                    Selected = true
                };
                foreach (var tag in TagCopy())
                {
                    yield return tag;
                }
            }
        }

        [Display(Name = "Не менее чем Х ответов (для множественного выбора)")]
        public int? MinAnswers { get; set; }

        [Display(Name = "Не более чем Х ответов (для множественного выбора)")]
        public int? MaxAnswers { get; set; }

        [Display(Name = "Оценить каждый пункт от 1 до Х (для множественного выбора)")]
        public int? MaxRank { get; set; }
    }
}