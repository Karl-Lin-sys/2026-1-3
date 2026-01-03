using SmartBuildingIoT.Models;

namespace SmartBuildingIoT.Services;

/// <summary>
/// IoT sensor simulator for generating realistic building data
/// </summary>
public class SensorSimulator
{
    private readonly Random _random = new();
    private readonly Dictionary<string, double> _currentValues = new();

    private readonly (string id, string type, string location, double baseValue, double variance, string unit)[] _sensors = new[]
    {
        ("TEMP-01", "Temperature", "Lobby", 22.0, 2.0, "°C"),
        ("TEMP-02", "Temperature", "Office A", 23.0, 1.5, "°C"),
        ("TEMP-03", "Temperature", "Server Room", 18.0, 1.0, "°C"),
        ("HUM-01", "Humidity", "Lobby", 45.0, 5.0, "%"),
        ("HUM-02", "Humidity", "Office A", 50.0, 5.0, "%"),
        ("PWR-01", "Power", "Building", 150.0, 30.0, "kW"),
        ("PWR-02", "Power", "Server Room", 45.0, 10.0, "kW"),
        ("CO2-01", "CO2", "Office A", 600.0, 100.0, "ppm"),
        ("CO2-02", "CO2", "Meeting Room 1", 500.0, 150.0, "ppm"),
        ("LIGHT-01", "Light", "Lobby", 500.0, 50.0, "lux"),
    };

    public SensorSimulator()
    {
        // Initialize with base values
        foreach (var sensor in _sensors)
        {
            _currentValues[sensor.id] = sensor.baseValue;
        }
    }

    /// <summary>
    /// Generate sensor readings with realistic variations
    /// </summary>
    public List<SensorData> GenerateReadings(bool includeAnomalies = true)
    {
        var readings = new List<SensorData>();
        var anomalyChance = includeAnomalies ? 0.05 : 0;  // 5% anomaly rate

        foreach (var sensor in _sensors)
        {
            var isAnomaly = _random.NextDouble() < anomalyChance;
            double value;

            if (isAnomaly)
            {
                // Generate anomalous value (±3 sigma)
                var multiplier = _random.NextDouble() > 0.5 ? 3.0 : -3.0;
                value = sensor.baseValue + multiplier * sensor.variance;
            }
            else
            {
                // Normal random walk
                var delta = (_random.NextDouble() - 0.5) * sensor.variance * 0.5;
                var current = _currentValues[sensor.id];
                value = current + delta;

                // Mean reversion (pull towards base)
                value += (sensor.baseValue - value) * 0.1;
            }

            _currentValues[sensor.id] = value;

            readings.Add(new SensorData
            {
                SensorId = sensor.id,
                SensorType = sensor.type,
                Location = sensor.location,
                Value = Math.Round(value, 2),
                Unit = sensor.unit,
                Timestamp = DateTime.UtcNow,
                IsAnomaly = isAnomaly
            });
        }

        return readings;
    }

    /// <summary>
    /// Get list of available sensors
    /// </summary>
    public IEnumerable<object> GetSensorInfo()
    {
        return _sensors.Select(s => new
        {
            s.id,
            s.type,
            s.location,
            s.unit
        });
    }
}
