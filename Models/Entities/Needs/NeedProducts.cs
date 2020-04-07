using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class NeedProducts
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("ProductDescription")]
        public string ProductDescription { get; set; }
        //public string ProductDescription { get; set; }
       // public Units Units { get; set; }

    }
}
