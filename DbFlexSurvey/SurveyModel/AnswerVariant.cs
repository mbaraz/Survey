using System.Collections.Generic;
using System.Linq;

namespace SurveyModel
{
    public class AnswerVariant
    {
        public const int DefaultAnswerFieldSize = 60;
        public const char TextPartsDelimiter = '#';
        
        public int AnswerVariantId { get; set; }
        
        public int SurveyQuestionId { get; set; }
        public virtual SurveyQuestion SurveyQuestion { get; set; }

        public string AnswerText { get; set; }
        public bool IsOpenAnswer { get; set; }

        public bool IsExcludingAnswer { get; set; }
		public int SymbolCount  { get; set; }
		public bool IsNumeric { get; set; }
        public bool IsUnmoved { get; set; }

        public int AnswerCode { get; set; }
        public int AnswerOrder { get; set; }
        public int? TagValue { get; set; }

        public string InstantText { get { return (AnswerText == null) ? AnswerText : AnswerText.Split(TextPartsDelimiter)[0]; } }
/*
        public int SymbolCount
        {
            get {
                if (!IsOpenAnswer || AnswerText == null)
                    return DefaultAnswerFieldSize;

                string[] arr = AnswerText.Split(TextPartsDelimiter);
                return arr.Length < 2 || arr[1] == "" ? DefaultAnswerFieldSize : int.Parse(arr[1]);
            } 
        }

        public bool IsNumeric
        {
            get {
                if (!IsOpenAnswer || AnswerText == null)
                    return false;

                string[] arr = AnswerText.Split(TextPartsDelimiter);
                return arr.Length > 2 && arr[2].ToLower() == "true";
            }
        }
*/
        private static AnswerVariant Create(string answer)
        {
            answer = answer.Trim();

            var answerVariant = new AnswerVariant();

            if (answer.Contains("__")) {
                answerVariant.AnswerText = answer.Replace("_", "");
                answerVariant.IsOpenAnswer = true;
            } else
                answerVariant.AnswerText = answer;

            return answerVariant;
        }

        public static AnswerVariant[] Create(IEnumerable<string> answerVars)
        { 
            return answerVars.Select(Create).ToArray();
        }

        public static AnswerVariant[] Create(params string[] answerVars)
        {
            return answerVars.Select(Create).ToArray();
        }

        public void copyFrom(AnswerVariant variant)
        {
            AnswerOrder = variant.AnswerOrder;
            IsOpenAnswer = variant.IsOpenAnswer;
            AnswerText = variant.AnswerText;
            AnswerCode = variant.AnswerCode;
            TagValue = variant.TagValue;
            IsExcludingAnswer = variant.IsExcludingAnswer;
		    SymbolCount = variant.SymbolCount;
		    IsNumeric = variant.IsNumeric;
            IsUnmoved = variant.IsUnmoved;
        }
    }
}
