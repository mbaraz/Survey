using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurveyModel
{
    public class Ticket
    {
        public int TicketId { get; set; }

        public Guid TicketGuid { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? ExpireDate { get; set; }

        public string TicketDescription { get; set; }

        public virtual ICollection<SurveyInvitation> Invitations { get; set; }
        public virtual ICollection<SurveyInvitation> CreatedInvitations { get; set; }
    }
}
