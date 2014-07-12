using System;
using System.Collections.Generic;
using SurveyModel;

namespace SurveyInterfaces.Repositories
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Ticket GetByToken(Guid ticketGuid);
        ICollection<Ticket> GetTicketsForProject(int surveyProjectId);
    }
}
