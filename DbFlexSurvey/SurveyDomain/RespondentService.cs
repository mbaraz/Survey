using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mail;
using SurveyCommon;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;

namespace SurveyDomain
{
    public class RespondentService : IRespondentService
    {
        private const string ReplyMailAddress = "opros.spbu@spbu.ru";
        private readonly IRespondentRepository _respondentRepository;
        private readonly ISurveyInvitationRepository _surveyInvitationRepository;
        private readonly ITicketRepository _ticketRepository;

        public RespondentService(IRespondentRepository respondentRepository, ISurveyInvitationRepository surveyInvitationRepository, ITicketRepository ticketRepository)
        {
            _respondentRepository = respondentRepository;
            _surveyInvitationRepository = surveyInvitationRepository;
            _ticketRepository = ticketRepository;
        }

        public InviteResult InviteByEmail(int surveyProjectId, string emails)
        {
            var emailList = GetEmailList(emails);
            
            var respondents = GetOrCreateRespondentsByEmail(emailList);
            var invitedRespondentsCount = InviteRespondents(surveyProjectId, respondents);
            return new InviteResult
                       {
                           InvitedUsers = invitedRespondentsCount,
                           NewUsers = respondents.Count(r => r.RespondentId == 0)
                       };
        }

        private int InviteRespondents(int surveyProjectId, IEnumerable<Respondent> respondents)
        {
            var invitedUsers = 0;
            foreach (var respondent in respondents.Where(respondent => respondent.Invitations.All(invite => invite.SurveyProjectId != surveyProjectId || invite.Deleted)))
            {
                _surveyInvitationRepository.Add(new SurveyInvitation
                                                    {
                                                        Respondent = respondent,
                                                        SurveyProjectId = surveyProjectId
                                                    });
                invitedUsers++;
            }
            return invitedUsers;
        }

        private IList<Respondent> GetOrCreateRespondentsByEmail(IEnumerable<string> emailList)
        {
            var respondents = new List<Respondent>();
            foreach (var email in emailList)
            {
                var respondent = _respondentRepository.GetByEmail(email);
                if (respondent == null)
                {
                    respondent = new Respondent
                                     {
                                         RespondentEmail = email,
                                         Invitations = new HashSet<SurveyInvitation>()
                                     };
                    _respondentRepository.Add(respondent);
                }
                respondents.Add(respondent);
            }
            return respondents;
        }

        private static IEnumerable<string> GetEmailList(string emails)
        {
            return emails.Split(new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries).Select(str => str.Trim()).Where(
                str => str != "");
        }

        public void DeleteInvitationIfNeeded(int currentRespondent, int project)
        {
            var respondent = _respondentRepository.GetById(currentRespondent);
            var invites =
                respondent.Invitations.Where(invitation => invitation.SurveyProjectId == project);
            foreach (var invite in invites)
            {
                invite.Deleted = true;
            }
        }

        public void InviteRespondentsToProject(int surveyProjectId, IEnumerable<int> respondentIds)
        {
            InviteRespondents(surveyProjectId, _respondentRepository.GetRespondentsByIds(respondentIds));
        }

        public IEnumerable<string> GetPossibleRoles()
        {
            return new[] {"Staff", "Interviewer", "Admin"};
        }

        public int SendInvites(string siteName)
        {
            var respondent = _respondentRepository.GetUnInvited().FirstOrDefault();
            if (respondent == null)
            {
                return 0;
            }
            SendInviteEmail(respondent, siteName);
            return 1;
        }

        public void SendInviteEmail(Respondent respondent, string siteName)
        {
            if (respondent.Token == null)
            {
                respondent.Token = Guid.NewGuid();
            }
            var mailMessage = GetMailMessage(respondent, siteName);
            new SmtpClient().Send(mailMessage);
            _respondentRepository.Edit(respondent);
        }

        public string MultiInvite(int surveyProjectId, IEnumerable<string> emails, string inviteText, string siteName, string inviteSubject)
        {
            var errorResult = string.Empty;
            var smtpClient = new SmtpClient();
            foreach (var email in emails) {
                if (!email.IsValidEmail()) {
                    errorResult += "," + email;
                    continue;
                }
                try {
                    Guid ticketGuid = Guid.NewGuid();
                    MailMessage msg = GetTicketInviteMessage(siteName, email, inviteText, inviteSubject, ticketGuid);
                    smtpClient.Send(msg);
                    CreateTicketForProject(surveyProjectId, email, ticketGuid);
                } catch (SmtpException e) {
                    errorResult += "," + email;
                }
            }
            return errorResult;
        }

        public void CreateTicketForProject(int surveyProjectId, string description, Guid? ticketGuid)
        {
            var ticket = new Ticket
            {
                TicketGuid = ticketGuid ?? Guid.NewGuid(),
                StartDate = DateTime.Now,
                Invitations = new Collection<SurveyInvitation>(),
                TicketDescription = description
            };
            ticket.Invitations.Add(new SurveyInvitation { SurveyProjectId = surveyProjectId });
            _ticketRepository.Add(ticket);
        }

/*
        private IList<Respondent> GenerateManyRespondents(int inviteCount)
        {
            var result = new List<Respondent>();
            for (var i = 0; i < inviteCount; i++)
            {
                var respondent = new Respondent
                                     {
                                         Invitations = new HashSet<SurveyInvitation>(),
                                         Token = Guid.NewGuid()
                                     };
                result.Add(respondent);
                _respondentRepository.Add(respondent);
            }
            return result;
        }
*/
        private static MailMessage GetTicketInviteMessage(string siteName, string email, string inviteText, string inviteSubject, Guid ticketGuid)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(ReplyMailAddress),
                Subject = inviteSubject,
                Body = inviteText + "\n" + GetTokenLink(siteName, ticketGuid),
                DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure // | DeliveryNotificationOptions.OnSuccess
            };
            mailMessage.To.Add(email);
            return mailMessage;
        }

        private static MailMessage GetMailMessage(Respondent respondent, string siteName)
        {
            var mailMessage = new MailMessage
                                  {
                                      From = new MailAddress(ReplyMailAddress),
                                      Subject = "Приглашаем принять участие в опросе!",
                                      Body = string.Format(
                                          @"Здравствуйте!
Приглашаем вас принять участие в опросе:
{0}
Для участия перейдите по ссылке:
{1}
",
                                          string.Join("\n",
                                                      respondent.Invitations.Select(
                                                          inv => inv.SurveyProject.SurveyProjectName)),
                                          GetRespondentLink(respondent, siteName))
                                  };
            mailMessage.To.Add(respondent.RespondentEmail);
            return mailMessage;
        }

        private static string GetRespondentLink(Respondent respondent, string siteName)
        {
            return GetTokenLink(siteName, respondent.Token);
        }

        private static string GetTokenLink(string siteName, Guid? token)
        {
            return string.Format("{1}/Account/RegisterWithTicket/?ticketGuid={0}", token, siteName);
        }
    }
}
