using Esercitazione_2._1.Models;
using Esercitazione_2._1.Services;

namespace Esercitazione_2._1.Tests
{

    public class AnomalyDetectionTests
    {

        private static MonitoredLocation Loc(int id, string lat, string lon, double degr) =>
            new MonitoredLocation { Id = id, Latitude = lat, Longitude = lon, Degradation = degr };
        
        [Fact]
        public void FindAnomalies_NoValuesAboveThreshold_ReturnsEmptyList()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 1.5),
                Loc(2, "N 0°0'0\"", "E 0°0'1\"", 2.0),
                Loc(3, "N 0°0'0\"", "E 0°0'2\"", -2.9),

            };
            var result = AnomalyDetectionService.FindAnomalies(locations, 3.0);
            Assert.Empty(result);

        }

        [Fact]
        public void FindAnomalies_SingleValueAboveThreshold_ReturnsSingleAnomaly()
        {
            var location = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 4.5)
            };

            var result = AnomalyDetectionService.FindAnomalies(location, 3);
            Assert.Single(result);
            Assert.Equal(1, result[0].FirstId);
            Assert.Equal(1, result[0].EndId);
        }

        [Fact]
        public void FindAnomalies_MultipleValuesAboveThreshold_ReturnsAnomaliesList()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 1.5),
                Loc(2, "N 0°0'0\"", "E 0°0'1\"", 2.0),
                Loc(3, "N 0°0'0\"", "E 0°0'2\"", -2.9)
            };
            var result = AnomalyDetectionService.FindAnomalies(locations, 1.5);
            Assert.Equal(1, result.Count);
        }

        [Fact]
        public void FindAnomalies_MissingCoordinates_SetsLengthZeroAndLinearDistanceNA()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "NA", "NA", 3.0),
                Loc(2, "NA", "NA", 5.0),
            };
            var result = AnomalyDetectionService.FindAnomalies(locations, 2);
            Assert.Equal("0.0000", result[0].Length);
            Assert.Equal("NA", result[0].LinearDistance);

        }

        [Fact]
        public void FindAnomalies_MixedAnomaliesWithNormalValues_ReturnsCorrectAnomalyCount()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 8.0),
                Loc(2, "N 0°0'1\"", "E 0°0'1\"", 0.5),
                Loc(3, "N 0°0'2\"", "E 0°0'2\"", 6.0),
                Loc(4, "N 0°0'3\"", "E 0°0'3\"", 2.0),
                Loc(5, "N 0°0'4\"", "E 0°0'4\"", 7.0)
            };

            var result = AnomalyDetectionService.FindAnomalies(locations, 3.0);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void FindAnomalies_MultipleAnomalies_SetsCorrectMaxIdAndMaxValue()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 8.0),
                Loc(2, "N 0°0'2\"", "E 0°0'2\"", 6.0),
                Loc(3, "N 0°0'4\"", "E 0°0'4\"", 7.0)
            };
            var result = AnomalyDetectionService.FindAnomalies(locations, 3);
            Assert.Equal(1, result[0].MaxId);
            Assert.Equal(8.0, result[0].MaxValore);
                
        }

        [Fact]
        public void FindAnomalies_ContiguousAnomalies_SetsCorrectLength()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(13,"N 45°29'12.0859\"","E 9°12'19.0606\"",-4.9338),
                Loc(14, "N 45°29'12.0996\"", "E 9°12'19.0675\"", 6.1497)
            };
            var result = AnomalyDetectionService.FindAnomalies(locations, 3);
            Assert.Equal("0.8975", result[0].Length);
            
        }


        [Fact]
        public void FindAnomalies_ValidLocations_SetsCorrectLinearDistance()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(13, "N 45°29'12.0859\"", "E 9°12'19.0606\"", -4.9338),
                Loc(14, "N 45°29'12.0996\"", "E 9°12'19.0675\"", 6.1497),
                Loc(16, "N 45°29'12.1133\"", "E 9°12'19.0778\"", 5.2728),
                Loc(17, "N 45°29'12.1133\"", "E 9°12'19.0846\"", 3.8218),
                Loc(18, "N 45°29'12.1271\"", "E 9°12'19.0915\"", 9.7715),
                Loc(19, "N 45°29'12.1271\"", "E 9°12'19.0984\"", 4.0511)
            };
            var result = AnomalyDetectionService.FindAnomalies(locations, 3);
            Assert.Equal("1.5131", result[0].LinearDistance);
        }

        [Fact]
        public void MergeAnomalies_NoValuesAboveThreshold_ReturnsEmptyList()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 1.0),
                Loc(2, "N 0°0'0\"", "E 0°0'1\"", 2.0)
            };

            var result = AnomalyDetectionService.MergeAnomalies(locations, 3.0, 1);
            Assert.Empty(result);
        }

        [Fact]
        public void MergeAnomalies_SingleAnomaly_ReturnsCorrectAnomaly()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 4.5)
            };

            var result = AnomalyDetectionService.MergeAnomalies(locations, 3.0, 1);
            Assert.Single(result);
            Assert.Equal(1, result[0].FirstId);
            Assert.Equal(1, result[0].EndId);
        }

        [Fact]
        public void MergeAnomalies_InterruptionBelowThresholdWithinTolerance_ReturnsSingleMergedAnomaly()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 4.0),
                Loc(2, "N 0°0'1\"", "E 0°0'1\"", 1.0),  
                Loc(3, "N 0°0'2\"", "E 0°0'2\"", 5.0)
            };

            var result = AnomalyDetectionService.MergeAnomalies(locations, 3.0, 2);
            Assert.Single(result);
            Assert.Equal(1, result[0].FirstId);
            Assert.Equal(3, result[0].EndId);
        }

        [Fact]
        public void MergeAnomalies_TooManyNormalValues_SplitsIntoTwoAnomalies()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 5.0),
                Loc(2, "N 0°0'1\"", "E 0°0'1\"", 0.5),
                Loc(3, "N 0°0'2\"", "E 0°0'2\"", 0.4),
                Loc(4, "N 0°0'3\"", "E 0°0'3\"", 6.0)
            };

            var result = AnomalyDetectionService.MergeAnomalies(locations, 3.0, 1);
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].FirstId);
            Assert.Equal(1, result[0].EndId);
            Assert.Equal(4, result[1].FirstId);
            Assert.Equal(4, result[1].EndId);
        }

        [Fact]
        public void MergeAnomalies_MissingCoordinates_SetsLengthZeroAndLinearDistanceNA()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "NA", "NA", 4.0),
                Loc(2, "NA", "NA", 5.0)
            };

            var result = AnomalyDetectionService.MergeAnomalies(locations, 2.0, 1);
            Assert.Single(result);
            Assert.Equal("0.0000", result[0].Length);
            Assert.Equal("NA", result[0].LinearDistance);
        }

        [Fact]
        public void MergeAnomalies_MaxValuesAreCorrectlyTracked()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 4.0),
                Loc(2, "N 0°0'1\"", "E 0°0'1\"", 7.0),
                Loc(3, "N 0°0'2\"", "E 0°0'2\"", 6.0)
            };

            var result = AnomalyDetectionService.MergeAnomalies(locations, 3.0, 1);
            Assert.Single(result);
            Assert.Equal(2, result[0].MaxId);
            Assert.Equal(7.0, result[0].MaxValore);
        }

        [Fact]
        public void MergeAnomalies_ContiguousAnomalies_ComputesCorrectLength()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(13,"N 45°29'12.0859\"","E 9°12'19.0606\"",4.9),
                Loc(14, "N 45°29'12.0996\"", "E 9°12'19.0675\"", 6.1)
            };

            var result = AnomalyDetectionService.MergeAnomalies(locations, 3.0, 1);
            Assert.Equal("0.8975", result[0].Length);
        }

        [Fact]
        public void MergeAnomalies_ValidLocations_SetsCorrectLinearDistance()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(13, "N 45°29'12.0859\"", "E 9°12'19.0606\"", 4.9),
                Loc(14, "N 45°29'12.0996\"", "E 9°12'19.0675\"", 6.1),
                Loc(16, "N 45°29'12.1133\"", "E 9°12'19.0778\"", 5.2),
                Loc(17, "N 45°29'12.1133\"", "E 9°12'19.0846\"", 3.8),
                Loc(18, "N 45°29'12.1271\"", "E 9°12'19.0915\"", 9.7),
                Loc(19, "N 45°29'12.1271\"", "E 9°12'19.0984\"", 4.0)
            };

            var result = AnomalyDetectionService.MergeAnomalies(locations, 3.0, 1);
            Assert.Equal("1.5131", result[0].LinearDistance);
        }

        [Fact]
        public void MergeAnomaliesWithNASupport_NoValuesAboveThreshold_ReturnsEmptyList()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 1.0),
                Loc(2, "N 0°0'1\"", "E 0°0'1\"", 2.0)
            };

            var result = AnomalyDetectionService.MergeAnomaliesWithNASupport(locations, 3.0, 1);
            Assert.Empty(result);
        }

        [Fact]
        public void MergeAnomaliesWithNASupport_SingleAnomalyWithValidCoordinates_ReturnsCorrectAnomalyWithZeroDistances()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 4.5)
            };

            var result = AnomalyDetectionService.MergeAnomaliesWithNASupport(locations, 3.0, 1);
            Assert.Single(result);
            Assert.Equal(1, result[0].FirstId);
            Assert.Equal(1, result[0].EndId);
            Assert.Equal("0.0000", result[0].Length);
            Assert.Equal("0.0000", result[0].LinearDistance);
        }

        [Fact]
        public void MergeAnomaliesWithNASupport_AllCoordinatesNA_ReturnsNaDistance()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "NA", "NA", 5.0),
                Loc(2, "NA", "NA", 4.0),
                Loc(3, "NA", "NA", 5.0),

            };

            var result = AnomalyDetectionService.MergeAnomaliesWithNASupport(locations, 3.0, 1);
            Assert.Single(result);
            Assert.Equal("NA", result[0].LinearDistance);
            Assert.Equal("0.0000", result[0].Length);
        }

        [Fact]
        public void MergeAnomaliesWithNASupport_InterruptionWithinTolerance_MergesAnomalies()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 4.0),
                Loc(2, "N 0°0'1\"", "E 0°0'1\"", 1.0),
                Loc(3, "N 0°0'2\"", "E 0°0'2\"", 5.0)
            };

            var result = AnomalyDetectionService.MergeAnomaliesWithNASupport(locations, 3.0, 2);
            Assert.Single(result);
            Assert.Equal(1, result[0].FirstId);
            Assert.Equal(3, result[0].EndId);
        }

        [Fact]
        public void MergeAnomaliesWithNASupport_TooManyNormals_SplitsAnomalies()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 5.0),
                Loc(2, "N 0°0'1\"", "E 0°0'1\"", 0.5),
                Loc(3, "N 0°0'2\"", "E 0°0'2\"", 0.4),
                Loc(4, "N 0°0'3\"", "E 0°0'3\"", 6.0)
            };

            var result = AnomalyDetectionService.MergeAnomaliesWithNASupport(locations, 3.0, 1);
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].FirstId);
            Assert.Equal(4, result[1].FirstId);
        }

        [Fact]
        public void MergeAnomaliesWithNASupport_NAEndCoordinates_InterpolatedFromNearestValid()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 5.0),
                Loc(2, "NA", "NA", 0.5),
                Loc(3, "N 0°0'2\"", "E 0°0'2\"", 0.0),
                Loc(4, "NA", "NA", 6.0)
            };

            var result = AnomalyDetectionService.MergeAnomaliesWithNASupport(locations, threshold: 3.0, nMeasurements: 1);

            Assert.Equal(2, result.Count);

            var first = result[0];
            Assert.Equal(1, first.FirstId);
            Assert.Equal(1, first.EndId);
            Assert.NotEqual("NA", first.EndLat);
            Assert.NotEqual("NA", first.EndLon);

            var second = result[1];
            Assert.Equal(4, second.FirstId);
            Assert.Equal(4, second.EndId);
            Assert.NotEqual("NA", second.EndLat);
            Assert.NotEqual("NA", second.EndLon);
        }

        [Fact]
        public void MergeAnomaliesWithNASupport_UnclosedFinalAnomaly_IsIncluded()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 5.0),
                Loc(2, "N 0°0'1\"", "E 0°0'1\"", 6.0)
            };

            var result = AnomalyDetectionService.MergeAnomaliesWithNASupport(locations, 3.0, 1);
            Assert.Single(result);
            Assert.Equal(1, result[0].FirstId);
            Assert.Equal(2, result[0].EndId);
        }

        [Fact]
        public void MergeAnomaliesWithNASupport_EndCoordinatesAreInterpolatedCorrectly_WhenNA()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 5.0),
                Loc(2, "N 0°0'1\"", "E 0°0'1\"", 4.0),
                Loc(3, "NA", "NA", 5.5),
            };

            var result = AnomalyDetectionService.MergeAnomaliesWithNASupport(locations, threshold: 3.0, nMeasurements: 0);

            Assert.Single(result);
            Assert.Equal(1, result[0].FirstId);
            Assert.Equal(3, result[0].EndId);
            Assert.NotEqual("NA", result[0].EndLat); 
            Assert.NotEqual("NA", result[0].EndLon);
        }


        [Fact]
        public void MergeAnomaliesWithNASupport_LastRilevationWithEqualsDistance_SplitsAnomalies()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "NA", "NA", 5.0),
                Loc(2, "N 0°0'1\"", "E 0°0'1\"", 0.5),
                Loc(3, "N 0°0'1\"", "E 0°0'1\"", 0.4),
                Loc(4, "N 0°0'1\"", "E 0°0'1\"", 6.0)
            };

            var result = AnomalyDetectionService.MergeAnomaliesWithNASupport(locations, 3.0, 1);
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].FirstId);
            Assert.Equal(4, result[1].FirstId);
        }
        [Fact]
        public void MergeAnomaliesParallel_NoValuesAboveThreshold_ReturnsEmptyList()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 1.0),
                Loc(2, "N 0°0'0\"", "E 0°0'1\"", 2.0)
            };

            var result = AnomalyDetectionService.MergeAnomaliesParallel(locations, 3.0, 1);
            Assert.Empty(result);
        }

        [Fact]
        public void MergeAnomaliesParallel_SingleAnomaly_ReturnsCorrectAnomaly()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 4.5)
            };

            var result = AnomalyDetectionService.MergeAnomaliesParallel(locations, 3.0, 1);
            Assert.Single(result);
            Assert.Equal(1, result[0].FirstId);
            Assert.Equal(1, result[0].EndId);
        }

        [Fact]
        public void MergeAnomaliesParallel_InterruptionBelowThresholdWithinTolerance_ReturnsSingleMergedAnomaly()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 4.0),
                Loc(2, "N 0°0'1\"", "E 0°0'1\"", 1.0),
                Loc(3, "N 0°0'2\"", "E 0°0'2\"", 5.0)
            };

            var result = AnomalyDetectionService.MergeAnomaliesParallel(locations, 3.0, 2);
            Assert.Single(result);
            Assert.Equal(1, result[0].FirstId);
            Assert.Equal(3, result[0].EndId);
        }

        [Fact]
        public void MergeAnomaliesParallel_TooManyNormalValues_SplitsIntoTwoAnomalies()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 5.0),
                Loc(2, "N 0°0'1\"", "E 0°0'1\"", 0.5),
                Loc(3, "N 0°0'2\"", "E 0°0'2\"", 0.4),
                Loc(4, "N 0°0'3\"", "E 0°0'3\"", 6.0)
            };

            var result = AnomalyDetectionService.MergeAnomaliesParallel(locations, 3.0, 1);
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].FirstId);
            Assert.Equal(1, result[0].EndId);
            Assert.Equal(4, result[1].FirstId);
            Assert.Equal(4, result[1].EndId);
        }

        [Fact]
        public void MergeAnomaliesParallel_MissingCoordinates_SetsLengthZeroAndLinearDistanceNA()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "NA", "NA", 4.0),
                Loc(2, "NA", "NA", 5.0)
            };

            var result = AnomalyDetectionService.MergeAnomaliesParallel(locations, 2.0, 1);
            Assert.Single(result);
            Assert.Equal("0.0000", result[0].Length);
            Assert.Equal("NA", result[0].LinearDistance);
        }

        [Fact]
        public void MergeAnomaliesParallel_MaxValuesAreCorrectlyTracked()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(1, "N 0°0'0\"", "E 0°0'0\"", 4.0),
                Loc(2, "N 0°0'1\"", "E 0°0'1\"", 7.0),
                Loc(3, "N 0°0'2\"", "E 0°0'2\"", 6.0)
            };

            var result = AnomalyDetectionService.MergeAnomaliesParallel(locations, 3.0, 1);
            Assert.Single(result);
            Assert.Equal(2, result[0].MaxId);
            Assert.Equal(7.0, result[0].MaxValore);
        }

        [Fact]
        public void MergeAnomaliesParallel_ContiguousAnomalies_ComputesCorrectLength()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(13,"N 45°29'12.0859\"","E 9°12'19.0606\"",4.9),
                Loc(14, "N 45°29'12.0996\"", "E 9°12'19.0675\"", 6.1)
            };

            var result = AnomalyDetectionService.MergeAnomaliesParallel(locations, 3.0, 1);
            Assert.Equal("0.8975", result[0].Length);
        }

        [Fact]
        public void MergeAnomaliesParallelParallel_ValidLocations_SetsCorrectLinearDistance()
        {
            var locations = new List<MonitoredLocation>
            {
                Loc(13, "N 45°29'12.0859\"", "E 9°12'19.0606\"", 4.9),
                Loc(14, "N 45°29'12.0996\"", "E 9°12'19.0675\"", 6.1),
                Loc(16, "N 45°29'12.1133\"", "E 9°12'19.0778\"", 5.2),
                Loc(17, "N 45°29'12.1133\"", "E 9°12'19.0846\"", 3.8),
                Loc(18, "N 45°29'12.1271\"", "E 9°12'19.0915\"", 9.7),
                Loc(19, "N 45°29'12.1271\"", "E 9°12'19.0984\"", 4.0)
            };

            var result = AnomalyDetectionService.MergeAnomaliesParallel(locations, 3.0, 1);
            Assert.Equal("1.5131", result[0].LinearDistance);
        }


    }
}