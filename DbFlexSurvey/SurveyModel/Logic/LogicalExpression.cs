using System;

namespace SurveyModel.Logic
{
    class LogicalExpression
    {
        private readonly string syndetic;
        private readonly string sign;
        private readonly int value;
        private readonly int tagId;

        internal int TagId { get { return tagId; } }

        internal LogicalExpression(string[] arr)
        {
			syndetic = arr[0];
			tagId = Convert.ToInt32(arr[1]);
			sign = arr[2];
			value = Convert.ToInt32(arr[3]);
		}

        internal bool check(int[] codesArray, bool prevlResult)
        {
            var result = LogicalUtil.checkInequality(sign, codesArray, value);
            return LogicalUtil.checkSyndetic(syndetic, prevlResult, result);
        }
    }
}
