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