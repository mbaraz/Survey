namespace SurveyModel
{
    public class TagValue
    {
        public int TagValueId { get; set; }
        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }
        public int InterviewId { get; set; }
        public virtual Interview Interview { get; set; }
        public int? Value { get; set; }
    }
}
