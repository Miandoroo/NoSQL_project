using MongoDB.Driver;
using NoSQL_project.Models;
using NoSQL_project.Repositories.Interfaces;


namespace NoSQL_project.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<Users> _userss;
        public UserRepository(IMongoDatabase db)
        {
            _userss = db.GetCollection<Users>("Users");
        }

        public List<Users> GetAll()
        {
            List<Users> users = _userss.Find(FilterDefinition<Users>.Empty)
                .ToList();
            return users;
        }

    }
}
