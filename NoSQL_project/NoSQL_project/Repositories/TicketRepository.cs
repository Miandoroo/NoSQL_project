using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQL_project.Models;
using NoSQL_project.Repositories.Interfaces;

namespace NoSQL_project.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly IMongoCollection<Tickets> _tickets;

        public TicketRepository(IMongoDatabase db)
        {
            _tickets = db.GetCollection<Tickets>("Tickets");
        }

        // -------- CRUD --------
        public Task<List<Tickets>> GetAllAsync(CancellationToken ct = default)
        {
            return _tickets.Find(FilterDefinition<Tickets>.Empty).ToListAsync(ct);
        }

        public Task<Tickets?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            return _tickets.Find(t => t.Id == id).FirstOrDefaultAsync(ct);
        }

        public async Task AddAsync(Tickets t, CancellationToken ct = default)
        {
            await _tickets.InsertOneAsync(t, cancellationToken: ct);
        }



        public Task UpdateAsync(string id, Tickets tickets, CancellationToken ct = default)
        {
            tickets.Id = id;
            return _tickets.ReplaceOneAsync(t => t.Id == id, tickets, cancellationToken: ct);
        }

        public Task DeleteAsync(string id, CancellationToken ct = default)
        {
            return _tickets.DeleteOneAsync(t => t.Id == id, ct);
        }

        // -------- Queries --------
        public Task<List<Tickets>> GetByUserIdAsync(string userId, CancellationToken ct = default)
        {
            var uid = MongoDB.Bson.ObjectId.Parse(userId);
            var filter = Builders<Tickets>.Filter.Eq("userId", uid);
            return _tickets.Find(filter).ToListAsync(ct);
        }

        public Task<List<Tickets>> GetByStatusAsync(int status, CancellationToken ct = default)
        {
            return _tickets.Find(t => t.Status == status).ToListAsync(ct);
        }

        public Task<List<Tickets>> GetByPriorityAsync(string priority, CancellationToken ct = default)
        {
            return _tickets.Find(t => t.Priority == priority).ToListAsync(ct);
        }
        public async Task<List<Tickets>> GetLastDaysAggAsync(int days = 7, CancellationToken ct = default)
        {
            var from = DateTime.UtcNow.Date.AddDays(-days);

            var pipeline = new[]
            {
        new BsonDocument("$match", new BsonDocument("date", new BsonDocument("$gte", from))),
        new BsonDocument("$sort", new BsonDocument("date", -1))
        };

            return await _tickets.Aggregate<Tickets>(pipeline).ToListAsync(ct);
        }

        public async Task<List<Tickets>> GetByUserLastDaysAggAsync(string userId, int days = 7, CancellationToken ct = default)
        {
            var from = DateTime.UtcNow.Date.AddDays(-days);
            var uid = MongoDB.Bson.ObjectId.Parse(userId);

            var pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument{
                { "userId", uid },
                { "date", new BsonDocument("$gte", from) }
                }),
            new BsonDocument("$sort", new BsonDocument("date", -1))
            };

            return await _tickets.Aggregate<Tickets>(pipeline).ToListAsync(ct);
        }

        public async Task<Dictionary<int, int>> GetStatusSummaryAggAsync(int days, string? userId, CancellationToken ct = default)
        {
            var from = DateTime.UtcNow.Date.AddDays(-days);

            var match = new BsonDocument("date", new BsonDocument("$gte", from));
            if (!string.IsNullOrEmpty(userId))
                match.Add("userId", ObjectId.Parse(userId));

            var pipeline = new[]
            {
                new BsonDocument("$match", match),
                new BsonDocument("$group", new BsonDocument {
                    { "_id", "$status" },
                    { "count", new BsonDocument("$sum", 1) }
                })
            };

            var results = await _tickets.Aggregate<BsonDocument>(pipeline).ToListAsync(ct);
            return results.ToDictionary(d => d["_id"].AsInt32, d => d["count"].AsInt32);
        }


    }

}
