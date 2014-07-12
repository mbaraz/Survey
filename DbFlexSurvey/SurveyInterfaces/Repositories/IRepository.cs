using System;
using System.Collections.Generic;

namespace SurveyInterfaces.Repositories
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        void Add(T obj);
        void Edit(T obj);
        void Delete(T obj);
        T GetById(int id);
        T Find(Func<T, bool> predicate);
        void AddRange(IEnumerable<T> range);
        void DeleteRange(ICollection<T> tagValues);
    }
}
