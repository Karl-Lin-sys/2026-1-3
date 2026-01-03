namespace SmartBuildingIoT.Models;

/// <summary>
/// Sensor data model for IoT readings
/// </summary>
public class SensorData
{
    public int Id { get; set; }
    public string SensorId { get; set; } = string.Empty;
    public string SensorType { get; set; } = string.Empty;  // Temperature, Humidity, Power, CO2
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool IsAnomaly { get; set; } = false;
}

/// <summary>
/// Building zone configuration
/// </summary>
public class BuildingZone
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Floor { get; set; }
    public string Type { get; set; } = string.Empty;  // Office, Meeting, Lobby
}

/// <summary>
/// Alert model for anomaly notifications
/// </summary>
public class Alert
{
    public int Id { get; set; }
    public string SensorId { get; set; } = string.Empty;
    public string AlertType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = "Warning";  // Info, Warning, Critical
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool IsResolved { get; set; } = false;
}

/// <summary>
/// Analytics result DTO
/// </summary>
public class AnalyticsResult
{
    public string SensorId { get; set; } = string.Empty;
    public double Mean { get; set; }
    public double StdDev { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
    public int AnomalyCount { get; set; }
    public string Trend { get; set; } = "Stable";  // Rising, Falling, Stable
    public double PredictedNext { get; set; }
}
