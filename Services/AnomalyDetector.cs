namespace SmartBuildingIoT.Services;

/// <summary>
/// AI-powered anomaly detection service using statistical methods
/// </summary>
public class AnomalyDetector
{
    private readonly Dictionary<string, List<double>> _sensorHistory = new();
    private readonly Dictionary<string, (double mean, double std)> _baselines = new();
    private const int HistorySize = 100;
    private const double AnomalyThreshold = 2.5;  // Z-score threshold

    /// <summary>
    /// Check if a reading is anomalous using Z-score method
    /// </summary>
    public bool IsAnomaly(string sensorId, double value)
    {
        if (!_sensorHistory.ContainsKey(sensorId))
        {
            _sensorHistory[sensorId] = new List<double>();
        }

        var history = _sensorHistory[sensorId];
        
        // Need enough history
        if (history.Count < 10)
        {
            history.Add(value);
            return false;
        }

        // Calculate statistics
        var mean = history.Average();
        var variance = history.Sum(x => Math.Pow(x - mean, 2)) / history.Count;
        var std = Math.Sqrt(variance);

        // Z-score
        var zScore = std > 0 ? Math.Abs(value - mean) / std : 0;
        var isAnomaly = zScore > AnomalyThreshold;

        // Update history
        history.Add(value);
        if (history.Count > HistorySize)
            history.RemoveAt(0);

        // Update baselines
        _baselines[sensorId] = (mean, std);

        return isAnomaly;
    }

    /// <summary>
    /// Predict next value using simple moving average
    /// </summary>
    public double PredictNext(string sensorId, int windowSize = 5)
    {
        if (!_sensorHistory.TryGetValue(sensorId, out var history) || history.Count < windowSize)
            return 0;

        return history.TakeLast(windowSize).Average();
    }

    /// <summary>
    /// Get trend direction
    /// </summary>
    public string GetTrend(string sensorId)
    {
        if (!_sensorHistory.TryGetValue(sensorId, out var history) || history.Count < 5)
            return "Stable";

        var recent = history.TakeLast(5).ToList();
        var slope = (recent.Last() - recent.First()) / recent.Count;

        return slope switch
        {
            > 0.5 => "Rising",
            < -0.5 => "Falling",
            _ => "Stable"
        };
    }

    /// <summary>
    /// Get analytics for a sensor
    /// </summary>
    public (double mean, double std, double min, double max) GetStats(string sensorId)
    {
        if (!_sensorHistory.TryGetValue(sensorId, out var history) || history.Count == 0)
            return (0, 0, 0, 0);

        var mean = history.Average();
        var variance = history.Sum(x => Math.Pow(x - mean, 2)) / history.Count;
        return (mean, Math.Sqrt(variance), history.Min(), history.Max());
    }
}
