using NoSQL_project.Models;
using NoSQL_project.Models.ViewModels;

namespace NoSQL_project.Services.Interfaces
{
    public interface ITicketService
    {
        List<Ticket> GetAll();
        List<Ticket> GetByUserId(string userId);
        Ticket? GetById(string id);
        void Create(Ticket ticket, string? currentUserId, bool isServiceDesk);
        void Update(string id, Ticket ticket, string? userId, bool isServiceDesk);
        void Delete(string id);
        List<Ticket> GetTicketsForUser(string userId, bool isServiceDesk, string? searchQuery);
        bool CanAccessTicket(Ticket ticket, string userId, bool isServiceDesk);
        User? GetTicketUser(string ticketId);
        DashboardViewModel DashboardEmployee(bool IsServiceDesk, string userId);
    }
}

