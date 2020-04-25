using Models;
using System;
using System.Collections.Generic;

namespace KokuaApi.Models
{
    public class VolunteerNeedsResponse
    {
        public string NeedId { get; set; }

        public string Title { get; set; }

        public string BeneficiaryUsername { get; set; }

        public string BeneficiaryNameSurname { get; set; }

        public string ProfileImage { get; set; }

        public DateTime CreatedAt { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public LocationViewModel Location  { get; set; }

        public double Distance { get; set; }
    }


    public class TakeNeedsResponse
    {
        public string NeedId { get; set; }
    }


    public class TakeNeedProductResponse
    {
        public string ProductId { get; set; }
        public string NeedId { get; set; }
    }
}