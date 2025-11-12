using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;

public interface ITicketReportService
{
    Task<List<BsonDocument>> ByStatusAsync(CancellationToken ct = default);
    Task<List<BsonDocument>> ByIncidentTypeAsync(CancellationToken ct = default);
    Task<List<BsonDocument>> ByUserLocationAsync(CancellationToken ct = default);
    Task<List<BsonDocument>> DailyCreatedAsync(int lastNDays = 14, CancellationToken ct = default);
}
