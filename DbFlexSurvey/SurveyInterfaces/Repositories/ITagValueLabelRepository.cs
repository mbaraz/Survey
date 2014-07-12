using System.Collections.Generic;
using SurveyModel;

namespace SurveyInterfaces.Repositories
{
    public interface ITagValueLabelRepository : IRepository<TagValueLabel>
    {
        IEnumerable<int> deletedIds { set; }
        void deleteDeleted();
        void removeFromDeleted(int tagId);
        IEnumerable<TagValueLabel> makeTagValues(SurveyQuestion question, int? subBoundTagId = null);
    }
}
