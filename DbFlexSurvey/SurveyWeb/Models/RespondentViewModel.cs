using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SurveyModel;

namespace SurveyWeb.Models
{
    public class RespondentViewModel
    {
        public static RespondentViewModel Create(Respondent respondent)
        {
            return new RespondentViewModel()
                       {
                           BirthYear = respondent.BirthYear,
                           IsMale = respondent.IsMale,
                           MembershipUserName = respondent.MembershipUserName,
                           RespondentDisplayName = respondent.RespondentDisplayName,
                           RespondentId = respondent.RespondentId,
                           RespondentEmail = respondent.RespondentEmail,
                           RespondentFullName = respondent.RespondentFullName,
                           RespondentPhone = respondent.RespondentPhone,
                           Registered = respondent.MembershipUserName != null,
                           InviteSent = respondent.Token != null
                       };
        }
        public string RespondentDisplayName { get; set; }
        public string RespondentFullName { get; set; }
        public string RespondentEmail { get; set; }
        public string RespondentPhone { get; set; }
        public int? BirthYear { get; set; }
        public bool? IsMale { get; set; }
        public string MembershipUserName { get; set; }
        public int RespondentId { get; set; }
        public ICollection<string> RespondentRoles { get; set; }
        public ICollection<string> PossibleRoles { get; set; }
        public bool IsCurrentUserAdmin { get; set; }
        public bool InviteSent { get; set; }
        public bool Registered { get; set; }
    }
}