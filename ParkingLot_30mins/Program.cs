using System.Collections.Concurrent;

enum SpotType { Small, Medium, Large }
class ParkingSpot
{
	private Guid Id;
	private string Number;
	private SpotType Type;
	private Vehicle? vehicle;
	private bool Available = true;
	private object spotLock = new ();

	public void Reserve(Vehicle currentVehicle)
	{
		lock (spotLock)
		{
			if (Available)
			{
				Available = false;
				vehicle = currentVehicle;
			}
			else
			{
				Console.WriteLine("Seat already reserved!");
			}
		}
	}

	public void UnReserve()
	{
		lock (spotLock)
		{
			Available = true;
			vehicle = null;
		}
	}

	public bool IsAvailable()
	{
		lock (spotLock)
		{
			return Available;
		}
	}

	public SpotType GetSpotType()
	{
		return Type;
	}
	
}

class Floor
{
	private List<ParkingSpot> AllSpots = new();
	private object floorLock = new();

	public List<ParkingSpot> GetAvailableSpots(SpotType type)
	{
		lock (floorLock)
		{
			return AllSpots.Where(spot => spot.IsAvailable() && spot.GetType().Equals(type)).ToList();
		}
	}
}

enum VehicleType { Car, Bike, Truck }

abstract class Vehicle
{
	private string LicenceNumber;
	private VehicleType Type;

	public VehicleType GetVehicleType()
	{
		return Type;
	}
}

class Car : Vehicle{}
class Bike : Vehicle{}
class Truck : Vehicle{}

class Ticket(Vehicle vehicle, ParkingSpot spot)
{
	private Guid Id = Guid.NewGuid();
	private Vehicle vehicle = vehicle;
	private ParkingSpot spot = spot;
	private DateTime Entry = DateTime.UtcNow;
	private DateTime Exit;

	public int CalculateDuration()
	{
		return (int)(Exit - Entry).TotalMinutes;
	}

	public VehicleType GetVehicleType()
	{
		return vehicle.GetVehicleType();
	}

	public ParkingSpot GetSpot()
	{
		return spot;
	}

	public void SetExitTime()
	{
		Exit = DateTime.UtcNow;
	}
}

interface IFareStrategy
{
	double GetFare(Ticket ticket);
}

class BaseFareStrategy : IFareStrategy
{
	private ConcurrentDictionary<VehicleType, double> fare = new();

	public BaseFareStrategy()
	{
		fare.TryAdd(VehicleType.Car, 2.500);
		fare.TryAdd(VehicleType.Bike, 2.050);
		fare.TryAdd(VehicleType.Truck, 5.099);
	}
	
	public double GetFare(Ticket ticket)
	{
		return fare.GetValueOrDefault(ticket.GetVehicleType(), 1)*ticket.CalculateDuration();
	}
}

class ParkingLot
{
	private List<Floor> AllFloors;
	private IFareStrategy Strategy = new BaseFareStrategy();

	public List<ParkingSpot> FindAvailableSpots(SpotType type)
	{
		List<ParkingSpot> AllSpots = new();

		foreach (Floor floor in AllFloors)
		{
			AllSpots.AddRange(floor.GetAvailableSpots(type));
		}

		return AllSpots;
	}

	public Ticket Park(ParkingSpot spot, Vehicle vehicle)
	{
		spot.Reserve(vehicle);
		Ticket newTicket = new(vehicle, spot);
		return newTicket;
	}

	public double UnPark(Ticket ticket)
	{
		ParkingSpot spot = ticket.GetSpot();
		spot.UnReserve();
		ticket.SetExitTime();
		return Strategy.GetFare(ticket);
	}
}

