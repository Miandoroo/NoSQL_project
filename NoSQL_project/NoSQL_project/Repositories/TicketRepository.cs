using MongoDB.Driver;
using MongoDB.Bson;
using NoSQL_project.Models;
using NoSQL_project.Repositories.Interfaces;
using NoSQL_project.Enum;
using NoSQL_project.Models.ViewModels;


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
        // CRUD Operations
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

        // Pipeline Operations
        public TicketStats GetTicketStats(string userId, bool isServiceDesk)
        {
            List<BsonDocument> pipeline = new List<BsonDocument>();

            if (!isServiceDesk)
            {
                BsonDocument matchFilter = new BsonDocument("userId", new ObjectId(userId));
                BsonDocument matchStage = new BsonDocument("$match", matchFilter);
                pipeline.Add(matchStage);
            }

            string openStatus = TicketStatus.Open.ToString();
            string resolvedStatus = TicketStatus.Resolved.ToString();
            string closedStatus = TicketStatus.Closed.ToString();

            // Group Stage
            BsonDocument groupFields = new BsonDocument
            {
                { "_id", BsonNull.Value },
                { "totalTickets", new BsonDocument("$sum", 1) },
                { "openTickets", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray
                    {
                        new BsonDocument("$eq", new BsonArray { "$status", openStatus }),
                        1,
                        0
                    })) },
                { "resolvedTickets", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray
                    {
                        new BsonDocument("$eq", new BsonArray { "$status", resolvedStatus }),
                        1,
                        0
                    })) },
                { "closedTickets", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray
                    {
                        new BsonDocument("$eq", new BsonArray { "$status", closedStatus }),
                        1,
                        0
                    })) }
            };

            BsonDocument groupStage = new BsonDocument("$group", groupFields);
            pipeline.Add(groupStage);

            IAsyncCursor<BsonDocument> cursor = _tickets.Aggregate<BsonDocument>(pipeline);
            BsonDocument result = cursor.FirstOrDefault();

            TicketStats stats = new TicketStats();

            if (result != null)
            {
                stats.TotalTickets = result.GetValue("totalTickets").ToInt32();
                stats.OpenTickets = result.GetValue("openTickets").ToInt32();
                stats.ResolvedTickets = result.GetValue("resolvedTickets").ToInt32();
                stats.ClosedTickets = result.GetValue("closedTickets").ToInt32();
            }

            return stats;
        }



        // Get recent tickets
        public List<Ticket> GetRecentTickets(string userId, bool isServiceDesk, int count)
        {
            List<BsonDocument> pipeline = new List<BsonDocument>();
            if (!isServiceDesk)
            {
                BsonDocument matchFilter = new BsonDocument("userId", new ObjectId(userId));
                BsonDocument matchStage = new BsonDocument("$match", matchFilter);
                pipeline.Add(matchStage);
            }
            BsonDocument sortFiels = new BsonDocument("date", -1);
            BsonDocument sortStage = new BsonDocument("$sort", sortFiels);
            pipeline.Add(sortStage);
            BsonDocument limitStage = new BsonDocument("$limit", count);
            pipeline.Add(limitStage);
            IAsyncCursor<Ticket> cursor = _tickets.Aggregate<Ticket>(pipeline);
            List<Ticket> recentTickets = cursor.ToList();
            return recentTickets;
        }


        // Get tickets by priority
        public List<PriorityCount> GetTicketsByPriority(string userId, bool isServiceDesk)
        {
            List<BsonDocument> pipeline = new List<BsonDocument>();

            if (!isServiceDesk)
            {
                BsonDocument matchFilter = new BsonDocument("userId", new ObjectId(userId));
                BsonDocument matchStage = new BsonDocument("$match", matchFilter);
                pipeline.Add(matchStage);
            }

            BsonDocument groupFields = new BsonDocument
            {
                { "_id", "$priority" },
                { "count", new BsonDocument("$sum", 1) }
            };

            BsonDocument groupStage = new BsonDocument("$group", groupFields);
            pipeline.Add(groupStage);
            IAsyncCursor<BsonDocument> cursor = _tickets.Aggregate<BsonDocument>(pipeline);
            List<BsonDocument> docs = cursor.ToList();

            List<PriorityCount> result = new List<PriorityCount>();

            foreach (BsonDocument doc in docs)
            {
                PriorityCount item = new PriorityCount();
                string priorityStr = doc.GetValue("_id").AsString;
                item.Priority = (TicketPrioritys)System.Enum.Parse(typeof(TicketPrioritys), priorityStr);
                item.Count = doc.GetValue("count", 0).ToInt32();
                result.Add(item);
            }
            return result;
        }



        // Get tickets by incident type
        public List<IncidentTypeCount> GetTicketsByIncidentType(string userId, bool isServiceDesk)
        {
            List<BsonDocument> pipeline = new List<BsonDocument>();
            if (!isServiceDesk)
            {
                BsonDocument matchFilter = new BsonDocument("userId", new ObjectId(userId));
                BsonDocument matchStage = new BsonDocument("$match", matchFilter);
                pipeline.Add(matchStage);
            }
            BsonDocument groupFields = new BsonDocument
            {
                { "_id", "$incidentType" },
                { "count", new BsonDocument("$sum", 1) }
            };
            BsonDocument groupStage = new BsonDocument("$group", groupFields);
            pipeline.Add(groupStage);

            IAsyncCursor<BsonDocument> cursor = _tickets.Aggregate<BsonDocument>(pipeline);
            List<BsonDocument> docs = cursor.ToList();

            List<IncidentTypeCount> result = new List<IncidentTypeCount>();
            foreach (BsonDocument doc in docs)
            {
                IncidentTypeCount item = new IncidentTypeCount();
                string incidentTypeStr = doc.GetValue("_id").AsString;
                item.IncidentType = (TicketIncidentType)System.Enum.Parse(typeof(TicketIncidentType), incidentTypeStr);
                item.Count = doc.GetValue("count", 0).ToInt32();
                result.Add(item);
            }
            return result;
        }



        // Get tickets with user details
        public List<TicketWithUser> GetTicketWithUser(string userId, bool isServiceDesk)
        {
            List<BsonDocument> pipeline = new List<BsonDocument>();
            if (!isServiceDesk)
            {
                BsonDocument matchFilter = new BsonDocument("userId", new ObjectId(userId));
                BsonDocument matchStage = new BsonDocument("$match", matchFilter);
                pipeline.Add(matchStage);
            }
            BsonDocument lookupStage = new BsonDocument("$lookup",
                new BsonDocument
                {
                    { "from", "Users" },
                    { "localField", "userId" },
                    { "foreignField", "_id" },
                    { "as", "user" }
                });
            pipeline.Add(lookupStage);

            BsonDocument unwindStage = new BsonDocument("$unwind", "$user");
            pipeline.Add(unwindStage);
            IAsyncCursor<BsonDocument> cursor = _tickets.Aggregate<BsonDocument>(pipeline);
            List<BsonDocument> docs = cursor.ToList();
            List<TicketWithUser> result = new List<TicketWithUser>();
            foreach (BsonDocument doc in docs)
            {
                Ticket ticket = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<Ticket>(doc);
                User user = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<User>(doc.GetValue("userDetails").AsBsonDocument);
                TicketWithUser ticketWithUser = new TicketWithUser
                {
                    Ticket = ticket,
                    User = user
                };
                result.Add(ticketWithUser);
            }
            return result;
        }
    }
}
