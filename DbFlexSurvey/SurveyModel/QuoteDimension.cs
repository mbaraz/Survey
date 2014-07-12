using System.Collections.Generic;

namespace SurveyModel
{
    public class QuoteDimension
    {
        public int QuoteDimensionId { get; set; }

        public virtual ICollection<QuoteDimensionValue> Values { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }

        public int QuoteId { get; set; }
        public Quote Quote { get; set; }
    }
}