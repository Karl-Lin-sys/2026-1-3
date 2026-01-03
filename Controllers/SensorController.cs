using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBuildingIoT.Data;
using SmartBuildingIoT.Models;
using SmartBuildingIoT.Services;

namespace SmartBuildingIoT.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly SensorSimulator _simulator;
    private readonly AnomalyDetector _anomalyDetector;

    public SensorController(
        ApplicationDbContext db,
        SensorSimulator simulator,
        AnomalyDetector anomalyDetector)
    {
        _db = db;
        _simulator = simulator;
        _anomalyDetector = anomalyDetector;
    }

    /// <summary>
    /// Get list of all sensors
    /// </summary>
    [HttpGet("list")]
    public IActionResult GetSensors()
    {
        return Ok(_simulator.GetSensorInfo());
    }

    /// <summary>
    /// Get latest readings from all sensors
    /// </summary>
    [HttpGet("readings")]
    public async Task<IActionResult> GetLatestReadings()
    {
        // Generate new readings
        var readings = _simulator.GenerateReadings();

        // Check for anomalies with AI
        foreach (var reading in readings)
        {
            reading.IsAnomaly = _anomalyDetector.IsAnomaly(reading.SensorId, reading.Value);
        }

        // Save to database
        _db.SensorReadings.AddRange(readings);
        await _db.SaveChangesAsync();

        return Ok(readings);
    }

    /// <summary>
    /// Get historical data for a sensor
    /// </summary>
    [HttpGet("history/{sensorId}")]
    public async Task<IActionResult> GetHistory(string sensorId, int limit = 50)
    {
        var history = await _db.SensorReadings
            .Where(s => s.SensorId == sensorId)
            .OrderByDescending(s => s.Timestamp)
            .Take(limit)
            .ToListAsync();

        return Ok(history);
    }

    /// <summary>
    /// Get building zones
    /// </summary>
    [HttpGet("zones")]
    public async Task<IActionResult> GetZones()
    {
        var zones = await _db.Zones.ToListAsync();
        return Ok(zones);
    }

    /// <summary>
    /// Get summary statistics
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var totalReadings = await _db.SensorReadings.CountAsync();
        var anomalies = await _db.SensorReadings.CountAsync(s => s.IsAnomaly);
        var sensors = _simulator.GetSensorInfo().Count();

        return Ok(new
        {
            totalReadings,
            anomalyCount = anomalies,
            sensorCount = sensors,
            anomalyRate = totalReadings > 0 ? (double)anomalies / totalReadings * 100 : 0
        });
    }
}
