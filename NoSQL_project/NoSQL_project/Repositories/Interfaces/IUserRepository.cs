using NoSQL_project.Models;

namespace NoSQL_project.Repositories.Interfaces
{
        public interface IUserRepository
        {
            List<Users> GetAll();
            void Add(Users user);
            Users? GetById(string id);
            //void Update(Users user);
            void Delete(string id);
            List<Users> GetByType(string type);
            List<Users> GetByLocation(string location);
        }
    
}
