using System;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;

namespace SurveyDomain
{
    public class UserService : IUserService
    {
        private readonly IRespondentRepository _respondentRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Random rnd = new Random();

        public UserService(IRespondentRepository respondentRepository, IUnitOfWork unitOfWork, ITicketRepository ticketRepository)
        {
            _respondentRepository = respondentRepository;
            _unitOfWork = unitOfWork;
            _ticketRepository = ticketRepository;
        }

        public Respondent AfterRegister(string userName, Guid inviteGuid, string email)
        {
            var respondent = _respondentRepository.GetByToken(inviteGuid) ?? CreateRespondent();
            respondent.MembershipUserName = userName;
            respondent.Token = null;
            respondent.RespondentEmail = email;
            var ticket = _ticketRepository.GetByToken(inviteGuid);
            if (ticket != null)
            {
                CopyInvitationsFromTicket(respondent, ticket);
            }
            return respondent;
        }

        private static void CopyInvitationsFromTicket(Respondent respondent, Ticket ticket)
        {
            foreach (var invite in ticket.Invitations)
            {
                respondent.Invitations.Add(new SurveyInvitation
                                               {
                                                   SurveyProjectId = invite.SurveyProjectId,
                                                   OriginTicketId = ticket.TicketId
                                               });
            }
        }

        private Respondent CreateRespondent()
        {
            var respondent = new Respondent();
            _respondentRepository.Add(respondent);
            return respondent;
        }

        public string GetDefaultUserName()
        {
            return string.Format("u{0}{1}", (_respondentRepository.GetCount() + 1), rnd.Next(0, 9));
        }

        public Respondent GetRespondentByName(string getUser)
        {
            var respondentByName = _respondentRepository.GetByName(getUser);
            if (respondentByName == null)
            {
                respondentByName = CreateRespondent();
                respondentByName.MembershipUserName = getUser;
                _unitOfWork.Save();
            }
            return respondentByName;
        }
    }
}
