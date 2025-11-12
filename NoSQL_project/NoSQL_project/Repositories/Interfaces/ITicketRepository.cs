using MongoDB.Bson;
using NoSQL_project.Models;

namespace NoSQL_project.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        Task<List<BsonDocument>> AggByStatusAsync(CancellationToken ct = default);
        Task<List<BsonDocument>> AggByIncidentTypeAsync(CancellationToken ct = default);
        Task<List<BsonDocument>> AggByUserLocationAsync(CancellationToken ct = default);
        Task<List<BsonDocument>> AggDailyCreatedAsync(int lastNDays = 14, CancellationToken ct = default);
    }
}
