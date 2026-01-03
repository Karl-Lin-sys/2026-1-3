using Microsoft.AspNetCore.Mvc;
using SmartBuildingIoT.Data;
using SmartBuildingIoT.Models;
using SmartBuildingIoT.Services;
using Microsoft.EntityFrameworkCore;

namespace SmartBuildingIoT.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly AnomalyDetector _anomalyDetector;

    public AnalyticsController(ApplicationDbContext db, AnomalyDetector anomalyDetector)
    {
        _db = db;
        _anomalyDetector = anomalyDetector;
    }

    /// <summary>
    /// Get AI-powered analytics for a sensor
    /// </summary>
    [HttpGet("{sensorId}")]
    public async Task<IActionResult> GetAnalytics(string sensorId)
    {
        var stats = _anomalyDetector.GetStats(sensorId);
        var trend = _anomalyDetector.GetTrend(sensorId);
        var predicted = _anomalyDetector.PredictNext(sensorId);

        var anomalyCount = await _db.SensorReadings
            .Where(s => s.SensorId == sensorId && s.IsAnomaly)
            .CountAsync();

        return Ok(new AnalyticsResult
        {
            SensorId = sensorId,
            Mean = Math.Round(stats.mean, 2),
            StdDev = Math.Round(stats.std, 2),
            Min = Math.Round(stats.min, 2),
            Max = Math.Round(stats.max, 2),
            AnomalyCount = anomalyCount,
            Trend = trend,
            PredictedNext = Math.Round(predicted, 2)
        });
    }

    /// <summary>
    /// Get recent alerts (anomalies)
    /// </summary>
    [HttpGet("alerts")]
    public async Task<IActionResult> GetAlerts(int limit = 20)
    {
        var alerts = await _db.SensorReadings
            .Where(s => s.IsAnomaly)
            .OrderByDescending(s => s.Timestamp)
            .Take(limit)
            .Select(s => new Alert
            {
                SensorId = s.SensorId,
                AlertType = s.SensorType,
                Message = $"Anomaly detected: {s.Value} {s.Unit} at {s.Location}",
                Severity = Math.Abs(s.Value) > 50 ? "Critical" : "Warning",
                Timestamp = s.Timestamp
            })
            .ToListAsync();

        return Ok(alerts);
    }

    /// <summary>
    /// Get energy consumption forecast
    /// </summary>
    [HttpGet("forecast")]
    public IActionResult GetForecast()
    {
        var powerSensors = new[] { "PWR-01", "PWR-02" };
        var forecasts = powerSensors.Select(id => new
        {
            sensorId = id,
            current = Math.Round(_anomalyDetector.GetStats(id).mean, 2),
            predicted = Math.Round(_anomalyDetector.PredictNext(id), 2),
            trend = _anomalyDetector.GetTrend(id)
        });

        return Ok(forecasts);
    }
}
