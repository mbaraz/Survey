using System.Collections.Generic;
using SurveyModel;

namespace SurveyInterfaces.Repositories
{
    public interface ITagValueRepository : IRepository<TagValue>
    {
        IEnumerable<TagValue> GetByTagId(int tagId);
    }
}
