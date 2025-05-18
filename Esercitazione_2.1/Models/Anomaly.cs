using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esercitazione_2._1.Models
{
    public class Anomaly
    {
        public int FirstId { get; set; }
        public int EndId { get; set; }
        public string FirstLat { get; set; }
        public string FirstLon { get; set; }
        public string EndLat { get; set; }
        public string EndLon { get; set; }
        public int MaxId { get; set; }
        public double MaxValore { get; set; }
        public string Length { get; set; }
        public string LinearDistance { get; set; }

        
    }

}
