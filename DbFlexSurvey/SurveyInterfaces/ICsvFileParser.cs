using System.Collections.Generic;

namespace SurveyInterfaces
{
    public interface ICsvFileParser<out T> where T : class
    {
        IEnumerable<T> Parse();
    }
}