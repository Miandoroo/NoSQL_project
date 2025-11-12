using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQL_project.Models;
using NoSQL_project.Repositories.Interfaces;

namespace NoSQL_project.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<Users> _users;
        private readonly IMongoDatabase _db;

        public UserRepository(IMongoDatabase db)
        {
            _db = db;
            _users = db.GetCollection<Users>("Users");
        }

        // ---------- CRUD ----------
        public async Task<List<Users>> GetAllAsync(CancellationToken ct = default)
        {
            return await _users.Find(FilterDefinition<Users>.Empty).ToListAsync(ct);
        }

        public async Task<Users?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            return await _users.Find(u => u.Id == id).FirstOrDefaultAsync(ct);
        }

        public async Task AddAsync(Users user, CancellationToken ct = default)
        {
            await _users.InsertOneAsync(user, cancellationToken: ct);
        }

        public async Task UpdateAsync(string id, Users user, CancellationToken ct = default)
        {
            await _users.ReplaceOneAsync(u => u.Id == id, user, cancellationToken: ct);
        }

        public async Task DeleteAsync(string id, CancellationToken ct = default)
        {
            await _users.DeleteOneAsync(u => u.Id == id, ct);
        }

        // ---------- FILTERS / QUERIES ----------
        public async Task<Users?> GetByUsernameAsync(string username, CancellationToken ct = default)
        {
            return await _users.Find(u => u.Username == username).FirstOrDefaultAsync(ct);
        }

        public async Task<Users?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync(ct);
        }

        public async Task<List<Users>> GetByTypeAsync(string type, CancellationToken ct = default)
        {
            return await _users.Find(u => u.Type == type).ToListAsync(ct);
        }

        public async Task<List<Users>> GetByLocationAsync(string location, CancellationToken ct = default)
        {
            return await _users.Find(u => u.Location == location).ToListAsync(ct);
        }

        // ---------- AGGREGATIONS ----------
        public async Task<List<BsonDocument>> Agg_CountByTypeAsync(CancellationToken ct = default)
        {
            var pipe = new[]
            {
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$type" },
                    { "count", new BsonDocument("$sum", 1) }
                }),
                new BsonDocument("$sort", new BsonDocument("count", -1))
            };
            return await _db.GetCollection<BsonDocument>("Users").Aggregate<BsonDocument>(pipe).ToListAsync(ct);
        }

        public async Task<List<BsonDocument>> Agg_CountByLocationAsync(CancellationToken ct = default)
        {
            var pipe = new[]
            {
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$location" },
                    { "count", new BsonDocument("$sum", 1) }
                }),
                new BsonDocument("$sort", new BsonDocument("count", -1))
            };
            return await _db.GetCollection<BsonDocument>("Users").Aggregate<BsonDocument>(pipe).ToListAsync(ct);
        }

        public async Task<List<BsonDocument>> Agg_UsersWithTicketCountAsync(CancellationToken ct = default)
        {
            var pipe = new[]
            {
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "Tickets" },
                    { "localField", "_id" },
                    { "foreignField", "userId" },
                    { "as", "tickets" }
                }),
                new BsonDocument("$project", new BsonDocument
                {
                    { "firstName", 1 },
                    { "lastName", 1 },
                    { "location", 1 },
                    { "type", 1 },
                    { "ticketCount", new BsonDocument("$size", "$tickets") }
                }),
                new BsonDocument("$sort", new BsonDocument("ticketCount", -1))
            };
            return await _db.GetCollection<BsonDocument>("Users").Aggregate<BsonDocument>(pipe).ToListAsync(ct);
        }

        public async Task<List<BsonDocument>> Agg_UsersWithOpenTicketsAsync(CancellationToken ct = default)
        {
            var pipe = new[]
            {
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "Tickets" },
                    { "let", new BsonDocument("uid", "$_id") },
                    { "pipeline", new BsonArray
                        {
                            new BsonDocument("$match", new BsonDocument("$expr",
                                new BsonDocument("$and", new BsonArray
                                {
                                    new BsonDocument("$eq", new BsonArray { "$userId", "$$uid" }),
                                    new BsonDocument("$in", new BsonArray { "$status", new BsonArray { 0, 1 } })
                                })
                            ))
                        }
                    },
                    { "as", "openTickets" }
                }),
                new BsonDocument("$project", new BsonDocument
                {
                    { "firstName", 1 },
                    { "lastName", 1 },
                    { "location", 1 },
                    { "type", 1 },
                    { "openCount", new BsonDocument("$size", "$openTickets") }
                }),
                new BsonDocument("$sort", new BsonDocument("openCount", -1))
            };
            return await _db.GetCollection<BsonDocument>("Users").Aggregate<BsonDocument>(pipe).ToListAsync(ct);
        }
    }
}
