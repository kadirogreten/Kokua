using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Entities
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string VolunteerName { get; set; }

        public string BeneficiaryName { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? AcceptedDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public IEnumerable<Needs> Needs { get; set; }

    }
}
