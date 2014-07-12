using System;
using System.Collections.Generic;
using SurveyModel;

namespace SurveyInterfaces
{
    public interface IRespondentService
    {
        InviteResult InviteByEmail(int surveyProjectId, string emails);
        void DeleteInvitationIfNeeded(int currentRespondent, int project);
        void InviteRespondentsToProject(int surveyProjectId, IEnumerable<int> respondentId);
        IEnumerable<string> GetPossibleRoles();
        int SendInvites(string siteName);
        void SendInviteEmail(Respondent respondent, string siteName);
        string MultiInvite(int surveyProjectId, IEnumerable<string> email, string inviteText, string siteName, string inviteSubject);
        void CreateTicketForProject(int surveyProjectId, string description, Guid? ticketGuid);
    }

    public class InviteResult
    {
        public int InvitedUsers { get; set; }
        public int NewUsers { get; set; }
    }
}
