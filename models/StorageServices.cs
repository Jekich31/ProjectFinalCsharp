using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace ProjectFIN.models;

public class StorageService
{
    private const string FileName = "vehicles_db.json";

    public static void SaveVehicles(List<Vehicle> vehicles)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(vehicles, options);
        File.WriteAllText(FileName, jsonString);
    }

    public static List<Vehicle> LoadVehicles()
    {
        if (!File.Exists(FileName))
        {
            return new List<Vehicle>();
        }

        try
        {
            string jsonString = File.ReadAllText(FileName);
            var vehicles = JsonSerializer.Deserialize<List<Vehicle>>(jsonString);
            return vehicles ?? new List<Vehicle>();
        }
        catch
        {
            return new List<Vehicle>();
        }
    }
}