using System.Collections.Concurrent;
using System.Threading.Channels;

class BaseClass
{
	public Guid Id { get; init; } = Guid.NewGuid();
	public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

public class Location(double longitude, double latitude )
{
	public double Latitude { get; set; } = latitude;
	public double Longitude { get; set; } = longitude;
}

class Rider(string name, string phoneNumber) : BaseClass
{
	public string Name { get; set; } = name;
	public string PhoneNumber { get; set; } = phoneNumber;
	public Location CurrentLocation { get; set; }
}

enum DriverStatus
{
	Available, Offline, InRide
}


class Driver(string name, string licence) : BaseClass
{
	public string Name { get; set; } = name;
	public string Licence { get; set; } = licence;
	public DriverStatus Status { get; set; } = DriverStatus.Available;
	private Object Lock = new();

	public void UpdateStatus(DriverStatus newStatus)
	{
		lock (Lock)
		{
			Status = newStatus;
		}
	}
}

enum RideStatus
{
	Scheduled, InProgress, Cancelled
}

class Ride(Rider rider, Location pickup, Location drop) : BaseClass
{
	public Rider CurrentRider { get; set; } = rider;
	public Driver? CurrentDriver { get; set; }
	public Location Pickup { get; set; } = pickup;
	public Location Drop { get; set; } = drop;
	public double? Fare { get; set; }
	public RideStatus Status { get; set; } = RideStatus.Scheduled;
	private Object Lock = new();

	public void UpdateStatus(RideStatus newStatus)
	{
		lock (Lock)
		{
			Status = newStatus;
		}
	}

	public void UpdateDriver(Driver driver)
	{
		lock (Lock)
		{
			CurrentDriver = driver;
		}
	}
}

interface IFareStrategy
{
	double CalculateFare(Ride ride);
}

class BaseStrategy : IFareStrategy
{
	public double CalculateFare(Ride ride)
	{
		return 100;
	}
}

class SurgeStrategy : IFareStrategy
{
	public double CalculateFare(Ride ride)
	{
		return 150;
	}
}


class RideBooking
{
	private static RideBooking Instance;
	private static Object rideBookingInstance = new();
	private static Object rideLock = new();
	private ConcurrentQueue<Ride> ScheduledRides = new();
	private ConcurrentDictionary<Guid, Ride> UserRides = new();
	private ConcurrentBag<Driver> DriverList = new();
	private IFareStrategy strategy = new BaseStrategy();
	
	private RideBooking(){}

	public static RideBooking GetInstance()
	{
		if (Instance is null)
		{
			lock (rideBookingInstance)
			{
				if (Instance is null)
				{
					Instance = new RideBooking();
				}
			}
		}

		return Instance;
	}

	public void SetStrategy(IFareStrategy newStrategy)
	{
		lock (rideBookingInstance)
		{
			strategy = newStrategy;
		}
	}

	public void ScheduleRide(Rider rider, Location pickup, Location drop)
	{
		Ride newRide = new(rider, pickup, drop);
		ScheduledRides.Enqueue(newRide);
		UserRides.TryAdd(rider.Id, newRide);
		NotifyDrivers(newRide);
	}

	public void AddDriver(string name, string licence)
	{
		Driver newDriver = new(name, licence);
		DriverList.Add(newDriver);
	}

	private void NotifyDrivers(Ride ride)
	{
		foreach(Driver driver in DriverList)
		{
			if (driver.Status.Equals(DriverStatus.Available))
			{
				Console.WriteLine($"Notified Driver {driver.Name}");
				AcceptRide(ride, driver);
				break;
			}
		}
	}

	private void AcceptRide(Ride ride, Driver driver)
	{
		lock (rideLock)
		{
			ride.UpdateStatus(RideStatus.InProgress);
			ride.UpdateDriver(driver);
			driver.UpdateStatus(DriverStatus.InRide);
		}
	}

	private void CancelRide(Ride ride)
	{
		lock (rideLock)
		{
			if (!(ride.CurrentDriver is null))
			{
				Driver driver = ride.CurrentDriver;
				driver.UpdateStatus(DriverStatus.Available);
				ride.CurrentDriver = null;
			}
			ride.UpdateStatus(RideStatus.Cancelled);
		}
	}
	
}



class UberDemo
{
	public static void Main(string[] args)
	{
		Console.WriteLine("Book your ride now!");
		RideBooking booking = RideBooking.GetInstance();
		Rider rider = new("A", "1");
		booking.AddDriver("B", "L1");
		booking.ScheduleRide(rider, new Location(12.1, 13.5), new Location(23.5, 21.6));
		
	}
}