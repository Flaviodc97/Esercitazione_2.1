using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Esercitazione_2._1.Models;
using Esercitazione_2._1.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class Program
{

    public static void Main()
    {
        string inputFilePath = Path.Combine(AppContext.BaseDirectory, "example.csv");
        string outputFilePath = Path.Combine(AppContext.BaseDirectory, "anomalie.csv");
        double threshold;
        int nMeasurement;
        bool flag = true;

        while (flag == true)
        {   
            Console.WriteLine("\nSelect method:");
            Console.WriteLine("1 - FindAnomaly");
            Console.WriteLine("2 - MergeAnomaly");
            Console.WriteLine("3 - MergeAnomaliesWithNASupport");
            Console.WriteLine("Any Other Key - Quit");
            Console.Write("Choice (1/2/3): ");

            string choice = Console.ReadLine();


            if (choice == "1" || choice == "2" || choice == "3")
            {
                Console.WriteLine("Insert a Threshold value");
                while (!double.TryParse(Console.ReadLine(), NumberStyles.Float, CultureInfo.InvariantCulture, out threshold))
                {
                    Console.Write("Not a valid threshold. Retry: ");
                }
               
                var rilevazioni = CsvReaderService.LoadCSV(inputFilePath);
                List<Anomaly> anomalies = new List<Anomaly>();

                switch (choice)
                {
                    case "1":
                        anomalies = AnomalyDetectionService.FindAnomaly(rilevazioni, threshold);
                        break;
                    case "2":
                        Console.WriteLine("Insert a Merge tolerance value");
                        while (!int.TryParse(Console.ReadLine(), NumberStyles.Integer, CultureInfo.InvariantCulture, out nMeasurement))
                        {
                            Console.Write("Not a valid integer. Retry: ");
                        }
                        anomalies = AnomalyDetectionService.MergeAnomaly(rilevazioni, threshold, nMeasurement);
                        break;
                    case "3":
                        Console.WriteLine("Insert a Merge tolerance value");
                        while (!int.TryParse(Console.ReadLine(), NumberStyles.Integer, CultureInfo.InvariantCulture, out nMeasurement))
                        {
                            Console.Write("Not a valid integer. Retry: ");
                        }
                        anomalies = AnomalyDetectionService.MergeAnomaliesWithNASupport(rilevazioni, threshold, nMeasurement);
                        break;
                }

                CSVWriterService.WriteAnomalies(anomalies, outputFilePath);
                Console.WriteLine($"Anomalies written to {outputFilePath}");
            }
            else
            {
                Console.WriteLine("Exiting.");
                flag = false;
            }
        }
    }
}
