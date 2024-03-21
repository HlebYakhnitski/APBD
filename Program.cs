using System;
using System.Collections.Generic;
using System.Linq;

public abstract class TransportUnit
{
    public double LoadWeight { get; protected set; }
    public int UnitHeight { get; private set; }
    public double BaseWeight { get; private set; }
    public int UnitDepth { get; private set; }
    public string UnitID { get; private set; }
    public double MaximumLoad { get; private set; }

    protected TransportUnit(int height, double baseWeight, int depth, string id, double maxLoad)
    {
        UnitHeight = height;
        BaseWeight = baseWeight;
        UnitDepth = depth;
        UnitID = id;
        MaximumLoad = maxLoad;
    }

    public abstract void Fill(double weight);
    public abstract void ClearLoad();
}

public interface IAlertSystem
{
    void HazardAlert();
}

public class FluidUnit : TransportUnit, IAlertSystem
{
    public bool Hazard { get; private set; }

    public FluidUnit(int height, double baseWeight, int depth, string id, double maxLoad, bool hazard)
        : base(height, baseWeight, depth, id, maxLoad)
    {
        Hazard = hazard;
    }

    public override void Fill(double weight)
    {
        double permissibleLoad = Hazard ? MaximumLoad * 0.5 : MaximumLoad * 0.9;
        if (weight > permissibleLoad) throw new CapacityExceededException("Exceeds permissible load.");
        LoadWeight = weight;
    }

    public override void ClearLoad()
    {
        LoadWeight = 0;
    }

    public void HazardAlert()
    {
        if (Hazard)
        {
            Console.WriteLine($"Warning: Hazardous material in unit {UnitID}");
        }
    }
}

public class CapacityExceededException : Exception
{
    public CapacityExceededException(string message) : base(message) { }
}

public class GasUnit : TransportUnit, IAlertSystem
{
    public double UnitPressure { get; private set; }

    public GasUnit(int height, double baseWeight, int depth, string id, double maxLoad, double pressure)
        : base(height, baseWeight, depth, id, maxLoad)
    {
        UnitPressure = pressure;
    }

    public override void Fill(double weight)
    {
        if (weight > MaximumLoad) throw new CapacityExceededException("Exceeds maximum load.");
        LoadWeight = weight;
    }

    public override void ClearLoad()
    {
        LoadWeight *= 0.05;
    }

    public void HazardAlert()
    {
        Console.WriteLine($"Warning: Gas unit {UnitID} is under hazardous conditions.");
    }
}

public class CoolUnit : TransportUnit
{
    public string StoredProduct { get; private set; }
    public double SetTemperature { get; private set; }

    public CoolUnit(int height, double baseWeight, int depth, string id, double maxLoad, string product, double temperature)
        : base(height, baseWeight, depth, id, maxLoad)
    {
        StoredProduct = product;
        SetTemperature = temperature;
    }

    public override void Fill(double weight)
    {
        if (weight > MaximumLoad) throw new CapacityExceededException("Exceeds maximum load.");
        LoadWeight = weight;
    }

    public override void ClearLoad()
    {
        LoadWeight = 0;
    }
}

public class Vessel
{
    public List<TransportUnit> Units { get; private set; } = new List<TransportUnit>();
    public double SpeedLimit { get; private set; }
    public int Capacity { get; private set; }
    public double WeightLimit { get; private set; }

    public Vessel(double speed, int capacity, double limit)
    {
        SpeedLimit = speed;
        Capacity = capacity;
        WeightLimit = limit;
    }

    public void AddUnit(TransportUnit unit)
    {
        if (Units.Count >= Capacity) throw new Exception("ship at full capacity.");
        if (Units.Sum(u => u.LoadWeight + u.BaseWeight) / 1000 + (unit.LoadWeight + unit.BaseWeight) / 1000 > WeightLimit) throw new Exception("Exceeds vessel's weight limit.");

        Units.Add(unit);
    }

    public void RemoveUnit(string id)
    {
        var unit = Units.FirstOrDefault(u => u.UnitID == id);
        if (unit != null) Units.Remove(unit);
    }
}

class VesselManagementSystem
{
    static void Main(string[] args)
    {
        var fleet = new List<Vessel>();
        var cargoUnits = new List<TransportUnit>();
        string action = "";

        while (action.ToLower() != "exit")
        {
            Console.WriteLine("\nVessel Management Console");
            Console.WriteLine("1. Register a new ship");
            Console.WriteLine("2. Create a new cargo unit");
            Console.WriteLine("3. Assign cargo to a ship");
            Console.WriteLine("4. Remove cargo from a ship");
            Console.WriteLine("5. Display fleet and cargo details");
            Console.WriteLine("Type 'exit' to close the application");
            Console.Write("Choose an operation: ");
            action = Console.ReadLine();

            switch (action)
            {
                case "1":
                    RegisterVessel(fleet);
                    break;
                case "2":
                    CreateCargoUnit(cargoUnits);
                    break;
                case "3":
                    AssignCargoToVessel(fleet, cargoUnits);
                    break;
                case "4":
                    RemoveCargoFromVessel(fleet);
                    break;
                case "5":
                    DisplayFleetAndCargo(fleet);
                    break;
                default:
                    if (action.ToLower() != "exit")
                    {
                        Console.WriteLine("Unknown command, please try again.");
                    }

                    break;
            }
        }
    }

