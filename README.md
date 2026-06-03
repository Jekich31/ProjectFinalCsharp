# Remote Car Control System

![.NET Version](https://img.shields.io/badge/.NET-8.0%20%2F%209.0-blue)
![Project Type](https://img.shields.io/badge/Project-Console%20Application-green)
![Architecture](https://img.shields.io/badge/Architecture-OOP%20%2F%20Patterns-orange)

**Remote Car Control** is a .NET console application that simulates a modern ecosystem of smart vehicles. This project is designed to demonstrate practical proficiency in core and advanced C# concepts, object-oriented design (OOD), design patterns, and resource management.

The application allows users to manage their fleet, track geolocation in real time, receive instant alerts regarding security boundary breaches (Geofencing), and maintain a detailed audit trail of system logs.

---

## 🚀 Key Features

* **Fleet Management:** User registration, adding electric vehicles (`ElectricCar`) and conventional cars (`GasolineCar`) utilizing the Factory Method pattern.
* **Remote Control:** Locking/unlocking doors, starting and stopping the engine through a unified interface.
* **Telemetry Simulation & Geofencing:** Real-time updates of vehicle coordinates with automatic boundary violation checks driven by events.
* **Full State Persistence:** Automatic serialization of the entire database into JSON format upon exiting, and seamless deserialization on application startup.
* **End-to-End Logging:** Keeping a continuous record of all critical events and user actions in a `telemetry.log` file.

---

## 🛠️ Technical Stack & Implementation (By Course Modules)

### Module 1: Fundamentals, Data Structures & Core Logic
* **C# 12/13 (.NET 8/9):** Leveraging modern syntax features, including **Top-level statements** in `Program.cs` for a clean, concise entry point.
* **Encapsulation:** Entity properties utilize restricted access modifiers (`get; private set;` / `get; init;`), preventing accidental modifications of critical data (IDs, VIN codes).
* **Architectural Separation:** Clear boundaries established using dedicated namespaces: `CarControl.Models`, `CarControl.Services`, `CarControl.Interfaces`.
* **Efficient Data Types:**
