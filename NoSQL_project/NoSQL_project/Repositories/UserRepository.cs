using MongoDB.Driver;
using MongoDB.Bson;
using NoSQL_project.Models;
using NoSQL_project.Repositories.Interfaces;

namespace NoSQL_project.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoDatabase db)
        {
            _users = db.GetCollection<User>("Users");
        }

        public List<User> GetAll()
        {
            return _users.Find(_ => true).ToList();
        }

        public User? GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            
            if (ObjectId.TryParse(id, out var objectId))
        {
                return _users.Find(user => user.Id == id || user.Id == objectId.ToString()).FirstOrDefault();
            }
            
            return _users.Find(user => user.Id == id).FirstOrDefault();
        }

        public void Add(User user)
        {
            _users.InsertOne(user);
        }

        public void Update(string id, User user)
        {
            _users.ReplaceOne(u => u.Id == id, user);
        }

        public void Delete(string id)
        {
            _users.DeleteOne(user => user.Id == id);
        }

        public User? GetByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;
            return _users.Find(user => user.Username != null && user.Username.ToLower() == username.ToLower()).FirstOrDefault();
        }

        public User? GetByEmail(string email)
        {
            return _users.Find(user => user.Email == email).FirstOrDefault();
        }
    }
}
