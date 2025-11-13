using NoSQL_project.Models;

namespace NoSQL_project.Repositories.Interfaces
{
        public interface IUserRepository
        {
        List<User> GetAll();
        void Add(User user);
        User? GetById(string id);
        void Update(string id, User user);
            void Delete(string id);
        User? GetByUsername(string username);
        User? GetByEmail(string email);
        }
}
