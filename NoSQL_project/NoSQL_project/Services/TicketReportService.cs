using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQL_project.Services.Interfaces;

namespace NoSQL_project.Services
{
    public class TicketReportService : ITicketReportService
    {
        private readonly IMongoDatabase _db;
        public TicketReportService(IMongoDatabase db) => _db = db;

        public Task<List<BsonDocument>> GetGlobalStatusBreakdownAsync(CancellationToken ct = default)
        {
            var pipe = new[]
            {
                new BsonDocument("$group", new BsonDocument { { "_id", "$status" }, { "count", new BsonDocument("$sum", 1) } }),
                new BsonDocument("$sort", new BsonDocument("_id", 1))
            };
            return _db.GetCollection<BsonDocument>("Tickets").Aggregate<BsonDocument>(pipe).ToListAsync(ct);
        }

        public Task<List<BsonDocument>> GetUserStatusBreakdownAsync(string userId, CancellationToken ct = default)
        {
            var pipe = new[]
            {
                new BsonDocument("$match", new BsonDocument("userId", new ObjectId(userId))),
                new BsonDocument("$group", new BsonDocument { { "_id", "$status" }, { "count", new BsonDocument("$sum", 1) } }),
                new BsonDocument("$sort", new BsonDocument("_id", 1))
            };
            return _db.GetCollection<BsonDocument>("Tickets").Aggregate<BsonDocument>(pipe).ToListAsync(ct);
        }
    }
}
