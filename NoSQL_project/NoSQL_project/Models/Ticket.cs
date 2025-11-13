using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using NoSQL_project.Enum;

namespace NoSQL_project.Models
{
    public class Ticket
    {
        public Ticket() { }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("date")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime Date { get; set; }

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public TicketStatus status { get; set; }

        [BsonElement("incidentSubject")]
        public string IncidentSubject { get; set; }

        [BsonElement("incidentType")]
        [BsonRepresentation(BsonType.String)]
        public TicketIncidentType IncidentType { get; set; }

        [BsonElement("deadline")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime? Deadline { get; set; }

        [BsonElement("priority")]
        [BsonRepresentation(BsonType.String)]
        public TicketPrioritys Priority { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        public Ticket(string id, DateTime date, TicketStatus status, string incidentSubject, TicketIncidentType incidentType, DateTime? deadline, TicketPrioritys priority, string description, string userId)
        {
            Id = id;
            Date = date;
            this.status = status;
            IncidentSubject = incidentSubject;
            IncidentType = incidentType;
            Deadline = deadline;
            Priority = priority;
            Description = description;
            UserId = userId;
        }
    }
}
