using System;
using SurveyModel;

namespace SurveyWeb.Models
{
    public class AnswerVariantModel
    {
        public int AnswerVariantId { get; set; }
        public int SurveyQuestionId { get; set; }
        public SurveyQuestion SurveyQuestion { get; set; }
        public string AnswerText { get; set; }
        public bool IsOpenAnswer { get; set; }
        public int AnswerCode { get; set; }
        public int AnswerOrder { get; set; }
        public int? TagValue { get; set; }

        public int SymbolCount  { get; set; }
        public bool IsNumeric { get; set; }

        public AnswerVariant Variant { 
            get
            {
                AnswerVariant result = new AnswerVariant();
                fillAnswerVariant(result);
                if (IsOpenAnswer)
                    result.AnswerText += AnswerVariant.TextPartsDelimiter + SymbolCount.ToString() + AnswerVariant.TextPartsDelimiter + IsNumeric.ToString();
                return result;
            }
        }

        public AnswerVariantModel() {}

        public AnswerVariantModel(AnswerVariant answerVariant) 
        {
            cloneToModel(answerVariant);
        }

        private void cloneToModel(AnswerVariant av)
        {
            fillModelVariant(av);
            AnswerText = av.InstantText;
            SymbolCount = Convert.ToInt32(av.SymbolCount);
            IsNumeric = av.IsNumeric.HasValue && (av.IsOpenAnswer && av.IsNumeric.Value);
        }

        private void fillModelVariant(AnswerVariant avFrom)
        {
            AnswerCode = avFrom.AnswerCode;
            AnswerVariantId = avFrom.AnswerVariantId;
            SurveyQuestionId = avFrom.SurveyQuestionId;
            SurveyQuestion = avFrom.SurveyQuestion;
            AnswerOrder = avFrom.AnswerOrder;
            TagValue = avFrom.TagValue;
            IsOpenAnswer = avFrom.IsOpenAnswer;
        }

        private void fillAnswerVariant(AnswerVariant avTo)
        {
            avTo.AnswerCode = AnswerCode;
            avTo.AnswerVariantId = AnswerVariantId;
            avTo.SurveyQuestionId = SurveyQuestionId;
            avTo.SurveyQuestion = SurveyQuestion;
            avTo.AnswerOrder = AnswerOrder;
            avTo.TagValue = TagValue;
            avTo.IsOpenAnswer = IsOpenAnswer;
            avTo.AnswerText = AnswerText;
        }
    }
}