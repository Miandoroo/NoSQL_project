using NoSQL_project.Models;
using NoSQL_project.Models.ViewModels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NoSQL_project.Services.Interfaces
{
    public interface ITicketService
    {
        Task<List<Tickets>> GetAllAsync(CancellationToken ct = default);
        Task<Tickets?> GetByIdAsync(string id, CancellationToken ct = default);
        Task CreateAsync(Tickets t, CancellationToken ct = default);
        Task UpdateAsync(string id, Tickets t, CancellationToken ct = default);
        Task DeleteAsync(string id, CancellationToken ct = default);

        // usadas por tu controlador
        Task<List<Tickets>> GetByUserIdAsync(string userId, CancellationToken ct = default);
        Task<List<Tickets>> GetByStatusAsync(int status, CancellationToken ct = default);
        Task<List<Tickets>> GetByPriorityAsync(string priority, CancellationToken ct = default);
        Task<List<Tickets>> GetLastDaysAggAsync(int days = 7, CancellationToken ct = default);
        Task<List<Tickets>> GetByUserLastDaysAggAsync(string userId, int days = 7, CancellationToken ct = default);
        Task<DashboardVm> GetDashboardAsync(int days, string? userId, bool isAll, CancellationToken ct = default);

    }
}
