using Esercitazione_2._1.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esercitazione_2._1.Services
{
    public class CsvReaderService
    {
        public static List<MonitoredLocation> LoadCSV(string path)
        {
            var measurements = new List<MonitoredLocation>();

            try
            {
                using var reader = new StreamReader(path, Encoding.UTF8);

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var fields = line.Split(',');
                    if (fields.Length != 4) continue;

                    if (!int.TryParse(fields[0], out int id)) continue;
                    if (!double.TryParse(fields[3], NumberStyles.Float, CultureInfo.InvariantCulture, out double degrado)) continue;

                    measurements.Add(new MonitoredLocation
                    {
                        Id = id,
                        Latitude = fields[1],
                        Longitude = fields[2],
                        Degradation = degrado
                    });
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File Not Found: " + path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Reading the file: {ex.Message}");
            }

            return measurements;
        }
    }
}
