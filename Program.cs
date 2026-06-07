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

    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine("==================================================");
    Console.WriteLine("         REMOTE CAR CONTROL SYSTEM         ");
    Console.WriteLine("==================================================");
    Console.ResetColor();

    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine($" Total vehicles in Garage: {garage.Count}");
    Console.WriteLine($" EVs: {garage.Count(v => v is ElectricCar)} | Gasoline: {garage.Count(v => v is GasolineCar)}");
    Console.WriteLine("--------------------------------------------------");
    Console.ResetColor();

    if (selectedCar == null)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(" [Active Car]: None selected. Please add or select a vehicle.");
        Console.ResetColor();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write(" [Active Car]: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"-> {selectedCar.Brand} {selectedCar.Model}");

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($" [VIN]: {selectedCar.Vin}");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($" [Doors]: {selectedCar.Doors} | [Engine]: {selectedCar.Engine}");
        Console.WriteLine($" [Resource]: {selectedCar.GetResourceStatus()}");
        Console.WriteLine($" [Coordinates]: {selectedCar.CurrentLocation.Latitude}, {selectedCar.CurrentLocation.Longitude}");
        Console.ResetColor();
    }

    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine("--------------------------------------------------");
    Console.ResetColor();

    Console.WriteLine("1. Add a new vehicle to garage");
    Console.WriteLine("2. Select active vehicle from garage (LINQ search)");
    Console.WriteLine("3. Start engine");
    Console.WriteLine("4. Stop engine");
    Console.WriteLine("5. Unlock doors");
    Console.WriteLine("6. Lock doors");
    Console.WriteLine("7. Simulate movement (Change coordinates & trigger Event)");
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine("8. Exit program");
    Console.ResetColor();

    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine("--------------------------------------------------");
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("Select an option (1-8): ");
    Console.ResetColor();

    string? choice = Console.ReadLine();
    Console.WriteLine();

    switch (choice)
    {
        case "1":
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Enter car data separated by spaces: [Brand] [Model] [VIN] [Latitude] [Longitude]");
            Console.WriteLine("Example: Toyota Camry VIN12345 50.45 30.52");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Input: ");
            Console.ResetColor();

            string? carInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(carInput))
            {
                string[] parts = carInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 5)
                {
                    string brand = parts[0];
                    string model = parts[1];
                    string vin = parts[2];

                    string latStr = parts[3].Replace(',', '.');
                    string lngStr = parts[4].Replace(',', '.');

                    if (double.TryParse(latStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double lat) &&
                        double.TryParse(lngStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double lng))
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

                        newCar.SetGeofence(lat, lng, 500.0);
                        newCar.OnGeofenceViolation += (alarmMessage) =>
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"\n[SECURITY ALERT SYSTEM]: {alarmMessage}");
                            Console.ResetColor();
                        };

                        garage.Add(newCar);
                        StorageService.SaveVehicles(garage);
                        selectedCar = newCar;

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\n[SUCCESS]: {newCar.Brand} has been successfully added and saved!");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n[ERROR]: Latitude and longitude must be valid numbers!");
                        Console.ResetColor();
                    }
                }
            }
            break;

        case "2":
            if (garage.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[INFO]: Your garage is empty.");
                Console.ResetColor();
                break;
            }

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("=== YOUR GARAGE VEHICLES ===");
            Console.ResetColor();
            foreach (var car in garage)
            {
                Console.WriteLine($"- {car.Brand} {car.Model} (VIN: {car.Vin})");
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nEnter the VIN of the car you want to control: ");
            Console.ResetColor();
            string? searchVin = Console.ReadLine();

            var foundCar = garage.FirstOrDefault(v => v.Vin.Equals(searchVin, StringComparison.OrdinalIgnoreCase));
            if (foundCar != null)
            {
                selectedCar = foundCar;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[SUCCESS]: Switched to {foundCar.Brand} {foundCar.Model}.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR]: Vehicle with that VIN was not found.");
                Console.ResetColor();
            }
            break;

        case "3":
            if (selectedCar == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR]: Please select a vehicle first!");
                Console.ResetColor();
                break;
            }
            try
            {
                selectedCar.StartEngine();
                StorageService.SaveVehicles(garage);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[COMMAND]: Start signal sent. Engine is running!");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[SECURITY ALARM]: {ex.Message}");
                Console.ResetColor();
            }
            break;

        case "4":
            if (selectedCar == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR]: Please select a vehicle first!");
                Console.ResetColor();
                break;
            }
            try
            {
                selectedCar.StopEngine();
                StorageService.SaveVehicles(garage);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[COMMAND]: Engine stopped.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ENGINE ERROR]: {ex.Message}");
                Console.ResetColor();
            }
            break;

        case "5":
            if (selectedCar == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR]: Please select a vehicle first!");
                Console.ResetColor();
                break;
            }
            try
            {
                selectedCar.UnlockDoors();
                StorageService.SaveVehicles(garage);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[COMMAND]: Doors unlocked.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[DOOR ERROR]: {ex.Message}");
                Console.ResetColor();
            }
            break;

        case "6":
            if (selectedCar == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR]: Please select a vehicle first!");
                Console.ResetColor();
                break;
            }
            try
            {
                selectedCar.LockDoors();
                StorageService.SaveVehicles(garage);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[COMMAND]: Doors locked and secured.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[DOOR ERROR]: {ex.Message}");
                Console.ResetColor();
            }
            break;

        case "7":
            if (selectedCar == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR]: Please select a vehicle first!");
                Console.ResetColor();
                break;
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Enter new coordinates separated by space (Latitude Longitude): ");
            Console.ResetColor();
            string? locInput = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(locInput))
            {
                string[] coords = locInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (coords.Length == 2)
                {
                    string newLatStr = coords[0].Replace(',', '.');
                    string newLngStr = coords[1].Replace(',', '.');

                    if (double.TryParse(newLatStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double newLat) &&
                        double.TryParse(newLngStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double newLng))
                    {
                        selectedCar.UpdateLocation(newLat, newLng);
                        StorageService.SaveVehicles(garage);
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine($"[TELEMETRY]: Coordinates updated to {newLat}, {newLng}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[ERROR]: Invalid coordinate input.");
                        Console.ResetColor();
                    }
                }
            }
            break;

        case "8":
            isRunning = false;
            StorageService.SaveVehicles(garage);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Goodbye!");
            Console.ResetColor();
            break;

        default:
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[WARNING]: Invalid choice, please try again.");
            Console.ResetColor();
            break;
    }

    if (isRunning)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ResetColor();
        Console.ReadKey();
    }
}