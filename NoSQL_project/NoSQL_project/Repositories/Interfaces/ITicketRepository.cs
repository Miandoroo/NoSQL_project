using NoSQL_project.Models;

namespace NoSQL_project.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        List<Tickets> GetAll();
        //void Add(Users user);
        // Users? GetById(string id);
        //void Update(Users user);
        //void Delete(string id);

    }
}
