using System.Collections.Generic;
using System.Linq;
using SurveyCommon;

namespace SurveyModel.Logic
{
    public class TagCondition
    {
        internal IEnumerable<int> TagIds { get { return expressionsArray.Select(ex => ex.TagId); } }

        private readonly IEnumerable<LogicalExpression> expressionsArray;

        internal TagCondition(string conditionString) {
            var conditionsArray = (LogicalUtil.Syndetics[0] + " " + conditionString).ConditionsArray();
            expressionsArray = conditionsArray.Select(cn => new LogicalExpression(cn));
		}

        internal bool check(IEnumerable<TagValue> tagValues) {
            var result = false;
            foreach (var expression in expressionsArray) {
                var responseArray = tagValues.Where(tv => tv.TagId == expression.TagId).OrderBy(tv => tv.Value).Select(tv => tv.Value.Value).ToArray();
                result = expression.check(responseArray, result);
			}
            return result;
		}
    }
}