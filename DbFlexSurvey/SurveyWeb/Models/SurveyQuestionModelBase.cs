using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace SurveyWeb.Models
{
    public class SurveyQuestionModelBase
    {
       
        [Display(Name = "�������������")]
        public bool Multiple { get; set; }

        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public ICollection<SelectListItem> Tags { get; set; }

        [Display(Name = "�������� ������ ���������� �������� ����������")]
        public int? BoundTagId { get; set; }

        public IEnumerable<SelectListItem> BoundTags
        {
            get
            {
                yield return new SelectListItem
                                 {
                                     Text = " � �� �������������",
                                     Value = "",
                                     Selected = true
                                 };
                yield return new SelectListItem
                                 {
                                     Text = " � ����� ����������",
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

        [Display(Name = "�������� ������ ������ ���� ����������...")]
        public int? ConditionalTagId { get; set; }

        public IEnumerable<SelectListItem> ConditionTags
        {
            get
            {
                yield return new SelectListItem
                                 {
                                     Text = " � ������ ��������",
                                     Value = "",
                                     Selected = true
                                 };
                foreach (var tag in TagCopy())
                {
                    yield return tag;
                }
            }
        }

        [Display(Name = "...�����")]
        public int? ConditionalValue { get; set; }

        [Display(Name = "����������� �������� ������� �� ����������...")]
        public int? FilterAnswersTagId { get; set; }

        public IEnumerable<SelectListItem> FilterAnswersTags
        {
            get
            {
                yield return new SelectListItem
                {
                    Text = " � �� �����������",
                    Value = "",
                    Selected = true
                };
                foreach (var tag in TagCopy())
                {
                    yield return tag;
                }
            }
        }

        [Display(Name = "�� ����� ��� � ������� (��� �������������� ������)")]
        public int? MinAnswers { get; set; }

        [Display(Name = "�� ����� ��� � ������� (��� �������������� ������)")]
        public int? MaxAnswers { get; set; }

        [Display(Name = "������� ������ ����� �� 1 �� � (��� �������������� ������)")]
        public int? MaxRank { get; set; }
    }
}