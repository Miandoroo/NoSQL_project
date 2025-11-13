using MongoDB.Bson;
using NoSQL_project.Models;
using NoSQL_project.Models.ViewModels;
using NoSQL_project.Repositories.Interfaces;
using NoSQL_project.Services;
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

        public void Create(Ticket ticket, string? currentUserId, bool isServiceDesk)
        {
            if (string.IsNullOrEmpty(ticket.Id))
                ticket.Id = ObjectId.GenerateNewId().ToString();

            if (ticket.Date == default(DateTime))
            {
                ticket.Date = DateTime.Now;
            }

            if (!isServiceDesk)
            {
                ticket.UserId = currentUserId ?? throw new ArgumentException("User ID is required");
            }
            else if (string.IsNullOrEmpty(ticket.UserId))
            {
                throw new ArgumentException("Please select a user");
            }

            _ticketRepo.Add(ticket);
        }

        public void Update(string id, Ticket ticket, string? userId, bool isServiceDesk)
        {
            ticket.Id = id;
            
            if (isServiceDesk && !string.IsNullOrEmpty(userId))
            {
                ticket.UserId = userId;
            }
            else
            {
                var existingTicket = GetById(id);
                if (existingTicket != null)
                {
                    ticket.UserId = existingTicket.UserId;
                }
            }

            _ticketRepo.Update(id, ticket);
        }

        public List<Ticket> GetTicketsForUser(string userId, bool isServiceDesk, string? searchQuery)
        {
            List<Ticket> tickets;
            if (isServiceDesk)
            {
                tickets = GetAll();
            }
            else
            {
                tickets = GetByUserId(userId);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var searchService = new TicketSearchService();
                tickets = searchService.SearchTickets(tickets, searchQuery);
            }
            else
            {
                tickets = tickets.OrderByDescending(t => t.Date).ToList();
            }

            return tickets;
        }

        public bool CanAccessTicket(Ticket ticket, string userId, bool isServiceDesk)
        {
            return isServiceDesk || ticket.UserId == userId;
        }

        public User? GetTicketUser(string ticketId)
        {
            var ticket = GetById(ticketId);
            if (ticket == null)
                return null;

            return _userRepo.GetById(ticket.UserId);
        }

        public void Delete(string id)
        {
            if (_ticketRepo.GetById(id) == null)
                throw new ArgumentException("Ticket not found");

            _ticketRepo.Delete(id);
        }

        public DashboardViewModel DashboardEmployee(bool IsServiceDesk, string userId)
        {
            List<Ticket> tickets;
            if (IsServiceDesk)
            {
                tickets = _ticketRepo.GetAll();
            }
            else
            {
                tickets = _ticketRepo.GetByUserId(userId);
            }

            TicketStats stats = new TicketStats
            {
                TotalTickets = tickets.Count,
                OpenTickets = tickets.Count(t => t.status == Enum.TicketStatus.Open),
                ResolvedTickets = tickets.Count(t => t.status == Enum.TicketStatus.Resolved),
                ClosedTickets = tickets.Count(t => t.status == Enum.TicketStatus.Closed)
            };

            List<Ticket> recentTickets = tickets
                .OrderByDescending(t => t.Date)
                .Take(5)
                .ToList();

            return new DashboardViewModel(stats, recentTickets, IsServiceDesk);
        }
    }
}

