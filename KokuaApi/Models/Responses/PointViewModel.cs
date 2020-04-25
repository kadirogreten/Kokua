using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KokuaApi.Models
{
    public class PointViewModel
    {

        public PointViewModel(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double GetDistanceTo(PointViewModel model)
        {
            if (double.IsNaN(model.Latitude) || double.IsNaN(model.Longitude) || double.IsNaN(Latitude) ||
                double.IsNaN(Longitude))
            {
                throw new ArgumentException("Argument latitude or longitude is not a number");
            }

            var d1 = model.Latitude * (Math.PI / 180.0);
            var num1 = model.Longitude * (Math.PI / 180.0);
            var d2 = Latitude * (Math.PI / 180.0);
            var num2 = Longitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                     Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }

    }

}
