using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KokuaApi.Models
{
    public class AddNeedResponse
    {
        public string Title { get; set; }
        public string[] ProductDescription { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Note { get; set; }
    }
}
