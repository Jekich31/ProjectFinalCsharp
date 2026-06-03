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

    public event Action<string>? OnGeofenceViolation;

    protected Vehicle() { }

    public Vehicle(string vin, string brand, string model, GeoCoordinate startLocation)
    {
        Vin = vin;
        Brand = brand;
        Model = model;
        CurrentLocation = startLocation;
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
        if (Doors == DoorState.Locked)
        {
            throw new Exception("Doors are already locked and secured.");
        }
        Doors = DoorState.Locked;
    }

    public void UnlockDoors()
    {
        if (Doors == DoorState.Unlocked)
        {
            throw new Exception("Doors are already unlocked.");
        }
        Doors = DoorState.Unlocked;
    }

    public virtual void StartEngine()
    {
        if (Engine == EngineState.Running)
        {
            throw new Exception("The engine is already running!");
        }

        if (Doors == DoorState.Unlocked)
        {
            throw new Exception("Security alert! Cannot start the engine while doors are unlocked.");
        }
        Engine = EngineState.Running;
    }

    public void StopEngine()
    {
        if (Engine == EngineState.Stopped)
        {
            throw new Exception("The engine is already stopped!");
        }
        Engine = EngineState.Stopped;
    }

    public abstract string GetResourceStatus();
}