using MongoDB.Driver;
using MongoDB.Bson;
using NoSQL_project.Models;
using NoSQL_project.Repositories.Interfaces;

namespace NoSQL_project.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly IMongoCollection<Ticket> _tickets;

        public TicketRepository(IMongoDatabase db)
        {
            _tickets = db.GetCollection<Ticket>("Tickets");
        }

        public List<Ticket> GetAll()
        {
            return _tickets.Find(_ => true).ToList();
        }

        public Ticket? GetById(string id)
        {
            return _tickets.Find(ticket => ticket.Id == id).FirstOrDefault();
        }

        public void Add(Ticket ticket)
        {
            if (string.IsNullOrEmpty(ticket.Id))
                ticket.Id = ObjectId.GenerateNewId().ToString();

            _tickets.InsertOne(ticket);
        }

        public void Update(string id, Ticket ticket)
        {
            _tickets.ReplaceOne(t => t.Id == id, ticket);
        }

        public void Delete(string id)
        {
            _tickets.DeleteOne(ticket => ticket.Id == id);
        }

        public List<Ticket> GetByUserId(string userId)
        {
            return _tickets.Find(ticket => ticket.UserId == userId).ToList();
        }
    }
}
