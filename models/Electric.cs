using System;
using System.Text.Json.Serialization;

namespace ProjectFIN.models;

public class ElectricCar : Vehicle
{
    public double BatteryLevel { get; init; } = 100.0;

    private ElectricCar() : base() { }

    [JsonConstructor]
    public ElectricCar(string vin, string brand, string model, GeoCoordinate currentLocation, double batteryLevel)
        : base(vin, brand, model, currentLocation)
    {
        BatteryLevel = batteryLevel;
    }

    public override void StartEngine()
    {
        try
        {
            base.StartEngine();
            if (BatteryLevel <= 3)
            {
                throw new Exception("Battery critically low! Charge required.");
            }
        }
        catch (Exception ex) when (ex.Message.Contains("Battery critically low"))
        {
            Engine = EngineState.Stopped;
            throw;
        }
    }

    public override string GetResourceStatus() => $"Battery Charge: {BatteryLevel}%";
}