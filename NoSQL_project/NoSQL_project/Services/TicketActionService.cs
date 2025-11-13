using NoSQL_project.Models;
using NoSQL_project.Repositories.Interfaces;
using NoSQL_project.Services.Interfaces;

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

            if (ticket.Status == 2 || ticket.Status == 3)
            {
                throw new InvalidOperationException("Can't escalate resolved or closed tickets");
            }

            ticket.Priority = "hoog";
            ticket.Deadline = DateTime.Now.AddHours(24);

            _ticketRepository.Update(id, ticket);
        }
        public void Close(string id)
        {
            Ticket ticket = _ticketRepository.GetById(id);

            if (ticket.Status == 3)
            {
                throw new InvalidOperationException("Ticket is already closed");
            }

            ticket.Status = 3;
            ticket.Deadline = DateTime.Now;

            _ticketRepository.Update(id, ticket);
        }

        public void Resolve(string id)
        {
            Ticket ticket = GetTicketOrThrow(id);

            if (ticket.Status == 3)
            {
                throw new InvalidOperationException("Cannot resolve a closed ticket");
            }

            ticket.Status = 2;
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
