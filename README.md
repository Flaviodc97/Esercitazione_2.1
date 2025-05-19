# Esercitazione_2.1
# Anomaly Detection on Measurements Degradation Data

This application detects anomalies in a set of GPS-based degradation measurements, allowing different strategies.

## Project Structure

- `Program.cs`: Main entry point. Manages user interaction via console and calls the appropriate detection method.
- `Models/MonitoredLocation.cs`: Data model for individual GPS measurements.
- `Models/Anomaly.cs`: Model representing a detected anomaly.
- `Models/Coordinate.cs`: Model representing simple Coordinate.
- `Services/AnomalyDetectionService.cs`: Contains the core logic for anomaly detection:
		- `FindAnomalies`: Detects anomalies where degradation exceeds a given threshold.
		- `MergeAnomalies`: Detects and merges consecutive anomalies, allowing interruptions with normal values (under a configurable tolerance).
		- `MergeAnomaliesWithNASupport`: Similar to MergeAnomaly, but includes logic to handle missing ("NA") coordinates.
- `Services/CsvReaderService.cs`: Loads mesurements data from a CSV file.
- `Services/CSVWriterService.cs`: Writes detected anomalies to a new CSV file.
- `Services/GeoService`: Contains Geo Logic for Geographycal calculation.
	- `DMSToDecimal`: Converts DMS-format coordinates to decimal degrees.
	- `DistanceCalculation`:  Computes linear distance between two decimal coordinates.

---

## How to Run

1. **Place input file**:
   - Make sure the file `example.csv` is located in the `src` directory.

2. **Run the program**:
   - Execute via Visual Studio, Rider, or command line:
     ```bash
     dotnet run
     ```

3. **Choose an operation**:
   - The console will prompt you to choose:
     - `1` – Basic anomaly detection (`FindAnomalies`)
     - `2` – Merging anomalies with tolerance (`MergeAnomalies`)
     - `3` – Merging with NA handling (`MergeAnomaliesWithNASupport`)
   - You’ll also be asked for a threshold (and a tolerance value if needed).

4. **Output**:
   - Results will be saved in the `src` directory under:
     - `anomalie_Es1.csv`
     - `anomalie_Es2.csv`
     - `anomalie_Es3.csv`
	 
	 
## How to Run Tests and CodeCoverage

1. **Run All the test**:
   - On Visual Studio -> Test -> Run All the Tests.

2. **Run Commands on Terminal**:
	 - Execute via Visual Studio,or command line:
     ```bash
     dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
     ```
	 
	 ```bash
     reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coveragereport
     ```
3. **Open Esercitazione_2.1/Esercitazione_2.1.Tests/coveragereport/Esercitazione_2.1_AnomalyDetectionService.html**:
	- To Observe the Results open this file on a Browser.