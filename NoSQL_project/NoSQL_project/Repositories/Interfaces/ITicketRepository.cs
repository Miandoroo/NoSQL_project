using NoSQL_project.Models;

namespace NoSQL_project.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        List<Tickets> GetAll();
        void Add(Tickets tickets);
        Tickets? GetById(string id);
        void Update(string id, Tickets tickets);
        //void Delete(string id);

    }
}
