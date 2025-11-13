using NoSQL_project.Models;

namespace NoSQL_project.Services.Interfaces
{
    public interface IUserService
    {
        List<User> GetAll();
        User? GetById(string id);
        User? GetByUsername(string username);
        User? GetByEmail(string email);
        void Create(User user, string password);
        void Update(string id, User user, string? password);
        void Delete(string id);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
        User? AuthenticateUser(string username, string password);
    }
}

