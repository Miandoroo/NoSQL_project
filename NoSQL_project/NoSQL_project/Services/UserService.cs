using BCrypt.Net;
using MongoDB.Bson;
using NoSQL_project.Models;
using NoSQL_project.Repositories.Interfaces;
using NoSQL_project.Services.Interfaces;

namespace NoSQL_project.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public List<User> GetAll()
        {
            return _userRepo.GetAll();
        }

        public User? GetById(string id)
        {
            return _userRepo.GetById(id);
        }

        public User? GetByUsername(string username)
        {
            return _userRepo.GetByUsername(username);
        }

        public User? GetByEmail(string email)
        {
            return _userRepo.GetByEmail(email);
        }

        /// Crud Operations
        public void Create(User user, string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                user.PasswordHash = HashPassword(password);
            }
            else
            {
                throw new ArgumentException("Password is required");
            }
            _userRepo.Add(user);
        }

        public void Update(string id, User user, string? password)
        {
            var existingUser = _userRepo.GetById(id);
            if (existingUser == null)
                throw new ArgumentException("User not found");

            user.Id = id;
            if (!string.IsNullOrEmpty(password))
            {
                user.PasswordHash = HashPassword(password);
            }
            else
            {
                user.PasswordHash = existingUser.PasswordHash;
            }

            _userRepo.Update(id, user);
        }

        public void Delete(string id)
        {
            var user = _userRepo.GetById(id);
            try {
                if (user == null)
                    throw new ArgumentException("User not found");
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            _userRepo.Delete(id);
        }
        /// Password Hashing and Verification
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }

        public User? AuthenticateUser(string username, string password)
        {
            var user = GetByUsername(username);
            if (user == null)
                return null;

            if (!VerifyPassword(password, user.PasswordHash))
                return null;

            return user;
        }

    }
}

