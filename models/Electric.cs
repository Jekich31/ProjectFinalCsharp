namespace ProjectFIN.models;

public class ElectricCar : Vehicle
{
    public double BatteryLevel { get; private set; } = 100.0;

    public ElectricCar(string vin, string brand, string model, GeoCoordinate startLocation, double battery)
        : base(vin, brand, model, startLocation)
    {
        BatteryLevel = battery;
    }

    public override void StartEngine()
    {
        base.StartEngine(); // сначала проверит двери
        if (BatteryLevel <= 3)
        {
            Engine = EngineState.Stopped;
            throw new Exception("Battery critically low! Charge required.");
        }
    }

    public override string GetResourceStatus() => $"Battery Charge: {BatteryLevel}%";
}