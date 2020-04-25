using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }

        public string Username { get; set; }

        public string RequestName { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public DateTime? OrderedDate { get; set; }

        public string NeedId { get; set; }

    }
}
