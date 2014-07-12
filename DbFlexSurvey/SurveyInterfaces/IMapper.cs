using System;

namespace SurveyInterfaces
{
    public interface IMapper<T> where T : class 
    {
        T FindOrCreateGroup(Func<T, bool> predicate, Func<T> creator );
    }
}