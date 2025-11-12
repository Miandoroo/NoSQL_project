using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace NoSQL_project.Models
{
    [BsonIgnoreExtraElements]
    public class Tickets
    {
        public Tickets() { }

        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? UserId { get; set; }

        [BsonElement("incidentSubject")]
        [Required] public string IncidentSubject { get; set; }

        [BsonElement("incidentType")]
        [Required] public string IncidentType { get; set; }

        [BsonElement("priority")]
        public string Priority { get; set; }

        [BsonElement("status")]
        public int Status { get; set; }

        [BsonElement("date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [BsonElement("deadline")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Deadline { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }
    }
}
