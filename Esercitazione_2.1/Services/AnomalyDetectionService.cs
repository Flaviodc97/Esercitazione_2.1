using Esercitazione_2._1.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esercitazione_2._1.Services
{
    public class AnomalyDetectionService
    {
        public static List<Anomaly> FindAnomaly(List<MonitoredLocation> measurements, double threshold)
        {
            int firstId = 0, maxId = 0;           
            double maxDeg = 0, length = 0;
            int nDeg = 0;
            string startLat = null, startLon = null;
            List<Coordinate> listDif = new List<Coordinate>();
            var anomalies = new List<Anomaly>();

            for (int i = 0; i < measurements.Count; i++)
            {
                if (Math.Abs(measurements[i].Degradation) > threshold)
                {
                    if (firstId == 0)
                    {
                        firstId = measurements[i].Id;
                        startLat = measurements[i].Latitude;
                        startLon = measurements[i].Longitude;
                        maxId = measurements[i].Id;
                        maxDeg = measurements[i].Degradation;

                    }
                    if (maxDeg < measurements[i].Degradation)
                    {
                        maxDeg = measurements[i].Degradation;
                        maxId = measurements[i].Id;
                    }
                    
                    nDeg++;
                    listDif.Add(new Coordinate { Latitude = measurements[i].Latitude, Longitude = measurements[i].Longitude });
                    
                    if (nDeg >= 2)
                    {
                        if (listDif[^2].Latitude.Contains("NA") || listDif[^1].Latitude.Contains("NA"))
                            length += 0;
                        else
                            length += GeoService.DistanceCalculation(GeoService.DMSToDecimal(listDif[^2].Latitude), GeoService.DMSToDecimal(listDif[^2].Longitude), GeoService.DMSToDecimal(listDif[^1].Latitude), GeoService.DMSToDecimal(listDif[^1].Longitude));

                    }
                }
                else
                {
                    if (firstId != 0)
                    {
                        var lastRilevation = measurements[i - 1];
                        var distanza = GeoService.DistanceCalculation(GeoService.DMSToDecimal(startLat), GeoService.DMSToDecimal(startLon), GeoService.DMSToDecimal(lastRilevation.Latitude), GeoService.DMSToDecimal(lastRilevation.Longitude));
                        var anomalia = new Anomaly()
                        {
                            FirstId = firstId,
                            FirstLat = startLat,
                            FirstLon = startLon,
                            EndId = lastRilevation.Id,
                            EndLat = lastRilevation.Latitude,
                            EndLon = lastRilevation.Longitude,
                            MaxId = maxId,
                            MaxValore = maxDeg,
                            Length = double.IsNaN(length) ? "NA" : (length * nDeg).ToString("F4", CultureInfo.InvariantCulture),
                            LinearDistance = double.IsNaN(distanza) ? "NA" : distanza.ToString("F4", CultureInfo.InvariantCulture)
                        };
                        anomalies.Add(anomalia);
                        firstId = 0;
                        startLat = null;
                        startLon = null;
                        maxId = 0;
                        maxDeg = 0;
                        length = 0;
                        nDeg = 0;
                        listDif.Clear();

                    }
                }
            }

            if (firstId != 0)
            {
                var fine = measurements[^1];
                var distanza = GeoService.DistanceCalculation(GeoService.DMSToDecimal(startLat), GeoService.DMSToDecimal(startLon), GeoService.DMSToDecimal(fine.Latitude), GeoService.DMSToDecimal(fine.Longitude));
                var anomalia = new Anomaly
                {
                    FirstId = firstId,
                    EndId = fine.Id,
                    FirstLat = startLat,
                    FirstLon = startLon,
                    EndLat = fine.Latitude,
                    EndLon = fine.Longitude,
                    MaxId = maxId,
                    MaxValore = maxDeg,
                    Length = double.IsNaN(length) ? "NA" : (length * nDeg).ToString("F4", CultureInfo.InvariantCulture),
                    LinearDistance = double.IsNaN(distanza) ? "NA" : distanza.ToString("F4", CultureInfo.InvariantCulture)
                };
                anomalies.Add(anomalia);
            }
            return anomalies;

        }
        public static List<Anomaly> MergeAnomaly(List<MonitoredLocation> measurements,double threshold, int nMeasurements) 
        {
            int firstId = 0, maxId = 0;
            double maxDeg = 0, length = 0;
            int nDeg = 0;
            int nNormalMeasurements = 0;    
            string startLat = null, startLon = null;
            List<Coordinate> listDif = new List<Coordinate>();
            var anomalies = new List<Anomaly>();

            for (int i = 0; i < measurements.Count; i++)
            {
                if (Math.Abs(measurements[i].Degradation) > threshold)
                {
                    nNormalMeasurements = 0;
                    if (firstId == 0)
                    {
                        firstId = measurements[i].Id;
                        startLat = measurements[i].Latitude;
                        startLon = measurements[i].Longitude;
                        maxId = measurements[i].Id;
                        maxDeg = measurements[i].Degradation;

                    }
                    if (maxDeg < measurements[i].Degradation)
                    {
                        maxDeg = measurements[i].Degradation;
                        maxId = measurements[i].Id;
                    }

                    nDeg++;
                    listDif.Add(new Coordinate { Latitude = measurements[i].Latitude, Longitude = measurements[i].Longitude });

                    if (nDeg >= 2)
                    {
                        if (listDif[^2].Latitude.Contains("NA") || listDif[^1].Latitude.Contains("NA"))
                            length += 0;
                        else
                            length += GeoService.DistanceCalculation(GeoService.DMSToDecimal(listDif[^2].Latitude), GeoService.DMSToDecimal(listDif[^2].Longitude), GeoService.DMSToDecimal(listDif[^1].Latitude), GeoService.DMSToDecimal(listDif[^1].Longitude));

                    }
                }
                else
                {
                    
                    
                    if (firstId != 0)
                    {
                        nNormalMeasurements++;
                        if (nNormalMeasurements > nMeasurements)
                        {
                            var lastRilevation = measurements[i - nNormalMeasurements];
                            var distanza = GeoService.DistanceCalculation(GeoService.DMSToDecimal(startLat), GeoService.DMSToDecimal(startLon), GeoService.DMSToDecimal(lastRilevation.Latitude), GeoService.DMSToDecimal(lastRilevation.Longitude));
                            var anomalia = new Anomaly()
                            {
                                FirstId = firstId,
                                FirstLat = startLat,
                                FirstLon = startLon,
                                EndId = lastRilevation.Id,
                                EndLat = lastRilevation.Latitude,
                                EndLon = lastRilevation.Longitude,
                                MaxId = maxId,
                                MaxValore = maxDeg,
                                Length = double.IsNaN(length) ? "NA" : (length * nDeg).ToString("F4", CultureInfo.InvariantCulture),
                                LinearDistance = double.IsNaN(distanza) ? "NA" : distanza.ToString("F4", CultureInfo.InvariantCulture)
                            };
                            anomalies.Add(anomalia);
                            firstId = 0;
                            startLat = null;
                            startLon = null;
                            maxId = 0;
                            maxDeg = 0;
                            length = 0;
                            nDeg = 0;
                            listDif.Clear();
                            nNormalMeasurements = 0;
                        }

                    }
                }
            }

            if (firstId != 0)
            {
                var fine = measurements[^1];
                var distanza = GeoService.DistanceCalculation(GeoService.DMSToDecimal(startLat), GeoService.DMSToDecimal(startLon), GeoService.DMSToDecimal(fine.Latitude), GeoService.DMSToDecimal(fine.Longitude));
                var anomalia = new Anomaly
                {
                    FirstId = firstId,
                    EndId = fine.Id,
                    FirstLat = startLat,
                    FirstLon = startLon,
                    EndLat = fine.Latitude,
                    EndLon = fine.Longitude,
                    MaxId = maxId,
                    MaxValore = maxDeg,
                    Length = double.IsNaN(length) ? "NA" : (length * nDeg).ToString("F4", CultureInfo.InvariantCulture),
                    LinearDistance = double.IsNaN(distanza) ? "NA" : distanza.ToString("F4", CultureInfo.InvariantCulture)
                };
                anomalies.Add(anomalia);
            }
            return anomalies;
        }

        public static List<Anomaly> MergeAnomaliesWithNASupport(List<MonitoredLocation> measurements, double threshold, int nMeasurements) 
        {
            List<int> validId = measurements.Where(x => !x.Latitude.Contains("NA")).Select(x=> x.Id).ToList();
            int firstId = 0, maxId = 0;
            int lastValidId = 0;
            double maxDeg = 0, length = 0;
            int nDeg = 0;
            int nNormalMeasurements = 0;
            string startLat = null, startLon = null;
            List<Coordinate> listDif = new List<Coordinate>();
            var anomalies = new List<Anomaly>();

            for (int i = 0; i < measurements.Count; i++)
            {
                if(!measurements[i].Latitude.Contains("NA"))
                    lastValidId = i;
                
                if (Math.Abs(measurements[i].Degradation) > threshold)
                {
                    nNormalMeasurements = 0;
                    if (firstId == 0)
                    {
                        firstId = measurements[i].Id;
                        if (measurements[i].Latitude.Contains("NA"))
                        {
                            startLat = measurements[lastValidId].Latitude;
                            startLon = measurements[lastValidId].Longitude;
                        }
                        else
                        {
                            startLat = measurements[i].Latitude;
                            startLon = measurements[i].Longitude;
                        }
                        maxId = measurements[i].Id;
                        maxDeg = measurements[i].Degradation;


                    }
                    if (maxDeg < measurements[i].Degradation)
                    {
                        maxDeg = measurements[i].Degradation;
                        maxId = measurements[i].Id;
                    }

                    nDeg++;
                    listDif.Add(new Coordinate { Latitude = measurements[i].Latitude, Longitude = measurements[i].Longitude });

                    if (nDeg >= 2)
                    {
                        if (listDif[^2].Latitude.Contains("NA") || listDif[^1].Latitude.Contains("NA"))
                            length += 0;
                        else
                            length += GeoService.DistanceCalculation(GeoService.DMSToDecimal(listDif[^2].Latitude), GeoService.DMSToDecimal(listDif[^2].Longitude), GeoService.DMSToDecimal(listDif[^1].Latitude), GeoService.DMSToDecimal(listDif[^1].Longitude));

                    }
                }
                else
                {


                    if (firstId != 0)
                    {
                        nNormalMeasurements++;
                        
                        if (nNormalMeasurements > nMeasurements)
                        {
                            var lastRilevation = measurements[i - nNormalMeasurements];
                            string latitude = lastRilevation.Latitude;
                            string longitude = lastRilevation.Longitude;

                            if (lastRilevation.Latitude.Contains("NA"))
                            {
                                int index = validId.BinarySearch(lastRilevation.Id);
                                if (index < 0)
                                    index = ~index; // posizione dove inserirlo

                                int nextIndex = (index < validId.Count) ? index : validId.Count - 1;
                                int prevIndex = (index > 0) ? index - 1 : 0;
                                var next = validId[nextIndex] -1;
                                var previous = validId[prevIndex] -1;
                                var nextDistance = GeoService.DistanceCalculation(GeoService.DMSToDecimal(lastRilevation.Latitude), GeoService.DMSToDecimal(lastRilevation.Longitude), GeoService.DMSToDecimal(measurements[next].Latitude), GeoService.DMSToDecimal(measurements[next].Longitude));
                                var previousDistance = GeoService.DistanceCalculation(GeoService.DMSToDecimal(lastRilevation.Latitude), GeoService.DMSToDecimal(lastRilevation.Longitude), GeoService.DMSToDecimal(measurements[previous].Latitude), GeoService.DMSToDecimal(measurements[previous].Longitude));
                                if (previousDistance == nextDistance)
                                {
                                     latitude = measurements[next].Latitude;
                                     longitude = measurements[next].Longitude;
                                }
                                else
                                {

                                    latitude = measurements[previous].Latitude;
                                    longitude = measurements[previous].Longitude;
                                }
                            }
                            var distanza = GeoService.DistanceCalculation(GeoService.DMSToDecimal(startLat), GeoService.DMSToDecimal(startLon), GeoService.DMSToDecimal(latitude), GeoService.DMSToDecimal(longitude));
                            var anomalia = new Anomaly()
                            {
                                FirstId = firstId,
                                FirstLat = startLat,
                                FirstLon = startLon,
                                EndId = lastRilevation.Id,
                                EndLat = latitude,
                                EndLon = longitude,
                                MaxId = maxId,
                                MaxValore = maxDeg,
                                Length = double.IsNaN(length) ? "NA" : (length * nDeg).ToString("F4", CultureInfo.InvariantCulture),
                                LinearDistance = double.IsNaN(distanza) ? "NA" : distanza.ToString("F4", CultureInfo.InvariantCulture)
                            };
                            anomalies.Add(anomalia);
                            firstId = 0;
                            startLat = null;
                            startLon = null;
                            maxId = 0;
                            maxDeg = 0;
                            length = 0;
                            nDeg = 0;
                            listDif.Clear();
                            nNormalMeasurements = 0;
                        }

                    }
                }
            }

            if (firstId != 0)
            {
                
                var fine = measurements[^1];
                string latitude = fine.Latitude;
                string longitude = fine.Longitude;
                if (fine.Latitude.Contains("NA"))
                {
                    int index = validId.BinarySearch(fine.Id);
                    if (index < 0)
                        index = ~index; // posizione dove inserirlo

                    int nextIndex = (index < validId.Count) ? index : validId.Count - 1;
                    int prevIndex = (index > 0) ? index - 1 : 0;
                    var next = validId[nextIndex] - 1;
                    var previous = validId[prevIndex] - 1;
                    var nextDistance = GeoService.DistanceCalculation(GeoService.DMSToDecimal(fine.Latitude), GeoService.DMSToDecimal(fine.Longitude), GeoService.DMSToDecimal(measurements[next].Latitude), GeoService.DMSToDecimal(measurements[next].Longitude));
                    var previousDistance = GeoService.DistanceCalculation(GeoService.DMSToDecimal(fine.Latitude), GeoService.DMSToDecimal(fine.Longitude), GeoService.DMSToDecimal(measurements[previous].Latitude), GeoService.DMSToDecimal(measurements[previous].Longitude));
                    if (previousDistance == nextDistance)
                    {
                        latitude = measurements[next].Latitude;
                        longitude = measurements[next].Longitude;
                    }
                    else
                    {

                        latitude = measurements[previous].Latitude;
                        longitude = measurements[previous].Longitude;
                    }
                }
                var distanza = GeoService.DistanceCalculation(GeoService.DMSToDecimal(startLat), GeoService.DMSToDecimal(startLon), GeoService.DMSToDecimal(fine.Latitude), GeoService.DMSToDecimal(fine.Longitude));
                var anomalia = new Anomaly
                {
                    FirstId = firstId,
                    EndId = fine.Id,
                    FirstLat = startLat,
                    FirstLon = startLon,
                    EndLat = latitude,
                    EndLon = longitude,
                    MaxId = maxId,
                    MaxValore = maxDeg,
                    Length = double.IsNaN(length) ? "NA" : (length * nDeg).ToString("F4", CultureInfo.InvariantCulture),
                    LinearDistance = double.IsNaN(distanza) ? "NA" : distanza.ToString("F4", CultureInfo.InvariantCulture)
                };
                anomalies.Add(anomalia);
            }
            return anomalies;
        }
    }
}
