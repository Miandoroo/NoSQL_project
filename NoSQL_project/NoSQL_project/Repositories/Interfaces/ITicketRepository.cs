using NoSQL_project.Models;
using NoSQL_project.Models.ViewModels;

namespace NoSQL_project.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        List<Ticket> GetAll();
        void Add(Ticket ticket);
        Ticket? GetById(string id);
        void Update(string id, Ticket ticket);
        void Delete(string id);
        List<Ticket> GetByUserId(string userId);
        TicketStats GetTicketStats(string userId, bool isServiceDesk);
        List<Ticket> GetRecentTickets(string userId, bool isServiceDesk, int count);
        List<PriorityCount> GetTicketsByPriority(string userId, bool isServiceDesk);
        List<IncidentTypeCount> GetTicketsByIncidentType(string userId, bool isServiceDesk);
        List<TicketWithUser> GetTicketWithUser(string userId, bool isServiceDesk);

    }
}
