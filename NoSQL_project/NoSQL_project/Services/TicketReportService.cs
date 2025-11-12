using MongoDB.Bson;
using NoSQL_project.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class TicketReportService : ITicketReportService
{
    private readonly ITicketRepository _repo;
    public TicketReportService(ITicketRepository repo) => _repo = repo;

    public Task<List<BsonDocument>> ByStatusAsync(CancellationToken ct = default) =>
        _repo.AggByStatusAsync(ct);

    public Task<List<BsonDocument>> ByIncidentTypeAsync(CancellationToken ct = default) =>
        _repo.AggByIncidentTypeAsync(ct);

    public Task<List<BsonDocument>> ByUserLocationAsync(CancellationToken ct = default) =>
        _repo.AggByUserLocationAsync(ct);

    public Task<List<BsonDocument>> DailyCreatedAsync(int lastNDays = 14, CancellationToken ct = default) =>
        _repo.AggDailyCreatedAsync(lastNDays, ct);
}
