using System.Threading;
using System.Threading.Tasks;
using NoSQL_project.Models;
using System.Collections.Generic;

namespace NoSQL_project.Services.Interfaces
{
    public interface IUserService
    {
        Task<Users?> GetByUsernameAsync(string username, CancellationToken ct = default);
        Task<Users?> GetByIdAsync(string id, CancellationToken ct = default);

        // CRUD (si los usas en UsersController)
        Task<List<Users>> GetAllAsync(CancellationToken ct = default);
        Task CreateAsync(Users user, CancellationToken ct = default);
        Task UpdateAsync(string id, Users user, CancellationToken ct = default);
        Task DeleteAsync(string id, CancellationToken ct = default);
    }
}
