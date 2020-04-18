using Models;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KokuaApi.Models
{
    public class UserDataResponse
    {

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Address { get; set; }

        public DateTime? Age { get; set; }

        public string PhoneNumber { get; set; }

        [DataType(DataType.ImageUrl)]
        public string ProfileImage { get; set; }

        public string WhoAmI { get; set; }

        public IEnumerable<object> Needs { get; set; }

        public int NeedsCount { get; set; }
    }

    public class UserDataPostResponse
    {

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Address { get; set; }

        public DateTime? Age { get; set; }

        public string PhoneNumber { get; set; }

        [DataType(DataType.ImageUrl)]
        public string ProfileImage { get; set; }

        public string WhoAmI { get; set; }
    }
}
