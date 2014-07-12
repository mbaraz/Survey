using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurveyModel
{
    public class QuoteDimensionValue
    {
        public QuoteDimensionValue()
        {
            Interviews = new HashSet<Interview>();
        }

        public int QuoteDimensionValueId { get; set; }

        public int QuoteDimensionId { get; set; }
        public virtual QuoteDimension QuoteDimension { get; set; }

        public int TagValueLabelId { get; set; }
        public virtual TagValueLabel TagValueLabel { get; set; }

        public int HardLimit { get; set; }
        public int SoftLimit { get; set; }

        public int CurrentValue { get; set; }

        public ICollection<Interview> Interviews { get; set; }
    }
}
