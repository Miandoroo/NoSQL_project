using NoSQL_project.Models;
using NoSQL_project.Models.ViewModels;
using NoSQL_project.Repositories.Interfaces;
using NoSQL_project.Services.Interfaces;

namespace NoSQL_project.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _repo;
        public TicketService(ITicketRepository repo) => _repo = repo;

        public Task<List<Tickets>> GetAllAsync(CancellationToken ct = default) => _repo.GetAllAsync(ct);
        public Task<Tickets?> GetByIdAsync(string id, CancellationToken ct = default) => _repo.GetByIdAsync(id, ct);
        public Task CreateAsync(Tickets t, CancellationToken ct = default) => _repo.AddAsync(t, ct);
        public Task UpdateAsync(string id, Tickets t, CancellationToken ct = default) => _repo.UpdateAsync(id, t, ct);
        public Task DeleteAsync(string id, CancellationToken ct = default) => _repo.DeleteAsync(id, ct);

        public Task<List<Tickets>> GetByUserIdAsync(string userId, CancellationToken ct = default) => _repo.GetByUserIdAsync(userId, ct);
        public Task<List<Tickets>> GetByStatusAsync(int status, CancellationToken ct = default) => _repo.GetByStatusAsync(status, ct);
        public Task<List<Tickets>> GetByPriorityAsync(string priority, CancellationToken ct = default) => _repo.GetByPriorityAsync(priority, ct);
        public Task<List<Tickets>> GetLastDaysAggAsync(int days = 7, CancellationToken ct = default)
            => _repo.GetLastDaysAggAsync(days, ct);
        public Task<List<Tickets>> GetByUserLastDaysAggAsync(string userId, int days = 7, CancellationToken ct = default)
            => _repo.GetByUserLastDaysAggAsync(userId, days, ct);

        public async Task<DashboardVm> GetDashboardAsync(int days, string? userId, bool isAll, CancellationToken ct = default)
        {
            var counts = await _repo.GetStatusSummaryAggAsync(days, userId, ct);

            var vm = new DashboardVm
            {
                Days = days,
                IsAll = isAll,
                Open = counts.TryGetValue(0, out var c0) ? c0 : 0,
                InProgress = counts.TryGetValue(1, out var c1) ? c1 : 0,
                Resolved = counts.TryGetValue(2, out var c2) ? c2 : 0,
                Closed = counts.TryGetValue(3, out var c3) ? c3 : 0,
            };
            vm.Total = vm.Open + vm.InProgress + vm.Resolved + vm.Closed;
            return vm;
        }
    }
}
