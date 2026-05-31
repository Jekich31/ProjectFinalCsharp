namespace ProjectFIN.models;

public class GasolineCar : Vehicle
{
    public double FuelLevel { get; private set; }

    public GasolineCar(string vin, string brand, string model, GeoCoordinate startLocation, double fuel) 
        : base(vin, brand, model, startLocation)
    {
        FuelLevel = fuel;
    }

    public override void StartEngine()
    {
        base.StartEngine();
        if (FuelLevel <= 0)
        {
            Engine = EngineState.Stopped;
            throw new Exception("Fuel tank is empty! Engine cannot start.");
        }
    }

    public override string GetResourceStatus() => $"Fuel Level: {FuelLevel}%";
}