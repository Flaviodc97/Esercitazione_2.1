using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Esercitazione_2._1.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class Program 
{
    const double DISTANZA_TRA_MISURE_METRI = 0.5;
    public static void Main()
    {
        double soglia = 3.0;
        string outputFilePath = "anomalie.csv";
        string filePath = "C:\\Users\\fdellacorte\\source\\repos\\Esercitazione_2.1\\Esercitazione_2.1\\example.csv";
        var rilevations = new List<Coordinate>();
        try
        {
            using (var reader = new StreamReader(filePath, Encoding.UTF8))
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
                    var coordinate = new Coordinate
                    {
                        Id = int.Parse(campi[0]),
                        Latitude = campi[1],
                        Longitude = campi[2],
                        Degradation = double.Parse(campi[3], CultureInfo.InvariantCulture)
                    };
                    rilevations.Add(coordinate);
                }
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Il file non è stato trovato: " + filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante l'apertura del file: " + ex.Message);
        }

        var anomalie = AnomaliesCSV(rilevations, soglia);
        foreach (var anomalia in anomalie)
        {
            Console.WriteLine(anomalia);
        }
    }

    private static List<Anomalia> AnomaliesCSV(List<Coordinate> rilevations, double soglia)
    {
        double test = 0;
        int inizioId = 0;
        int maxId = 0;
        double maxDeg = 0;
        int nDeg = 0;
        double lunghezza = 0;
        List<LatLong> listDif = new List<LatLong>();
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
                listDif.Add(new LatLong { Latitude = rilevations[i].Latitude, Longitude = rilevations[i].Longitude });
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
                        Lunghezza = double.IsNaN(lunghezza) ? "NA" : (lunghezza * nDeg).ToString()
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
            var anomalia = new Anomalia
            {
                InizioId = inizioId,
                FineId = fine.Id,
                LatInizio = latInizio,
                LonInizio = lonInizio,
                LatFine = fine.Latitude,
                LonFine = fine.Longitude,
                MaxId = maxId,
                MaxValore= maxDeg,
                Lunghezza = double.IsNaN(lunghezza) ? "NA" : (Math.Round(lunghezza * nDeg,4)).ToString()
                
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
        double R = 6371000; // raggio della Terra in metri

        // Converti in radianti
        double radLat1 = lat1 * Math.PI / 180.0;
        double radLon1 = lon1 * Math.PI / 180.0;
        double radLat2 = lat2 * Math.PI / 180.0;
        double radLon2 = lon2 * Math.PI / 180.0;

        double deltaLat = radLat2 - radLat1;
        double deltaLon = radLon2 - radLon1;

        double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                   Math.Cos(radLat1) * Math.Cos(radLat2) *
                   Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c; 
    }


}
