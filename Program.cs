using ProjectFIN.models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;


List<Vehicle> garage = StorageService.LoadVehicles();

Vehicle? selectedCar = null;

foreach (var car in garage)
{
    car.OnGeofenceViolation += (alarmMessage) =>
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[SECURITY ALERT SYSTEM]: {alarmMessage}");
        Console.ResetColor();
    };
}

bool isRunning = true;
Console.OutputEncoding = System.Text.Encoding.UTF8;

while (isRunning)
{
    Console.Clear();
    Console.WriteLine("==================================================");
    Console.WriteLine("         SMART CAR TELEMATICS SYSTEM         ");
    Console.WriteLine("==================================================");

    Console.WriteLine($" Total vehicles in Garage: {garage.Count}");
    Console.WriteLine($" EVs: {garage.Count(v => v is ElectricCar)} | Gasoline: {garage.Count(v => v is GasolineCar)}");
    Console.WriteLine("--------------------------------------------------");

    if (selectedCar == null)
    {
        Console.WriteLine(" [Active Car]: None selected. Please add or select a vehicle.");
    }
    else
    {
        Console.WriteLine($" [Active Car]: -> {selectedCar.Brand} {selectedCar.Model}");
        Console.WriteLine($" [VIN]: {selectedCar.Vin}");
        Console.WriteLine($" [Doors]: {selectedCar.Doors} | [Engine]: {selectedCar.Engine}");
        Console.WriteLine($" [Resource]: {selectedCar.GetResourceStatus()}");
        Console.WriteLine($" [Coordinates]: {selectedCar.CurrentLocation.Latitude}, {selectedCar.CurrentLocation.Longitude}");
    }

    Console.WriteLine("--------------------------------------------------");
    Console.WriteLine("1. Add a new vehicle to garage");
    Console.WriteLine("2. Select active vehicle from garage (LINQ search)");
    Console.WriteLine("3. Start engine");
    Console.WriteLine("4. Stop engine");
    Console.WriteLine("5. Unlock doors");
    Console.WriteLine("6. Lock doors");
    Console.WriteLine("7. Simulate movement (Change coordinates & trigger Event)");
    Console.WriteLine("8. Exit program");
    Console.WriteLine("--------------------------------------------------");
    Console.Write("Select an option (1-8): ");

    string? choice = Console.ReadLine();
    Console.WriteLine();

    switch (choice)
    {
        case "1":
            Console.WriteLine("Enter car data separated by spaces: [Brand] [Model] [VIN] [Latitude] [Longitude]");
            Console.WriteLine("Example: Toyota Camry VIN12345 50.45 30,52");
            Console.Write("Input: ");

            string? carInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(carInput))
            {
                string[] parts = carInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 5)
                {
                    string brand = parts[0];
                    string model = parts[1];
                    string vin = parts[2];
                    if (garage.Any(v => v.Vin.Equals(vin, StringComparison.OrdinalIgnoreCase)))
                    {
                        Console.WriteLine("\n[ERROR]: A vehicle with this VIN already exists in your garage!");
                        break;
                    }

                    if (double.TryParse(parts[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double lat) &&
    double.TryParse(parts[4], NumberStyles.Any, CultureInfo.InvariantCulture, out double lng))
                    {
                        Vehicle newCar;
                        if (brand.Equals("Tesla", StringComparison.OrdinalIgnoreCase))
                        {
                            newCar = new ElectricCar(vin, brand, model, new GeoCoordinate(lat, lng), 85.0);
                        }
                        else
                        {
                            newCar = new GasolineCar(vin, brand, model, new GeoCoordinate(lat, lng), 45.0);
                        }

                        newCar.OnGeofenceViolation += (alarmMessage) =>
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"\n[SECURITY ALERT SYSTEM]: {alarmMessage}");
                            Console.ResetColor();
                        };

                        newCar.SetGeofence(lat, lng, 500.0);

                        garage.Add(newCar);
                        StorageService.SaveVehicles(garage);
                        selectedCar = newCar;

                        Console.WriteLine($"\n[SUCCESS]: {newCar.Brand} has been successfully added and saved!");
                    }
                    else
                    {
                        Console.WriteLine("\n[ERROR]: Latitude and longitude must be valid floating-point numbers!");
                    }
                }
                else
                {
                    Console.WriteLine($"\n[ERROR]: Invalid format. You entered {parts.Length} elements instead of 5.");
                }
            }
            break;

        case "2":
            if (garage.Count == 0) { Console.WriteLine("[INFO]: Your garage is empty."); break; }

            Console.WriteLine("=== YOUR GARAGE VEHICLES ===");
            foreach (var car in garage)
            {
                Console.WriteLine($"- {car.Brand} {car.Model} (VIN: {car.Vin})");
            }
            Console.Write("\nEnter the VIN of the car you want to control: ");
            string? searchVin = Console.ReadLine();

            var foundCar = garage.FirstOrDefault(v => v.Vin.Equals(searchVin, StringComparison.OrdinalIgnoreCase));

            if (foundCar != null)
            {
                selectedCar = foundCar;
                Console.WriteLine($"[SUCCESS]: Switched to {selectedCar.Brand} {selectedCar.Model}.");
            }
            else
            {
                Console.WriteLine("[ERROR]: Vehicle with that VIN was not found.");
            }
            break;

        case "3":
            if (selectedCar == null) { Console.WriteLine("[ERROR]: Please select a vehicle first!"); break; }
            try
            {
                selectedCar.StartEngine();
                Console.WriteLine("[COMMAND]: Start signal sent. Engine is running!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SECURITY ALARM]: {ex.Message}");
            }
            break;

        case "4": 
            if (selectedCar == null) { Console.WriteLine("[ERROR]: Please select a vehicle first!"); break; }
            try
            {
                selectedCar.StopEngine();
                StorageService.SaveVehicles(garage);
                Console.WriteLine("[COMMAND]: Engine stopped.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ENGINE ERROR]: {ex.Message}");
            }
            break;

        case "5":
            if (selectedCar == null) { Console.WriteLine("[ERROR]: Please select a vehicle first!"); break; }
            try {
                selectedCar.UnlockDoors();
                if(selectedCar.Doors==DoorState.Unlocked)
                {
                    throw new Exception("Doors are already unlocked.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[SECURITY ALARM]: {ex.Message}");
                break;
            }
            Console.WriteLine("[COMMAND]: Doors unlocked.");
            break;

        case "6":
            if (selectedCar == null) { Console.WriteLine("[ERROR]: Please select a vehicle first!"); break; }
            try
            {
                selectedCar.LockDoors();
                if(selectedCar.Doors == DoorState.Locked)
                {
                    throw new Exception("Doors are already locked and secured.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SECURITY ALARM]: {ex.Message}");
                break;
            }
            Console.WriteLine("[COMMAND]: Doors locked and secured.");
            break;

        case "7":
            if (selectedCar == null) { Console.WriteLine("[ERROR]: Please select a vehicle first!"); break; }

            Console.Write("Enter new coordinates separated by space (Latitude Longitude): ");
            string? locInput = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(locInput))
            {
                string[] coords = locInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (coords.Length == 2 &&
    double.TryParse(coords[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double newLat) &&
    double.TryParse(coords[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double newLng))
                {
                    selectedCar.UpdateLocation(newLat, newLng);
                    StorageService.SaveVehicles(garage);
                    Console.WriteLine($"[TELEMETRY]: Coordinates updated to {newLat}, {newLng}");
                }
                else
                {
                    Console.WriteLine("[ERROR]: Invalid coordinate input.");
                }
            }
            break;

        case "8":
            isRunning = false;
            StorageService.SaveVehicles(garage);
            Console.WriteLine("Goodbye!");
            break;

        default:
            Console.WriteLine("[WARNING]: Invalid choice, please try again (select 1-8).");
            break;
    }

    if (isRunning)
    {
        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }
}