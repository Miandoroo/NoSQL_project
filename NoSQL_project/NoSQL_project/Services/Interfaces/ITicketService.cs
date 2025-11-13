using NoSQL_project.Models;
using NoSQL_project.Models.ViewModels;

namespace NoSQL_project.Services.Interfaces
{
    public interface ITicketService
    {
        List<Ticket> GetAll();
        List<Ticket> GetByUserId(string userId);
        Ticket? GetById(string id);
        void Create(Ticket ticket);
        void Update(string id, Ticket ticket);
        void Delete(string id);
        public DashboardViewModel DashboardEmployee(bool IsServiceDesk, string userId);
        List<PriorityCount> GetTicketsByPriority(string userId, bool isServiceDesk);

    }
}

