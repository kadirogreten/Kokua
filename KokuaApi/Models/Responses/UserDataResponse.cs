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

        public IEnumerable<UserProfileNeedResponse> Needs { get; set; }

        public int NeedsCount { get; set; }
    }

    public class UserProfileNeedResponse
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Username { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public double Latitude { get; set; }

        public double Longitude { get; set; }
        public IEnumerable<UserProfileNeedProductResponse> NeedProducts { get; set; }
    }

    public class UserProfileNeedProductResponse
    {
        public string Id { get; set; }
        public string ProductDescription { get; set; }
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
