using Esercitazione_2._1.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esercitazione_2._1.Services
{
    public class CSVWriterService
    {
        public static void WriteAnomalies(List<Anomaly> anomalie, string path)
        {
            var sb = new StringBuilder();
            sb.AppendLine("FirstId,FirstLat,FirstLon,EndId,EndLat,EndLon,MaxId,MaxValore,Length,LinearDistance");

            foreach (var a in anomalie)
            {
                sb.AppendLine(string.Join(",",
                    a.FirstId,
                    a.FirstLat,
                    a.FirstLon,
                    a.EndId,
                    a.EndLat,
                    a.EndLon,
                    a.MaxId,
                    a.MaxValore.ToString("F4", CultureInfo.InvariantCulture),
                    a.Length,
                    a.LinearDistance)
                    );
            }

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }
    }
}
