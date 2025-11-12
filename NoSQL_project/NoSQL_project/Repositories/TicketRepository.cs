using MongoDB.Driver;
using MongoDB.Bson;
using NoSQL_project.Models;
using NoSQL_project.Repositories.Interfaces;
using System.Linq;


namespace NoSQL_project.Repositories
{
    public class TicketRepository : ITicketRepository
    {

        private readonly IMongoCollection<BsonDocument> _tickets;
        public TicketRepository(IMongoDatabase db) =>
            _tickets = db.GetCollection<BsonDocument>("Tickets");

        public Task<List<BsonDocument>> AggByStatusAsync(CancellationToken ct = default)
        {
            var pipeline = new[]
            {
            new BsonDocument("$group", new BsonDocument { { "_id", "$status" }, { "count", new BsonDocument("$sum", 1) } }),
            new BsonDocument("$group", new BsonDocument { { "_id", BsonNull.Value }, { "total", new BsonDocument("$sum", "$count") }, { "rows", new BsonDocument("$push", new BsonDocument { { "status", "$_id" }, { "count", "$count" } }) } }),
            new BsonDocument("$unwind", "$rows"),
            new BsonDocument("$project", new BsonDocument {
                { "_id", 0 }, { "status", "$rows.status" }, { "count", "$rows.count" },
                { "percentage", new BsonDocument("$round", new BsonArray {
                    new BsonDocument("$multiply", new BsonArray {
                        new BsonDocument("$divide", new BsonArray { "$rows.count", "$total" }), 100
                    }), 2
                })}
            }),
            new BsonDocument("$sort", new BsonDocument("count", -1))
        };
            return _tickets.Aggregate<BsonDocument>(pipeline).ToListAsync(ct);
        }

        public Task<List<BsonDocument>> AggByIncidentTypeAsync(CancellationToken ct = default)
        {
            var pipeline = new[]
            {
            new BsonDocument("$group", new BsonDocument { { "_id", "$incidentType" }, { "count", new BsonDocument("$sum", 1) } }),
            new BsonDocument("$sort", new BsonDocument("count", -1))
        };
            return _tickets.Aggregate<BsonDocument>(pipeline).ToListAsync(ct);
        }

        public Task<List<BsonDocument>> AggByUserLocationAsync(CancellationToken ct = default)
        {
            var pipeline = new[]
            {
            new BsonDocument("$lookup", new BsonDocument {
                { "from", "Users" }, { "localField", "userId" }, { "foreignField", "_id" }, { "as", "user" }
            }),
            new BsonDocument("$unwind", "$user"),
            new BsonDocument("$group", new BsonDocument { { "_id", "$user.location" }, { "totalTickets", new BsonDocument("$sum", 1) } }),
            new BsonDocument("$sort", new BsonDocument("totalTickets", -1))
        };
            return _tickets.Aggregate<BsonDocument>(pipeline).ToListAsync(ct);
        }

        public Task<List<BsonDocument>> AggDailyCreatedAsync(int lastNDays = 14, CancellationToken ct = default)
        {
            var pipeline = new[]
            {
            new BsonDocument("$addFields", new BsonDocument("dateISO", new BsonDocument("$toDate", "$date"))),
            new BsonDocument("$match", new BsonDocument("dateISO", new BsonDocument("$gte",
                new BsonDocument("$dateSubtract", new BsonDocument { { "startDate", "$$NOW" }, { "unit", "day" }, { "amount", lastNDays - 1 } } )))),
            new BsonDocument("$project", new BsonDocument("day",
                new BsonDocument("$dateToString", new BsonDocument { { "format", "%Y-%m-%d" }, { "date", "$dateISO" } }))),
            new BsonDocument("$group", new BsonDocument { { "_id", "$day" }, { "count", new BsonDocument("$sum", 1) } }),
            new BsonDocument("$sort", new BsonDocument("_id", 1))
        };
            return _tickets.Aggregate<BsonDocument>(pipeline).ToListAsync(ct);
        }
    }
}
