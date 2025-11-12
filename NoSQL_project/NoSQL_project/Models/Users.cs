using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NoSQL_project.Models
{
    public class Users
    {
        public Users() { }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("firstName")]
        public string FirstName { get; set; }
        [BsonElement("lastName")]
        public string LastName { get; set; }
        [BsonElement("email")]
        public string Email { get; set; }
        [BsonElement("phoneNumber")]
        public string PhoneNumber { get; set; }
        [BsonElement("location")]
        public string Location { get; set; }
        [BsonElement("type")]
        public string Type { get; set; }
        [BsonElement("role")]
        public string Role { get; set; }
        [BsonElement("username")]
        public string Username { get; set; }
        [BsonElement("passwordHash")]
        public string password
        {
            get; set;
        }

        public Users(string id, string firstName, string lastName, string email, string phoneNumber, string location, string type, string role, string username, string password)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Location = location;
            Type = type;
            Role = role;
            Username = username;
            this.password = password;
        }
    }
}
