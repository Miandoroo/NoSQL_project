using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NoSQL_project.Models;

namespace NoSQL_project.Repositories.Interfaces
{
    public interface IUserRepository
    {
        // ---------- CRUD ----------
        Task<List<Users>> GetAllAsync(CancellationToken ct = default);
        Task<Users?> GetByIdAsync(string id, CancellationToken ct = default);
        Task AddAsync(Users user, CancellationToken ct = default);
        Task UpdateAsync(string id, Users user, CancellationToken ct = default);
        Task DeleteAsync(string id, CancellationToken ct = default);

        // ---------- FILTERS / QUERIES ----------
        Task<Users?> GetByUsernameAsync(string username, CancellationToken ct = default);
        Task<Users?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<List<Users>> GetByTypeAsync(string type, CancellationToken ct = default);
        Task<List<Users>> GetByLocationAsync(string location, CancellationToken ct = default);

        // ---------- AGGREGATIONS ----------
        Task<List<MongoDB.Bson.BsonDocument>> Agg_CountByTypeAsync(CancellationToken ct = default);
        Task<List<MongoDB.Bson.BsonDocument>> Agg_CountByLocationAsync(CancellationToken ct = default);
        Task<List<MongoDB.Bson.BsonDocument>> Agg_UsersWithTicketCountAsync(CancellationToken ct = default);
        Task<List<MongoDB.Bson.BsonDocument>> Agg_UsersWithOpenTicketsAsync(CancellationToken ct = default);
    }
}