using System.Linq;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;

namespace SurveyDomain.Repositories
{
    public class SurveyInvitationRepository : RepositoryBase<SurveyInvitation>, ISurveyInvitationRepository
    {
        public SurveyInvitationRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public SurveyInvitation GetById(int id)
        {
            return DataSet.Single(invitation => invitation.SurveyInvitationId == id);
        }
    }
}
