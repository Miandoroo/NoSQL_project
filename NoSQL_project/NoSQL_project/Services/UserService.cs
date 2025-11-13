using BCrypt.Net;
using MongoDB.Bson;
using NoSQL_project.Models;
using NoSQL_project.Models.ViewModels;
using NoSQL_project.Repositories.Interfaces;
using NoSQL_project.Services.Interfaces;
using NoSQL_project.Enum;
using System.Security.Claims;

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
            if (user == null)
                throw new ArgumentException("User not found");
            
            _userRepo.Delete(id);
        }

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

        public List<Claim> CreateClaims(User user)
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
        }

        public User CreateUserFromRegister(RegisterViewModel model)
        {
            return new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Username = model.Username,
                PhoneNumber = model.PhoneNumber ?? "",
                Location = model.Location ?? "",
                Type = model.Type ?? "",
                Role = UserRoles.RegularEmployee
            };
        }

        public bool ValidateUserRegistration(string username, string email, out string? errorMessage)
        {
            errorMessage = null;

            var existingUserByUsername = GetByUsername(username);
            if (existingUserByUsername != null)
            {
                errorMessage = "Username is already taken";
                return false;
            }

            var existingUserByEmail = GetByEmail(email);
            if (existingUserByEmail != null)
            {
                errorMessage = "Email is already registered";
                return false;
            }

            return true;
        }

    }
}

