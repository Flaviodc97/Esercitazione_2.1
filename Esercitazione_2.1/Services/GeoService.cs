using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esercitazione_2._1.Services
{
    public class GeoService
    {
        public static double DMSToDecimal(string dms)
        {
            var parts = dms.Trim().Split(' ');
            if (parts.Length < 2 || parts[1] == "NA") return double.NaN;

            char direction = parts[0][0];

            string[] dmsParts = parts[1].Replace("°", " ").Replace("'", " ").Replace("\"", "").Split(' ');
            double degrees = double.Parse(dmsParts[0], CultureInfo.InvariantCulture);
            double minutes = double.Parse(dmsParts[1], CultureInfo.InvariantCulture);
            double seconds = double.Parse(dmsParts[2], CultureInfo.InvariantCulture);

            double decimalDegrees = degrees + (minutes / 60) + (seconds / 3600);

            if (direction == 'S' || direction == 'W')
            {
                decimalDegrees *= -1;
            }

            return decimalDegrees;
        }
        public static double DistanceCalculation(double lat1, double lon1, double lat2, double lon2)
        {
            //Earth's radius in meters
            double R = 6371000;

            //Convert to radians
            double φ1 = lat1 * Math.PI / 180.0;
            double φ2 = lat2 * Math.PI / 180.0;
            double λ1 = lon1 * Math.PI / 180.0;
            double λ2 = lon2 * Math.PI / 180.0;

            double x = (λ2 - λ1) * Math.Cos((φ1 + φ2) / 2);
            double y = φ2 - φ1;

            return Math.Sqrt(x * x + y * y) * R;
        }
    }
}
