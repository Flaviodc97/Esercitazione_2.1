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

            try
            {
                File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO error while writing to file: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied to file path: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error during CSV writing: {ex.Message}");
            }
        }
    }
}
