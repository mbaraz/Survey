using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SurveyInterfaces;
using System.Data.Entity;

namespace SurveyDomain
{
    public class RepositoryBase<T>  where T : class
    {
        protected readonly DBSurveyRepository _context;

        protected RepositoryBase(IUnitOfWork unitOfWork)
        {
            _context = unitOfWork as DBSurveyRepository;
            if (_context == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }
        }

        protected DbSet<T> DataSet
        {
            get { return _context.Set<T>(); }
        }

        public void Add(T obj)
        {
            _context.Set<T>().Add(obj);
        }

        public virtual void Delete(T obj)
        {
            _context.Set<T>().Remove(obj);
        }

        public void Edit(T obj)
        {
            _context.Entry(obj).State = EntityState.Modified;
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public T Find(Func<T, bool> predicate)
        {
            return DataSet.SingleOrDefault(predicate);
        }

        public void AddRange(IEnumerable<T> enumerable)
        {
            var autodetect = _context.Configuration.AutoDetectChangesEnabled;
            try {
                _context.Configuration.AutoDetectChangesEnabled = false;
                foreach (var course in enumerable)
                    Add(course);
            } finally {
                _context.Configuration.AutoDetectChangesEnabled = autodetect;
            }
            
        }

        public void DeleteRange(ICollection<T> tagValues)
        {
            foreach (var tagValue in tagValues)
                Delete(tagValue);
        }
    }
}