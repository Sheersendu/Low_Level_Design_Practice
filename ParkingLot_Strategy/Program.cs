using System.Collections.Concurrent;

class BaseClass
{
	public Guid Id { get; init; } = Guid.NewGuid();
	public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

enum SpotType
{
	Small = 0, Medium = 1, Large = 2
}

class Spot : BaseClass
{
	public string Name { get; set; }
	public Vehicle? vehicle { get; set; }
	public SpotType Type { get; set; }
	public bool IsAvailable { get; set; }
	private object _spot = new();
	
	public void ReserveSpot(Vehicle currentVehicle)
	{
		lock (_spot)
		{
			vehicle = currentVehicle;
			IsAvailable = false;
		}
	}
	
	public void UnreserveSpot()
	{
		lock (_spot)
		{
			vehicle = null;
			IsAvailable = true;
		}
	}
}

class Floor(int floorLevel) : BaseClass
{
	public int Level { get; set; } = floorLevel;
	private List<Spot> _spots = new();
	private object _floor = new();

	public List<Spot> GetAvailableSlots()
	{
		return _spots.Where(spot => spot.IsAvailable).ToList();
	}

	public void AddSpot(Spot newSpot)
	{
		lock (_floor)
		{
			_spots.Add(newSpot);
		}
	}
}

enum VehicleType
{
	Small = 0, Medium = 1, Large = 2
}

abstract class Vehicle(string vehicleNumber, VehicleType vehicleType) : BaseClass
{
	public string Number { get; } = vehicleNumber;
	public VehicleType Type { get; } = vehicleType;
}

class Car(string vehicleNumber) :  Vehicle(vehicleNumber, VehicleType.Medium){}
class Bike(string vehicleNumber) :  Vehicle(vehicleNumber, VehicleType.Small){}
class Truck(string vehicleNumber) :  Vehicle(vehicleNumber, VehicleType.Large){}

interface IParkingStrategy
{
	Spot? GetAvailableSpot(List<Floor> floors, VehicleType vehicleType);
}

class NearestSpotStrategy : IParkingStrategy
{
	public Spot? GetAvailableSpot(List<Floor> floors, VehicleType vehicleType)
	{
		foreach (Floor floor in floors)
		{
			foreach (Spot spot in floor.GetAvailableSlots())
			{
				if ((int)spot.Type >= (int)vehicleType)
				{
					return spot;
				}
			}
		}
		Console.WriteLine("No available spot!");
		return null;
	}
}

class ParkingTicket(Vehicle currentVehicle, Spot currentSpot) : BaseClass
{
	public Vehicle vehicle { get; } = currentVehicle;
	public Spot spot { get; } = currentSpot;
	public DateTime EntryTime { get; } = DateTime.UtcNow;
	public DateTime ExitTime { get; set; }
}

interface IParkingPricing
{
	double GetParkingPrice(ParkingTicket ticket);
}

class BasicPricing : IParkingPricing
{
	// Assuming the hourly charge is Rs.10 
	private const int Rate = 10;
	
	public double GetParkingPrice(ParkingTicket ticket)
	{
		var totalMinutes = (ticket.ExitTime - ticket.EntryTime).TotalMinutes;
		double price = (totalMinutes*Rate)/60;
		return price;
	}
}

class VehicleTypePricing : IParkingPricing
{
	private static readonly ConcurrentDictionary<VehicleType, int> _vehiclePrices = new();
	public VehicleTypePricing()
	{
		_vehiclePrices.TryAdd(VehicleType.Small, 10);
		_vehiclePrices.TryAdd(VehicleType.Medium, 20);
		_vehiclePrices.TryAdd(VehicleType.Large, 30);
	}
		
	public double GetParkingPrice(ParkingTicket ticket)
	{
		var totalMinutes = (ticket.ExitTime - ticket.EntryTime).TotalMinutes;
		_vehiclePrices.TryGetValue(ticket.vehicle.Type, out int Rate);
		double price = (totalMinutes*Rate)/60;
		return price;
	}
}

class ParkingLot
{
	private static ParkingLot instance;
	private ConcurrentBag<Floor> _floors = new();
	private ConcurrentDictionary<Guid, ParkingTicket> _tickets = new();
	private IParkingPricing _parkingPricing = new BasicPricing();
	private IParkingStrategy _parkingStrategy = new NearestSpotStrategy();
	private static object _parkinglot = new();
	private object _parkingfloor = new();
	
	private ParkingLot(){}

	public static ParkingLot GetInstance()
	{
		if (instance is null)
		{
			lock (_parkinglot)
			{
				if (instance is null)
				{
					instance = new ParkingLot();
				}
			}
		}
		return instance;
	}

	public void AddFloor(Floor newFloor)
	{
		lock (_parkingfloor)
		{
			_floors.Add(newFloor);
		}
	}

	public void SetParkingStrategy(IParkingStrategy newStrategy)
	{
		_parkingStrategy = newStrategy;
	}
	
	public void SetParkingPricing(IParkingPricing newParkingPricing)
	{
		_parkingPricing = newParkingPricing;
	}

	public ParkingTicket? Park(Vehicle currentVehicle)
	{
		Spot? spot = _parkingStrategy.GetAvailableSpot(_floors.ToList(), currentVehicle.Type);
		if (!(spot is null))
		{
			spot.ReserveSpot(currentVehicle);
			ParkingTicket newTicket = new(currentVehicle, spot);
			_tickets.TryAdd(newTicket.Id, newTicket);
			return newTicket;
		}

		return null;
	}

	public double UnPark(ParkingTicket ticket)
	{
		Spot spot = ticket.spot;
		ticket.ExitTime = DateTime.UtcNow;
		double price = _parkingPricing.GetParkingPrice(ticket);
		Console.WriteLine($"Please pay Rs.{price}");
		spot.UnreserveSpot();
		_tickets.Remove(ticket.Id, out _);
		return price;
	}
}

class ParkingLotStrategy
{
	public static void Main(string[] args)
	{
		ParkingLot parkingLot = ParkingLot.GetInstance();
	}
}