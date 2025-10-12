using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace NoSQL_project.Models
{
    public class Tickets
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("date")]
        public string Date { get; set; }
        [BsonElement("status")]
        public int Status { get; set; }
        [BsonElement("incidentSubject")]
        public string IncidentSubject { get; set; }
        [BsonElement("incidentType")]
        public string IncidentType { get; set; }
        [BsonElement("deadline")]
        public string Deadline { get; set; }
        [BsonElement("priority")]
        public string Priority { get; set; }
        [BsonElement("description")]
        public string Description { get; set; }
        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        public Tickets(string id, string date, int status, string incidentSubject, string incidentType, string deadline, string priority, string description, string userId)
        {
            Id = id;
            Date = date;
            Status = status;
            IncidentSubject = incidentSubject;
            IncidentType = incidentType;
            Deadline = deadline;
            Priority = priority;
            Description = description;
            UserId = userId;
        }
    }
}
