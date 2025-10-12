using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
namespace NoSQL_project.Models
{
    public class Users
    {
        [BsonId]
        [BsonRepresentation(BsonType.Binary)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Location {  get; set; }
        public string Type { get; set; }

        public Users(string id, string firstName, string lastName, string email, string phoneNumber, string location, string type)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Location = location;
            Type = type;
        }
    }
}
