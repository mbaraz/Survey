using System;
using System.Collections.Generic;
using System.Linq;
using SurveyModel.Univer;

namespace SurveyModel
{
    public class Respondent
    {
        public Respondent()
        {
            Invitations = new HashSet<SurveyInvitation>();
            Interviews = new HashSet<Interview>();
        }
        public int RespondentId { get; set; }

        public string RespondentFirstName { get; set; }
        public string RespondentFatherName { get; set; }
        public string RespondentSurName { get; set; }
        
        public string RespondentEmail { get; set; }
        public string RespondentPhone { get; set; }

        public Guid? Token { get; set; }

        public string RespondentDisplayName
        {
            get
            {
                if (RespondentFullName.Length > 0)
                {
                    return RespondentFullName;
                }
                if (MembershipUserName.Length > 0)
                {
                    return MembershipUserName;
                }
                return "Неизвестный респондент";
            }
        }

        public string RespondentFullName
        {
            get { return string.Join(" ", new[] {RespondentFirstName, RespondentFatherName, RespondentSurName}.Where(s => s!= null)); }
        }

        public int? BirthYear { get; set; }
        public bool? IsMale { get; set; }

        public string MembershipUserName { get; set; }

        public virtual ICollection<SurveyInvitation> Invitations { get; set; }
        
        public virtual ICollection<Interview> Interviews { get; set; }

        public IEnumerable<SurveyInvitation> ActiveInvitations
        {
            get { return Invitations.Where(invitation => !invitation.Deleted); }
        }

        public IEnumerable<SurveyProject> ActiveSurveys
        {
            get { return Interviews.Where(interview => !interview.Completed && !interview.TestInterview).Select(interview => interview.SurveyProject); }
        }

        public int? StudentGroupId { get; set; }
        public virtual StudentGroup StudentGroup { get; set; }

        public string RespondentComment { get; set; }
    }
}
