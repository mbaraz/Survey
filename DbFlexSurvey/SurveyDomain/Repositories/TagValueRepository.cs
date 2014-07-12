using System.Collections.Generic;
using System.Linq;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;

namespace SurveyDomain.Repositories
{
    public class TagValueRepository : RepositoryBase<TagValue>, ITagValueRepository
    {
        public TagValueRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public TagValue GetById(int id)
        {
            return DataSet.Single(value => value.TagValueId == id);
        }

        public IEnumerable<TagValue> GetByTagId(int tagId)
        {
            return DataSet.Where(tv => tv.TagId == tagId);
        }
    }
}
