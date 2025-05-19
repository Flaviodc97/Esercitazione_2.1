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
        string inputFilePath = Path.Combine(Directory.GetCurrentDirectory(), "src\\example.csv");
        string outputFilePath = Path.Combine(Directory.GetCurrentDirectory(), "src\\anomalie.csv");
        double threshold;
        int nMeasurement;
        bool flag = true;

        while (flag == true)
        {   
            Console.WriteLine("Select method:");
            Console.WriteLine("1 - FindAnomaly - Es1");
            Console.WriteLine("2 - MergeAnomaly - Es2");
            Console.WriteLine("3 - MergeAnomaliesWithNASupport -Es4");
            Console.WriteLine("4 - MergeAnomaliesParallel -Es6");
            Console.WriteLine("Any Other Key - Quit");
            Console.Write("Choice (1/2/3): ");

            string choice = Console.ReadLine();


            if (choice == "1" || choice == "2" || choice == "3" || choice == "4")
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
                        anomalies = AnomalyDetectionService.FindAnomalies(rilevazioni, threshold);
                        CSVWriterService.WriteAnomalies(anomalies, outputFilePath.Replace(".csv", "_Es1.csv"));
                        Console.WriteLine($"Anomalies written.");
                        break;
                    case "2":
                        Console.WriteLine("Insert a Merge tolerance value");
                        while (!int.TryParse(Console.ReadLine(), NumberStyles.Integer, CultureInfo.InvariantCulture, out nMeasurement))
                        {
                            Console.Write("Not a valid integer. Retry: ");
                        }
                        anomalies = AnomalyDetectionService.MergeAnomalies(rilevazioni, threshold, nMeasurement);
                        CSVWriterService.WriteAnomalies(anomalies, outputFilePath.Replace(".csv", "_Es2.csv"));
                        Console.WriteLine($"Anomalies written.");
                        break;
                    case "3":
                        Console.WriteLine("Insert a Merge tolerance value");
                        while (!int.TryParse(Console.ReadLine(), NumberStyles.Integer, CultureInfo.InvariantCulture, out nMeasurement))
                        {
                            Console.Write("Not a valid integer. Retry: ");
                        }
                        anomalies = AnomalyDetectionService.MergeAnomaliesWithNASupport(rilevazioni, threshold, nMeasurement);
                        CSVWriterService.WriteAnomalies(anomalies, outputFilePath.Replace(".csv", "_Es4.csv"));
                        Console.WriteLine($"Anomalies written.");
                        break;
                    case "4":
                        Console.WriteLine("Insert a Merge tolerance value");
                        while (!int.TryParse(Console.ReadLine(), NumberStyles.Integer, CultureInfo.InvariantCulture, out nMeasurement))
                        {
                            Console.Write("Not a valid integer. Retry: ");
                        }
                        anomalies = AnomalyDetectionService.MergeAnomalies(rilevazioni, threshold, nMeasurement);
                        CSVWriterService.WriteAnomalies(anomalies, outputFilePath.Replace(".csv", "_Es6.csv"));
                        Console.WriteLine($"Anomalies written.");
                        break;
                }

                
            }
            else
            {
                Console.WriteLine("Exiting.");
                flag = false;
            }
        }
    }
}
