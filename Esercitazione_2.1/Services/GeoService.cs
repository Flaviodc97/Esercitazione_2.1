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
        public static double DistanceCalculation(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            //Earth's radius in meters
            double earthRadius = 6371000;

            //Convert to radians
            double latRad1 = latitude1 * Math.PI / 180.0;
            double latRad2 = latitude2 * Math.PI / 180.0;
            double lonRad1 = longitude1 * Math.PI / 180.0;
            double lonRad2 = longitude2 * Math.PI / 180.0;

            //Calculate differences
            double deltaX = (lonRad2 - lonRad1) * Math.Cos((latRad1 + latRad2) / 2);
            double deltaY = latRad2 - latRad1;

            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY) * earthRadius;
        }
    }
}
