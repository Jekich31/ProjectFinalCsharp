using ProjectFIN.InterFaces;
using System;
using System.Text.Json.Serialization;

namespace ProjectFIN.models;

[JsonDerivedType(typeof(ElectricCar), typeDiscriminator: "EV")]
[JsonDerivedType(typeof(GasolineCar), typeDiscriminator: "Gas")]
public abstract class Vehicle : IRemoteControllable
{
    public string Vin { get; init; }
    public string Brand { get; init; }
    public string Model { get; init; }
    public EngineState Engine { get; protected set; } = EngineState.Stopped;
    public DoorState Doors { get; protected set; } = DoorState.Locked;
    public GeoCoordinate CurrentLocation { get; set; }

    public GeoCoordinate? HomeZone { get; private set; }
    public double AllowedRadius { get; private set; }

    [JsonIgnore]
    private IVehicleState _state;

    public event Action<string>? OnGeofenceViolation;

    protected Vehicle()
    {
        InitializeState();
    }

    public Vehicle(string vin, string brand, string model, GeoCoordinate startLocation)
    {
        Vin = vin;
        Brand = brand;
        Model = model;
        CurrentLocation = startLocation;
        InitializeState();
    }

    private void InitializeState()
    {
        if (Engine == EngineState.Running)
        {
            _state = new RunningState();
        }
        else
        {
            _state = new StoppedState();
        }
    }

    public void SetState(IVehicleState state)
    {
        _state = state;
    }

    public void SetEngineState(EngineState engineState)
    {
        Engine = engineState;
    }

    public void SetDoorState(DoorState doorState)
    {
        Doors = doorState;
    }

    public void SetGeofence(double lat, double lng, double radius)
    {
        HomeZone = new GeoCoordinate(lat, lng);
        AllowedRadius = radius;
    }

    public void UpdateLocation(double lat, double lng)
    {
        CurrentLocation = new GeoCoordinate(lat, lng);

        if (HomeZone.HasValue)
        {
            double distance = CurrentLocation.DistanceTo(HomeZone.Value);
            if (distance > AllowedRadius)
            {
                OnGeofenceViolation?.Invoke($"ALARM! Vehicle {Brand} {Model} (VIN: {Vin}) left the safe zone! Current distance: {distance:F1} meters out of {AllowedRadius}m limit.");
            }
        }
    }

    public void LockDoors()
    {
        _state.LockDoors(this);
    }

    public void UnlockDoors()
    {
        _state.UnlockDoors(this);
    }

    public virtual void StartEngine()
    {
        _state.StartEngine(this);
    }

    public void StopEngine()
    {
        _state.StopEngine(this);
    }

    public abstract string GetResourceStatus();
}