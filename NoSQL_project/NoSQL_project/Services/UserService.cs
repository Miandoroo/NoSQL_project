using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NoSQL_project.Models;
using NoSQL_project.Repositories.Interfaces;
using NoSQL_project.Services.Interfaces;

namespace NoSQL_project.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        public UserService(IUserRepository repo) => _repo = repo;

        public Task<Users?> GetByUsernameAsync(string username, CancellationToken ct = default) => _repo.GetByUsernameAsync(username, ct);
        public Task<Users?> GetByIdAsync(string id, CancellationToken ct = default) => _repo.GetByIdAsync(id, ct);

        public Task<List<Users>> GetAllAsync(CancellationToken ct = default) => _repo.GetAllAsync(ct);
        public Task CreateAsync(Users user, CancellationToken ct = default) => _repo.AddAsync(user, ct);
        public Task UpdateAsync(string id, Users user, CancellationToken ct = default) => _repo.UpdateAsync(id, user, ct);
        public Task DeleteAsync(string id, CancellationToken ct = default) => _repo.DeleteAsync(id, ct);
    }
}
