using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Needs
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("Title")]
        public string Title { get; set; }

        public string Username { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? AcceptedDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public IEnumerable<NeedProducts> NeedProducts { get; set; }
    }






}
