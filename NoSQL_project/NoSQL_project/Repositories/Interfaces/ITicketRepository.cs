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
    }
}
