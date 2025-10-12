using MongoDB.Driver;
using NoSQL_project.Models;
using NoSQL_project.Repositories.Interfaces;


namespace NoSQL_project.Repositories
{
    public class TicketRepository : ITicketRepository
    {

        private readonly IMongoCollection<Tickets> _ticketss;
        public TicketRepository(IMongoDatabase db)
        {
            _ticketss = db.GetCollection<Tickets>("Tickets");
        }

        public List<Tickets> GetAll()
        {
            return _ticketss.Find(_ => true).ToList();
        }
    }
}
