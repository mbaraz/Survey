using System.Collections.Generic;

namespace SurveyModel
{
    public class Quote
    {
        public int QuoteId { get; set; }

        public virtual ICollection<QuoteDimension> Dimensions { get; set; }

        public int SurveyProjectId { get; set; }
        public SurveyProject SurveyProject { get; set; }
    }
}