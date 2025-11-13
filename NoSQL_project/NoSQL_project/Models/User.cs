using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using NoSQL_project.Enum;

namespace NoSQL_project.Models
{
    public class User
    {
        public User() { }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [BsonElement("lastName")]
        public string LastName { get; set; }

        [BsonElement("email")]
        [EmailAddress]
        public string Email { get; set; }

        [BsonElement("phoneNumber")]
        public string PhoneNumber { get; set; }

        [BsonElement("location")]
        public string Location { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("role")]
        [Required]
        public UserRoles Role { get; set; }

        [BsonElement("username")]
        [Required]
        public string Username { get; set; }

        [BsonElement("passwordHash")]
        [Required]
        public string? PasswordHash { get; set; }
    }
}
