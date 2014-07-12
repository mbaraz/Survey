using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurveyModel
{
    public class SurveyInvitation
    {
        public int SurveyInvitationId { get; set; }

        public int? RespondentId { get; set; }
        public virtual Respondent Respondent { get; set; }

        public int SurveyProjectId { get; set; }
        public virtual SurveyProject SurveyProject { get; set; }

        public bool Deleted { get; set; }

        public int? TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        public int? OriginTicketId { get; set; }
        public virtual Ticket OriginTicket { get; set; }
    }
}
