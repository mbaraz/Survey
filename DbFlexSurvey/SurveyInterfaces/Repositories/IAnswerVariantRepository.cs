using System.Collections.Generic;
using SurveyModel;

namespace SurveyInterfaces.Repositories
{
    public interface IAnswerVariantRepository : IRepository<AnswerVariant>
    {
        IEnumerable<int> deletedIds { set; }
        void save(SurveyQuestion question, ICollection<AnswerVariant> variants);
        void deleteDeleted();
    }
}
