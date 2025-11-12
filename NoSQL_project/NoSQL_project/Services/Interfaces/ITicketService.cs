using NoSQL_project.Models;

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
    }
}

