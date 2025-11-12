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

        public void Create(User user, string password)
        {
            if (string.IsNullOrEmpty(user.Id))
                user.Id = ObjectId.GenerateNewId().ToString();

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

        public void Initialize()
        {
            try
            {
                var adminUser = GetByUsername("admin");
                var regularUser = GetByUsername("johndoe");
                var serviceDeskUser = GetByUsername("janesmith");

                if (adminUser == null)
                {
                    adminUser = new User
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        FirstName = "Admin",
                        LastName = "User",
                        Email = "admin@ticketing.com",
                        Username = "admin",
                        PasswordHash = HashPassword("Admin123!"),
                        Type = "manager",
                        Role = "ServiceDeskEmployee",
                        PhoneNumber = "",
                        Location = ""
                    };
                    _userRepo.Add(adminUser);
                    Console.WriteLine("Admin user created - Username: admin, Password: Admin123!");
                }
                else
                {
                    adminUser.PasswordHash = HashPassword("Admin123!");
                    adminUser.Role = "ServiceDeskEmployee";
                    _userRepo.Update(adminUser.Id, adminUser);
                    Console.WriteLine("Admin user updated - Username: admin, Password: Admin123!");
                }

                if (regularUser == null)
                {
                    regularUser = new User
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        FirstName = "John",
                        LastName = "Doe",
                        Email = "john.doe@ticketing.com",
                        Username = "johndoe",
                        PasswordHash = HashPassword("User123!"),
                        Type = "developer",
                        Role = "RegularEmployee",
                        PhoneNumber = "",
                        Location = ""
                    };
                    _userRepo.Add(regularUser);
                    Console.WriteLine("Regular user created - Username: johndoe, Password: User123!");
                }
                else
                {
                    regularUser.PasswordHash = HashPassword("User123!");
                    regularUser.Role = "RegularEmployee";
                    _userRepo.Update(regularUser.Id, regularUser);
                    Console.WriteLine("Regular user updated - Username: johndoe, Password: User123!");
                }

                if (serviceDeskUser == null)
                {
                    serviceDeskUser = new User
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        FirstName = "Jane",
                        LastName = "Smith",
                        Email = "jane.smith@ticketing.com",
                        Username = "janesmith",
                        PasswordHash = HashPassword("Service123!"),
                        Type = "support",
                        Role = "ServiceDeskEmployee",
                        PhoneNumber = "",
                        Location = ""
                    };
                    _userRepo.Add(serviceDeskUser);
                    Console.WriteLine("Service Desk user created - Username: janesmith, Password: Service123!");
                }
                else
                {
                    serviceDeskUser.PasswordHash = HashPassword("Service123!");
                    serviceDeskUser.Role = "ServiceDeskEmployee";
                    _userRepo.Update(serviceDeskUser.Id, serviceDeskUser);
                    Console.WriteLine("Service Desk user updated - Username: janesmith, Password: Service123!");
                }

                var allUsers = GetAll();
                foreach (var user in allUsers)
                {
                    if (user.Username != "admin" && user.Username != "johndoe" && user.Username != "janesmith")
                    {
                        bool updated = false;
                        if (string.IsNullOrEmpty(user.Role))
                        {
                            user.Role = "RegularEmployee";
                            updated = true;
                        }

                        if (string.IsNullOrEmpty(user.Username))
                        {
                            user.Username = user.Email?.Split('@')[0] ?? $"user{user.Id}";
                            updated = true;
                        }

                        if (string.IsNullOrEmpty(user.PasswordHash))
                        {
                            user.PasswordHash = HashPassword("TempPassword123!");
                            updated = true;
                        }

                        if (updated)
                        {
                            _userRepo.Update(user.Id, user);
                        }
                    }
                }

                Console.WriteLine("Seed data initialization completed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during seed initialization: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}

