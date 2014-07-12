using System;
using System.Collections.Generic;
using System.Linq;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;

namespace SurveyDomain.Repositories
{
    public class TicketRepository : RepositoryBase<Ticket>, ITicketRepository
    {
        public TicketRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Ticket GetById(int id)
        {
            return DataSet.Single(ticket => ticket.TicketId == id);
        }

        public Ticket GetByToken(Guid ticketGuid)
        {
            if (ticketGuid == Guid.Empty)
            {
                return null;
            }
            return DataSet.SingleOrDefault(ticket => ticket.TicketGuid == ticketGuid);
        }

        public ICollection<Ticket> GetTicketsForProject(int surveyProjectId)
        {
            throw new NotImplementedException();
        }
    }
}
