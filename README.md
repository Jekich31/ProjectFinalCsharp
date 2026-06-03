# Remote Car Control System

![.NET Version](https://img.shields.io/badge/.NET-8.0%20%2F%209.0-blue)
![Project Type](https://img.shields.io/badge/Project-Console%20Application-green)
![Architecture](https://img.shields.io/badge/Architecture-OOP%20%2F%20Patterns-orange)

**Remote Car Control** ist eine .NET-Konsolenanwendung, die ein modernes Ökosystem für intelligente Fahrzeuge simuliert. Das Projekt wurde entwickelt, um die praktische Beherrschung grundlegender und fortgeschrittener C#-Konzepte, objektorientierten Designs (OOD), Entwurfsmustern und Ressourcenmanagements zu demonstrieren.

Die Anwendung ermöglicht es Benutzern, ihren eigenen Fuhrpark zu verwalten, die Geoposition in Echtzeit zu verfolgen, sofortige Benachrichtigungen bei Sicherheitsbereichsverletzungen (Geofencing) zu erhalten und ein detailliertes Audit-Trail von Telemetrielogs zu führen.

---

## 🚀 Hauptfunktionen

* **Fuhrparkmanagement:** Benutzerregistrierung, Hinzufügen von Elektrofahrzeugen (`ElectricCar`) und herkömmlichen Autos (`GasolineCar`) unter Verwendung des Factory-Method-Musters.
* **Fernsteuerung:** Sperren/Entsperren von Türen, Starten und Stoppen des Motors über eine einheitliche Schnittstelle.
* **Telemetriesimulation & Geofencing:** Echtzeit-Aktualisierung von Fahrzeugkoordinaten mit automatischer Überprüfung von Grenzüberschreitungen ("Heimatzone") gesteuert durch Events.
* **Vollständige Statuspersistenz:** Automatische Serialisierung der gesamten Datenbank in das JSON-Format beim Beenden und nahtlose Deserialisierung beim Anwendungsstart.
* **End-to-End-Logging:** Kontinuierliche Aufzeichnung aller kritischen Ereignisse und Benutzeraktionen in einer `telemetry.log`-Datei.

---

## 🛠️ Technischer Stack & Implementierung (Nach Kursmodulen)

### Modul 1: Grundlagen, Datenstrukturen & Kernlogik
* **C# 12/13 (.NET 8/9):** Nutzung moderner Syntaxfeatures, einschließlich **Top-level statements** in `Program.cs` für einen sauberen, prägnanten Einstiegspunkt.
* **Kapselung:** Entitätseigenschaften verwenden restriktive Zugriffsmodifizierer (`get; private set;` / `get; init;`), was versehentliche Änderungen kritischer Daten (IDs, VIN-Codes) verhindert.
* **Architektonische Trennung:** Klare Abgrenzung durch dedizierte Namespaces: `CarControl.Models`, `CarControl.Services`, `CarControl.Interfaces`.
* **Effiziente Datentypen:** Geolocation-Koordinaten sind als leistungseffiziente `struct GeoCoordinate` implementiert, und Fahrzeugzustände nutzen typsichere `enum`-Strukturen (`EngineState`, `DoorState`).
* **Nullable-Typen:** Verwendung zur flexiblen Handhabung optionaler oder fehlender Daten (z. B. `GeoCoordinate? HomeZone`).
* **Benutzerdefinierte Ausnahmen (Exceptions):** Domänenspezifische Fehlerbehandlung wird über benutzerdefinierte Ausnahmen wie `CarSecurityException` verwaltet (z. B. beim Versuch, den Motor bei entriegelten Türen zu starten).

### Modul 2: Arrays, Kollektionen & Fortgeschrittenes OOP
* **Validierung & String-API:** Strikte Überprüfung von eindeutigen 17-stelligen VIN-Codes und umfassende Verarbeitung eingehender Benutzungsbefehle.
* **Operatorüberladung:** Die Operatoren `==` und `!=` wurden für die Klasse `Car` überdefiniert, um einen direkten Objektvergleich basierend auf eindeutigen VIN-Codes zu ermöglichen.
* **Indexer:** Implementierung der Indizierung in der Klasse `User` für schnelle Fahrzeug-Lookups nach Index `user[0]` oder nach einem bestimmten VIN-Code `user["VIN123"]`.
* **Polymorphie:** Aufbauend auf einer abstrakten Basisklasse `Vehicle` und ihren abgeleiteten Klassen `ElectricCar` (Überwachung der Batteriekapazität) und `GasolineCar` (Simulation des Kraftstoffverbrauchs) mit überschriebenem `DisplayStatus()`-Verhalten.
* **Schnittstellen (Interfaces):** Der Kontrollvertrag für die Hardwaresteuerung wird durch `IRemoteControllable` strikt definiert und erzwungen.

### Modul 3: Events, Generics, LINQ & Datenverarbeitung
* **Events & Records:** Das `OnGeofenceViolation`-Event nutzt unveränderliche `record`-Typen, um Daten über Zonenverletzungen zuverlässig zu übertragen.
* **Generics:** Erstellung einer generischen Klasse `CommandsHistory<T>` zur Speicherung eines Audit-Trails von Aktionen oder Bewegungspfaden.
* **LINQ:** Verwendung von Fluent-/Query-Syntax für eine erweiterte Fuhrparkfiltrierung (Finden von Fahrzeugen mit niedrigem Energiestatus, Sortieren nach Marke, Auswahl laufender Autos).
* **I/O & Serialisierung:** Dateiauditierung implementiert mittels `StreamWriter` und Erhalt des Systemstatus über `System.Text.Json`.

### Modul 4: Architektur & Ressourcenmanagement
* **Speicherverwaltung:** Die ordnungsgemäße Implementierung des `IDisposable`-Musters zusammen mit `using`-Anweisungen garantiert die sichere Freigabe von Logdatei-Handles.

---

## 🖥️ Struktur des Konsolenmenüs

```text
Hauptbildschirm
 ├── [1] Registrieren
 ├── [2] Im persönlichen Bereich anmelden
 └── [3] Beenden (JSON-Autosave)

Persönlicher Bereich (LINQ-Fuhrparkmonitor)
 ├── [1] Fahrzeug hinzufügen (Fabrikmethode: Electric / Gasoline)
 ├── [2] Fahrzeug aus der Garage auswählen (Indexer-Suche)
 └── [3] Zurück

Fahrzeugsteuerungs-Bildschirm (IRemoteControllable)
 ├── Status: [Türen: Locked | Motor: Stopped | Batterie: 85% | Koordinaten: 50.45, 30.52]
 ├── Befehle: Sperren/Entsperren, Motor Starten/Stoppen
 └── [Koordinatenänderung simulieren] ──> (Trigger Geofence -> Event -> Eintrag in .log)
