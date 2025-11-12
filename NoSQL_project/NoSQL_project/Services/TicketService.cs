using MongoDB.Bson;
using NoSQL_project.Models;
using NoSQL_project.Repositories.Interfaces;
using NoSQL_project.Services.Interfaces;

namespace NoSQL_project.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepo;
        private readonly IUserRepository _userRepo;

        public TicketService(ITicketRepository ticketRepo, IUserRepository userRepo)
        {
            _ticketRepo = ticketRepo;
            _userRepo = userRepo;
        }

        public List<Ticket> GetAll()
        {
            return _ticketRepo.GetAll();
        }

        public List<Ticket> GetByUserId(string userId)
        {
            return _ticketRepo.GetByUserId(userId);
        }

        public Ticket? GetById(string id)
        {
            return _ticketRepo.GetById(id);
        }

        public void Create(Ticket ticket)
        {
            if (string.IsNullOrEmpty(ticket.Id))
                ticket.Id = ObjectId.GenerateNewId().ToString();

            if (ticket.Date == default(DateTime))
            {
                ticket.Date = DateTime.Now;
            }

            _ticketRepo.Add(ticket);
        }

        public void Update(string id, Ticket ticket)
        {
            ticket.Id = id;
            _ticketRepo.Update(id, ticket);
        }

        public void Delete(string id)
        {
            var ticket = _ticketRepo.GetById(id);
            if (ticket == null)
                throw new ArgumentException("Ticket not found");

            _ticketRepo.Delete(id);
        }
    }
}

