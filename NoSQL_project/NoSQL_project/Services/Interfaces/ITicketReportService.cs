using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace NoSQL_project.Services.Interfaces
{
    public interface ITicketReportService
    {
        Task<List<BsonDocument>> GetGlobalStatusBreakdownAsync(CancellationToken ct = default);
        Task<List<BsonDocument>> GetUserStatusBreakdownAsync(string userId, CancellationToken ct = default);
    }
}