    static void RegisterVessel(List<Vessel> fleet)
    {
        Console.Write("Enter ship's maximum speed: ");
        double speed = double.Parse(Console.ReadLine());
        Console.Write("Enter ship's cargo capacity: ");
        int capacity = int.Parse(Console.ReadLine());
        Console.Write("Enter ship's weight limit (tons): ");
        double limit = double.Parse(Console.ReadLine());

        var vessel = new Vessel(speed, capacity, limit);
        fleet.Add(vessel);
        Console.WriteLine("ship registered successfully.");
    }

    static void CreateCargoUnit(List<TransportUnit> cargoUnits)
    {
        Console.WriteLine("Choose the type of cargo unit (1. Fluid, 2. Gas, 3. Cool): ");
        string choice = Console.ReadLine();
        int height, depth;
        double baseWeight, maxLoad;

        Console.Write("Enter unit height: ");
        height = int.Parse(Console.ReadLine());
        Console.Write("Enter unit base weight: ");
        baseWeight = double.Parse(Console.ReadLine());
        Console.Write("Enter unit depth: ");
        depth = int.Parse(Console.ReadLine());
        Console.Write("Enter unit ID: ");
        string id = Console.ReadLine();
        Console.Write("Enter unit's maximum load: ");
        maxLoad = double.Parse(Console.ReadLine());

        TransportUnit unit = null;
        switch (choice)
        {
            case "1": // Fluid
                Console.Write("Is the material hazardous? (true/false): ");
                bool hazardous = bool.Parse(Console.ReadLine());
                unit = new FluidUnit(height, baseWeight, depth, id, maxLoad, hazardous);
                break;
            case "2": // Gas
                Console.Write("Enter unit pressure: ");
                double pressure = double.Parse(Console.ReadLine());
                unit = new GasUnit(height, baseWeight, depth, id, maxLoad, pressure);
                break;
            case "3": // Cool
                Console.Write("Enter stored product type: ");
                string product = Console.ReadLine();
                Console.Write("Enter temperature setting: ");
                double temperature = double.Parse(Console.ReadLine());
                unit = new CoolUnit(height, baseWeight, depth, id, maxLoad, product, temperature);
                break;
        }

        if (unit != null)
        {
            cargoUnits.Add(unit);
            Console.WriteLine("Cargo unit created successfully.");
        }
        else
        {
            Console.WriteLine("Invalid choice of cargo unit.");
        }
    }

    static void AssignCargoToVessel(List<Vessel> fleet, List<TransportUnit> cargoUnits)
    {
        if (fleet.Count == 0 || cargoUnits.Count == 0)
        {
            Console.WriteLine("Operation not possible. Either no ships or no cargo units available.");
            return;
        }

        Console.WriteLine("Choose a ship by index:");
        for (int i = 0; i < fleet.Count; i++)
        {
            Console.WriteLine(
                $"{i + 1}. ship with speed limit {fleet[i].SpeedLimit} and capacity {fleet[i].Capacity}");
        }

        int vesselIndex = int.Parse(Console.ReadLine()) - 1;

        Console.WriteLine("Select a cargo unit by index:");
        for (int i = 0; i < cargoUnits.Count; i++)
        {
            Console.WriteLine($"{i + 1}. Cargo unit with ID {cargoUnits[i].UnitID}");
        }

        int unitIndex = int.Parse(Console.ReadLine()) - 1;

        try
        {
            fleet[vesselIndex].AddUnit(cargoUnits[unitIndex]);
            Console.WriteLine("Cargo unit assigned to the ship successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to assign cargo unit to the ship: {ex.Message}");
        }
    }

    static void RemoveCargoFromVessel(List<Vessel> fleet
    )
    {
        if (fleet.Count == 0)
        {
            Console.WriteLine("There are no ships registered in the system.");
            return;
        }

        Console.WriteLine("Choose a ship by index to remove cargo from:");
        for (int i = 0; i < fleet.Count; i++)
        {
            Console.WriteLine(
                $"{i + 1}. ship with speed limit {fleet[i].SpeedLimit} and capacity for {fleet[i].Capacity} units");
        }

        int vesselIndex = int.Parse(Console.ReadLine()) - 1;

        if (fleet[vesselIndex].Units.Count == 0)
        {
            Console.WriteLine("This ship has no cargo units loaded.");
            return;
        }

        Console.Write("Enter the ID of the cargo unit to remove: ");
        string unitID = Console.ReadLine();

        try
        {
            fleet[vesselIndex].RemoveUnit(unitID);
            Console.WriteLine("Cargo unit removed from the ship successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to remove cargo unit from the ship: {ex.Message}");
        }
    }

    static void DisplayFleetAndCargo(List<Vessel> fleet)
    {
        if (fleet.Count == 0)
        {
            Console.WriteLine("No ships are currently registered.");
            return;
        }

        foreach (var vessel in fleet)
        {
            Console.WriteLine(
                $"\nship Details: Maximum Speed: {vessel.SpeedLimit} knots, Capacity: {vessel.Capacity} units, Weight Limit: {vessel.WeightLimit} tons");
            if (vessel.Units.Count > 0)
            {
                Console.WriteLine("Loaded Cargo Units:");
                foreach (var unit in vessel.Units)
                {
                    Console.WriteLine(
                        $"- ID: {unit.UnitID}, Load Weight: {unit.LoadWeight} kg, Type: {unit.GetType().Name.Replace("Unit", "")}");
                }
            }
            else
            {
                Console.WriteLine("No cargo units are currently loaded on this vessel.");
            }
        }
    }
}

