using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{

    public class UserReport
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }
        public string ReportDoUsername { get; set; }       
        public string ReportToUsername { get; set; }
        public string Detail { get; set; }
        public ReportType ReportType { get; set; }
        public ReportSubject ReportSubject { get; set; }
        public ReportStatus ReportStatus { get; set; } = 0;

    }
}
