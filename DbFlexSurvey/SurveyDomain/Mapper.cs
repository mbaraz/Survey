using System;
using System.Collections.Generic;
using System.Linq;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;

namespace SurveyDomain
{
    internal class Mapper<T> : IMapper<T> where T : class
    {
        private readonly IRepository<T> _studentGroupRepository;
        private readonly List<T> _groups = new List<T>();

        public Mapper(IRepository<T> studentGroupRepository)
        {
            _studentGroupRepository = studentGroupRepository;
        }

        public T FindOrCreateGroup(Func<T, bool> predicate, Func<T> creator)
        {
            var group = _groups.SingleOrDefault(predicate);
            if (@group != null)
            {
                return @group;
            }
            var foundGroup = _studentGroupRepository.Find(predicate) ?? creator();
            _groups.Add(foundGroup);
            return foundGroup;
        }
    }
}