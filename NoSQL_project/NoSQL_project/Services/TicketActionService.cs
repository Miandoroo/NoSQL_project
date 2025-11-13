using NoSQL_project.Models;
using NoSQL_project.Repositories.Interfaces;
using NoSQL_project.Services.Interfaces;
using NoSQL_project.Enum;

namespace NoSQL_project.Services
{
    public class TicketActionService : ITicketActionService
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketActionService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }
        public void Escalate(string id)
        {
            Ticket ticket = _ticketRepository.GetById(id);

            if (ticket.status == TicketStatus.Resolved || ticket.status == TicketStatus.Closed)
            {
                throw new InvalidOperationException("Can't escalate resolved or closed tickets");
            }

            ticket.Priority = TicketPrioritys.hoog;
            ticket.Deadline = DateTime.Now.AddHours(24);

            _ticketRepository.Update(id, ticket);
        }
        public void Close(string id)
        {
            Ticket ticket = _ticketRepository.GetById(id);

            if (ticket.status == TicketStatus.Closed)
            {
                throw new InvalidOperationException("Ticket is already closed");
            }

            ticket.status = TicketStatus.Closed;
            ticket.Deadline = DateTime.Now;

            _ticketRepository.Update(id, ticket);
        }

        public void Resolve(string id)
        {
            Ticket ticket = GetTicketOrThrow(id);

            if (ticket.status == TicketStatus.Closed)
            {
                throw new InvalidOperationException("Cannot resolve a closed ticket");
            }

            ticket.status = TicketStatus.Resolved;
            _ticketRepository.Update(ticket.Id, ticket);
        }

        public Ticket GetTicketOrThrow(string id)
        {
            Ticket ticket = _ticketRepository.GetById(id);
            if (ticket == null)
            {
                throw new ArgumentException("Ticket not found");
            }
            return ticket;
        }

    }
}
