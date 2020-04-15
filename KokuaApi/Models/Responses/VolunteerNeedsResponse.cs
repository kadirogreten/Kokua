using Models;
using System;
using System.Collections.Generic;

namespace KokuaApi.Models
{
    public class VolunteerNeedsResponse
    {
        public string Title { get; set; }

        public string BeneficiaryUsername { get; set; }

        public DateTime CreatedAt { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public IEnumerable<NeedProducts> NeedProducts { get; set; }
    }
    public class TakeNeedsResponse
    {
        public string Id { get; set; }

        public string BeneficiaryUsername { get; set; }

        public string VolunteerUsername { get; set; }

        public DateTime AcceptedAt { get; set; }

        public OrderStatus OrderStatus { get; set; } = OrderStatus.Accepted;
    }
}