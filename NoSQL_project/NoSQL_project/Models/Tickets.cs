using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace NoSQL_project.Models
{
    public class Tickets
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string date { get; set; }
        public int status { get; set; }
        public string incidentSubject { get; set; }

        public string incidentType { get; set; }
        public string deadline { get; set; }
        public string priority { get; set; }
        public string description { get; set; }
        public string userId { get; set; }
    }
}
