﻿using SurveyModel;

namespace SurveyWeb.Models
{
    public class AnswerVariantRestricted
    {
        public int AnswerVariantId { get; set; }
        public int SurveyQuestionId { get; set; }
        public string AnswerText { get; set; }
        public bool IsOpenAnswer { get; set; }
        public int AnswerCode { get; set; }
        public int AnswerOrder { get; set; }
        public int? TagValue { get; set; }

        public int SymbolCount  { get; set; }
        public bool IsNumeric { get; set; }
        public bool IsExcludingAnswer { get; set; }
        public bool IsUnmoved { get; set; }

        public AnswerVariantRestricted(AnswerVariant answerVariant) 
        {
            cloneToRestricted(answerVariant);
        }

        private void cloneToRestricted(AnswerVariant variant)
        {
            fillModelVariant(variant);
            AnswerText = variant.InstantText;
            SymbolCount = variant.SymbolCount;
            IsNumeric = variant.IsOpenAnswer && variant.IsNumeric;
        }

        private void fillModelVariant(AnswerVariant variant)
        {
            AnswerCode = variant.AnswerCode;
            AnswerVariantId = variant.AnswerVariantId;
            SurveyQuestionId = variant.SurveyQuestionId;
            AnswerOrder = variant.AnswerOrder;
            TagValue = variant.TagValue;
            IsOpenAnswer = variant.IsOpenAnswer;
            IsExcludingAnswer = variant.IsExcludingAnswer;
//            SymbolCount = variant.SymbolCount;
//            IsNumeric = variant.IsNumeric;
            IsUnmoved = variant.IsUnmoved;
        }
    }
}