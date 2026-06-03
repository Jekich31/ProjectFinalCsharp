using System;

namespace ProjectFIN.models;

public class ElectricCar : Vehicle
{
    public double BatteryLevel { get; init; } = 100.0;

    private ElectricCar() : base() { }

    public ElectricCar(string vin, string brand, string model, GeoCoordinate startLocation, double battery)
        : base(vin, brand, model, startLocation)
    {
        BatteryLevel = battery;
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