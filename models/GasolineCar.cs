using System;
using System.Text.Json.Serialization;

namespace ProjectFIN.models;

public class GasolineCar : Vehicle
{
    public double FuelLevel { get; init; }

    private GasolineCar() : base() { }

    [JsonConstructor]
    public GasolineCar(string vin, string brand, string model, GeoCoordinate currentLocation, double fuelLevel)
        : base(vin, brand, model, currentLocation)
    {
        FuelLevel = fuelLevel;
    }

    public override void StartEngine()
    {
        try
        {
            base.StartEngine();
            if (FuelLevel <= 0)
            {
                throw new Exception("Fuel tank is empty! Engine cannot start.");
            }
        }
        catch (Exception ex) when (ex.Message.Contains("Fuel tank is empty"))
        {
            Engine = EngineState.Stopped;
            throw;
        }
    }

    public override string GetResourceStatus() => $"Fuel Level: {FuelLevel}%";
}