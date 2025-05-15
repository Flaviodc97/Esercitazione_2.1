using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Esercitazione_2._1.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class Program 
{
    
    public static void Main()
    {
        string outputFilePath = Path.Combine(AppContext.BaseDirectory, "anomalie.csv");
        string inputFilePath = Path.Combine(AppContext.BaseDirectory, "example.csv");
        double soglia;

        while (!double.TryParse(Console.ReadLine(), NumberStyles.Float, CultureInfo.InvariantCulture, out soglia))
        {
            Console.Write("Valore non valido. Riprova: ");
        }

        var rilevazioni = CaricaCSV(inputFilePath);
        
        var anomalie = AnomaliesCSV(rilevazioni, soglia);
        
        ExportAnomalieCSV(anomalie, outputFilePath);
    }
    private static List<MonitoredLocation> CaricaCSV(string inputFilePath)
    {
        
        try
        {
            var rilevazioni = new List<MonitoredLocation>();
            using (var reader = new StreamReader(inputFilePath, Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var campi = line.Split(',');

                    if (campi.Length != 4)
                    {
                        Console.WriteLine($"Formato riga non valido: {line}");
                        continue;
                    }
                    if (!int.TryParse(campi[0], out int id))
                    {
                        Console.WriteLine($"ID non valido: {campi[0]}");
                        continue;
                    }

                    string lat = campi[1];
                    string lon = campi[2];

                    if (!double.TryParse(campi[3], NumberStyles.Float, CultureInfo.InvariantCulture, out double degrado))
                    {
                        Console.WriteLine($"Valore degrado non valido: {campi[3]}");
                        continue;
                    }
                    var coordinate = new MonitoredLocation
                    {
                        Id = int.Parse(campi[0]),
                        Latitude = campi[1],
                        Longitude = campi[2],
                        Degradation = double.Parse(campi[3], CultureInfo.InvariantCulture)
                    };
                    rilevazioni.Add(coordinate);
                }
            }

            return rilevazioni;
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Il file non è stato trovato: " + inputFilePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante l'apertura del file: " + ex.Message);
        }
        return new List<MonitoredLocation>();
    }
    private static void ExportAnomalieCSV(List<Anomalia> anomalie, string outputFilePath)
    {

        var sb = new StringBuilder();

        // Header CSV
        sb.AppendLine("InizioId,LatInizio,LonInizio,FineId,LatFine,LonFine,MaxId,MaxValore,Lunghezza,DistanzaLineare ");

        foreach (var anomalia in anomalie)
        {
            sb.AppendLine(string.Join(",",
                anomalia.InizioId,
                anomalia.LatInizio,
                anomalia.LonInizio,
                anomalia.FineId,
                anomalia.LatFine,
                anomalia.LonFine,
                anomalia.MaxId,
                anomalia.MaxValore.ToString("F4", CultureInfo.InvariantCulture),   // usa punto come separatore decimale
                anomalia.Lunghezza,
                anomalia.DistanzaLineare
            ));
        }

        // Scrivi su file
        File.WriteAllText(outputFilePath, sb.ToString(), Encoding.UTF8);
    }


    private static List<Anomalia> AnomaliesCSV(List<MonitoredLocation> rilevations, double soglia)
    {
        double test = 0;
        int inizioId = 0;
        int maxId = 0;
        double maxDeg = 0;
        int nDeg = 0;
        double lunghezza = 0;
        List<Coordinate> listDif = new List<Coordinate>();
        string latInizio = null, lonInizio = null;
        var anamalie = new List<Anomalia>();
        for (int i = 0; i < rilevations.Count; i++)
        {
            if (Math.Abs(rilevations[i].Degradation) > soglia)
            {
                if (inizioId == 0)
                {
                    inizioId = rilevations[i].Id;
                    latInizio = rilevations[i].Latitude;
                    lonInizio = rilevations[i].Longitude;
                    maxId = rilevations[i].Id;
                    maxDeg = rilevations[i].Degradation;

                }
                if (maxDeg < rilevations[i].Degradation)
                {
                    maxDeg = rilevations[i].Degradation;
                    maxId = rilevations[i].Id;
                }
                nDeg++;
                listDif.Add(new Coordinate { Latitude = rilevations[i].Latitude, Longitude = rilevations[i].Longitude });
                if (nDeg >= 2)
                { 
                    if (listDif[^2].Latitude.Contains("NA") || listDif[^1].Latitude.Contains("NA"))
                        lunghezza += 0;
                    else
                        lunghezza += CalcolaDistanza(DMSToDecimal(listDif[^2].Latitude), DMSToDecimal(listDif[^2].Longitude), DMSToDecimal(listDif[^1].Latitude), DMSToDecimal(listDif[^1].Longitude));

                }
            }
            else
            {
                if (inizioId != 0)
                {
                    var lastRilevation = rilevations[i - 1];
                    var distanza = CalcolaDistanza(DMSToDecimal(latInizio), DMSToDecimal(lonInizio), DMSToDecimal(lastRilevation.Latitude), DMSToDecimal(lastRilevation.Longitude));
                    var anomalia = new Anomalia()
                    {
                        InizioId = inizioId,
                        LatInizio = latInizio,
                        LonInizio = lonInizio,
                        FineId = lastRilevation.Id,
                        LatFine = lastRilevation.Latitude,
                        LonFine = lastRilevation.Longitude,
                        MaxId = maxId,
                        MaxValore = maxDeg,
                        Lunghezza = double.IsNaN(lunghezza) ? "NA" : (lunghezza * nDeg).ToString("F4", CultureInfo.InvariantCulture),
                        DistanzaLineare = double.IsNaN(distanza) ? "NA" : distanza.ToString("F4", CultureInfo.InvariantCulture)
                    };
                    anamalie.Add(anomalia);
                    inizioId = 0;
                    latInizio = null;
                    lonInizio = null;
                    maxId = 0;
                    maxDeg = 0;
                    lunghezza = 0;
                    nDeg = 0;
                    listDif.Clear();

                }
            }
        }

        if (inizioId != 0)
        {
            var fine = rilevations[^1];
            var distanza = CalcolaDistanza(DMSToDecimal(latInizio), DMSToDecimal(lonInizio), DMSToDecimal(fine.Latitude), DMSToDecimal(fine.Longitude));
            var anomalia = new Anomalia
            {
                InizioId = inizioId,
                FineId = fine.Id,
                LatInizio = latInizio,
                LonInizio = lonInizio,
                LatFine = fine.Latitude,
                LonFine = fine.Longitude,
                MaxId = maxId,
                MaxValore = maxDeg,
                Lunghezza = double.IsNaN(lunghezza) ? "NA" : (Math.Round(lunghezza * nDeg, 4)).ToString("F4", CultureInfo.InvariantCulture),
                DistanzaLineare = double.IsNaN(distanza) ? "NA" : distanza.ToString("F4", CultureInfo.InvariantCulture)
            };
            anamalie.Add(anomalia);
        }
        return anamalie;
    }


    private static double DMSToDecimal(string dms)
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

    public static double CalcolaDistanza(double lat1, double lon1, double lat2, double lon2)
    {
        double R = 6371000; // Raggio della Terra in metri

        // Converti in radianti
        double φ1 = lat1 * Math.PI / 180.0;
        double φ2 = lat2 * Math.PI / 180.0;
        double λ1 = lon1 * Math.PI / 180.0;
        double λ2 = lon2 * Math.PI / 180.0;

        double x = (λ2 - λ1) * Math.Cos((φ1 + φ2) / 2);
        double y = φ2 - φ1;

        return Math.Sqrt(x * x + y * y) * R;
    }


}
