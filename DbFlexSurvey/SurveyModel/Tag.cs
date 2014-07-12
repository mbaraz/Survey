using System.Collections.Generic;

namespace SurveyModel
{
    public class Tag
    {
        public int TagId { get; set; }
        
        public int? SurveyProjectId { get; set; } // Для тегов, локальных для проекта
        public virtual SurveyProject SurveyProject { get; set; }

        public string TagName { get; set; }
        public bool IsAgeTag { get; set; }

        public int BoundedQuestionOrder { get { int result; return int.TryParse(orderString, out result) ? result : int.MaxValue; } }

        public bool IsGlobal { get { return SurveyProjectId == null; } }

        public virtual ICollection<TagValueLabel> ValueLabels { get; set; }

        private string orderString { get { return TagName.Substring(1); } }

        public static Tag Create(string tagName, int surveyProjectId, IEnumerable<TagValueLabel> tagValueLabels)
        {
            var tag = new Tag {
                              TagName = tagName,
                              SurveyProjectId = surveyProjectId,
                              ValueLabels = new List<TagValueLabel>()
                          };

            foreach (var tagValueLabel in tagValueLabels) {
                tagValueLabel.Tag = tag;
                tag.ValueLabels.Add(tagValueLabel);
            }
            return tag;
        }
    }
}
