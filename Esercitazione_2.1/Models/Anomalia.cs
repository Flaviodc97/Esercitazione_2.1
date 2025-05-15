using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esercitazione_2._1.Models
{
    public class Anomalia
    {
        public int InizioId { get; set; }
        public int FineId { get; set; }
        public string LatInizio { get; set; }
        public string LonInizio { get; set; }
        public string LatFine { get; set; }
        public string LonFine { get; set; }
        public int MaxId { get; set; }
        public double MaxValore { get; set; }
        public string Lunghezza { get; set; }
        public string DistanzaLineare { get; set; }

        public override string ToString()
        {
            return $"{{ \"InizioId\": {InizioId},\n \"FineId\": {FineId}, \n" +
           $"\"LatInizio\": \"{LatInizio}\", \n\"LonInizio\": \"{LonInizio}\", \n" +
           $"\"LatFine\": \"{LatFine}\", \n\"LonFine\": \"{LonFine}\" \n" +
           $"\"MaxId\": \"{MaxId}\",\n \"MaxValore\": \"{MaxValore}\" \n" +
           $"\"Lunghezza\": \"{Lunghezza}\",\n \"Distanza\": \"{DistanzaLineare}\" \n}}";

        }
    }

}
