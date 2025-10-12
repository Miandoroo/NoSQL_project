using MongoDB.Driver;
using MongoDB.Bson;
using NoSQL_project.Models;
using NoSQL_project.Repositories.Interfaces;
using System.Linq;


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
        public Tickets? GetById(string id)
        {
            return _ticketss.Find(Tickets => Tickets.Id == id).FirstOrDefault();
        }
        public void Add(Tickets tickets)
        {
            if (string.IsNullOrEmpty(tickets.Id))
                tickets.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

            _ticketss.InsertOne(tickets);
        }

        public void Delete(string id)
        {
            _ticketss.DeleteOne(ticket => ticket.Id == id);
        }

        public List<Tickets> GetByUserId(string userId)
        {
            return _ticketss.Find(ticket => ticket.UserId == userId).ToList();
        }

        public List<Tickets> GetByStatus(int status)
        {
            return _ticketss.Find(ticket => ticket.Status == status).ToList();
        }

        public List<Tickets> GetByPriority(string priority)
        {
            return _ticketss.Find(ticket => ticket.Priority == priority).ToList();
        }
        
    }
}
