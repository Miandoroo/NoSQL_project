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
            return _userss.Find(_ => true).ToList();
        }

        public Users? GetById(string id)
        {
            return _userss.Find(user => user.Id == id).FirstOrDefault();
        }

        public void Add(Users user)
        {
            _userss.InsertOne(user);
        }

    }
}
