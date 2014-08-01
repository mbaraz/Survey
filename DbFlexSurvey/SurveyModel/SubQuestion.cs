using System.Collections.Generic;
using System.Linq;

namespace SurveyModel
{
    public class SubQuestion
    {
        public int SubQuestionId { get; set; }
        public int SurveyQuestionId { get; set; }
        public virtual SurveyQuestion SurveyQuestion { get; set; }
        public string QuestionText { get; set; }
        public int SubOrder { get; set; }

        public int? BoundTagId { get; set; }
        public virtual Tag BoundTag { get; set; }

        public string ConditionString { get; set; }

        public int? FilterAnswersTagId { get; set; }
        public virtual Tag FilterAnswersTag { get; set; }


        public void copyFrom(SubQuestion subQuestion)
        {
            QuestionText = subQuestion.QuestionText;
            SubOrder = subQuestion.SubOrder;
            BoundTagId = subQuestion.BoundTagId;
            ConditionString = subQuestion.ConditionString;
        }

        public void updateBoundTag(Tag tag, IEnumerable<TagValueLabel> tagValueLabels, string tagName)
        {
            BoundTag = tag;
            BoundTag.TagName = tagName;
            BoundTag.ValueLabels = tagValueLabels.ToList();
        }

    }
}
