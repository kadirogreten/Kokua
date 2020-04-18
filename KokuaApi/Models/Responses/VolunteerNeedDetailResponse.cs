using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KokuaApi.Models
{
    public class VolunteerNeedDetailResponse
    {
        public string NeedId { get; set; }
        public List<NeedProducts> NeedProducts { get; set; }
        public int ProductsCount { get; set; }
        public string BeneficiaryNameSurname { get; set; }
    }
}
