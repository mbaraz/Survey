namespace SurveyModel
{
    public class TagValueLabel
    {
        public int TagValueLabelId { get; set; }

        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }

        public int ValueCode { get; set; }

        public string Label { get; set; }

        public void updateValueLabel(int code, string label, int? tagId = null)
        {
            ValueCode = code;
            Label = label;
            if (tagId.HasValue)
                TagId = tagId.Value;
        }
    }
}
