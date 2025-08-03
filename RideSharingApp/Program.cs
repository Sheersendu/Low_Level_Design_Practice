using System.Collections.Concurrent;

class BaseClass
{
	public Guid Id { get; init; } = Guid.NewGuid();
	public DateTime CreatedTimeStamp { get; init; } = DateTime.UtcNow;
}

class Location (double latitude, double longitude) : BaseClass
{
	public double Latitude { get; set; } = latitude;
	public double Longitude { get; set; } = longitude;
}

class User(string name, string email, string phoneNumber) : BaseClass
{
	public string Name { get; set; } = name;
	public string Email { get; set; } = email;
	public string PhoneNumber { get; set; } = phoneNumber;
}

class Rider(string name, string email, string phoneNumber) : User(name, email, phoneNumber)
{
	private ConcurrentBag<Ride> _rideHistory = new();

	public void AddRide(Ride ride)
	{
		_rideHistory.Add(ride);
	}
}

enum DriverStatus
{
	Offline, InRide, Available
}

class Driver(string name, string email, string phoneNumber, string licenceNumber, Location currentLocation) : User(name, email, phoneNumber)
{
	public string LicenceNumber { get; set; } = licenceNumber;
	public Location CurrentLocation { get; set; } = currentLocation;
	public DriverStatus Status { get; set; } = DriverStatus.Available;
	private ConcurrentBag<Ride> _rideHistory = new();

	public void AddRide(Ride ride)
	{
		_rideHistory.Add(ride);
	}
}

enum RideStatus
{
	Requested, Ongoing, Completed, Cancelled
}

enum RideType
{
	Mini, Premium 
}

class Ride(Rider rider, Location pickup, Location drop, RideType rideType) : BaseClass
{
	public Rider CurrentRider { get; init; } = rider;
	public Location Pickup { get; init; } = pickup;
	public Location Drop { get; init; } = drop;
	public Driver? CurrentDriver { get; set; }
	public double Fare { get; set; } = 0.0;
	public RideType Type { get; set; } = rideType;
	public RideStatus Status { get; set; } = RideStatus.Requested;
}

class RideBookingService
{
	private static RideBookingService _instance;
	private readonly ConcurrentQueue<Ride> _requestedRides = new();
	private readonly ConcurrentDictionary<Guid, Ride> _rides = new();
	private static readonly object RideBooking = new();
	private readonly ConcurrentBag<Driver> _drivers = new();
	
	private RideBookingService(){}

	public static RideBookingService GetInstance()
	{
		if (_instance is null)
		{
			lock (RideBooking)
			{
				if (_instance is null)
				{
					_instance = new RideBookingService();
				}
			}
		}
		return _instance;
	}

	public Ride RequestRide(Rider rider, Location pickup, Location drop, RideType rideType)
	{
		Ride newRide = new(rider, pickup, drop, rideType);
		_rides.TryAdd(newRide.Id, newRide);
		_requestedRides.Enqueue(newRide);
		Console.WriteLine($"Your ride is {newRide.Status}");
		return newRide;
	}

	public Ride GetRide(Guid rideId)
	{
		_rides.TryGetValue(rideId, out var ride);
		return ride;
	}

	public void AddDriver(string name, string email, string phoneNumber, string licenceNumber, double latitude, double longitude)
	{
		Location driverLocation = new(latitude, longitude);
		Driver newDriver = new(name, email, phoneNumber, licenceNumber, driverLocation);
		_drivers.Add(newDriver);
	}

	public Driver GetAvailableDriver()
	{
		lock (RideBooking)
		{
			foreach (Driver driver in _drivers)
			{
				if (driver.Status == DriverStatus.Available)
				{
					return driver;
				}
			}
		}
		Console.WriteLine("No available driver!");
		return null;
	}

	public void AcceptRide(Driver driver, Ride ride)
	{
		lock (RideBooking)
		{
			ride.CurrentDriver = driver;
			driver.Status = DriverStatus.InRide;
			ride.Status = RideStatus.Ongoing;
			Console.WriteLine($"Your ride is {ride.Status} with {ride.CurrentDriver.Name}");
			Console.WriteLine($"Driver Status: {ride.CurrentDriver.Status}");
		}
	}

	public void CancelRide(Ride ride)
	{
		lock (RideBooking)
		{
			if (!(ride.CurrentDriver is null))
			{
				Driver driver = ride.CurrentDriver;
				driver.Status = DriverStatus.Available;
				ride.CurrentDriver = null;
				Console.WriteLine($"Driver Status: {driver.Status}");
			}
			ride.Status = RideStatus.Cancelled;
			Console.WriteLine($"Your ride is {ride.Status}");
		}
	}

	public void CompleteRide(Ride ride)
	{
		lock (RideBooking)
		{
			if (ride.CurrentDriver != null)
			{
				ride.CurrentDriver.Status = DriverStatus.Available;
			}

			ride.Status = RideStatus.Completed;
			ride.Fare = CalculateFare(ride.Type);
			Console.WriteLine($"Your ride is {ride.Status}. Please pay Rs.{ride.Fare}");
			Console.WriteLine($"Driver Status: {ride.CurrentDriver.Status}");
		}
	}

	private double CalculateFare(RideType type)
	{
		switch (type)
		{
			case RideType.Mini:
				return 100;
			case RideType.Premium:
				return 150;
			default:
				return 80;
		}
	}
	
}

class RideSharingApp
{
	public static void Main(string[] args)
	{
		Console.WriteLine("Welcome to Ride Booking Service");
		RideBookingService rideBookingService = RideBookingService.GetInstance();
		Rider rider1 = new("Rider 1", "rider1@email.com", "123");
		Location rider1Pickup = new(22.1, 21.2);
		Location rider1Drop = new(104.1, 71.2);
		rideBookingService.AddDriver("Driver 1", "driver1@email.com", "987", "ACd12X67890", 10.23, 75.41);
		
		#region Ride Complete Flow
		
		// Ride requestedRide = rideBookingService.RequestRide(rider1, rider1Pickup, rider1Drop, RideType.Premium);
		// Driver availableDriver = rideBookingService.GetAvailableDriver();
		// rideBookingService.AcceptRide(availableDriver, requestedRide);
		// rideBookingService.CompleteRide(requestedRide);
		
		#endregion
		
		#region Ride Cancelled Flow
		
		// Ride requestedRide = rideBookingService.RequestRide(rider1, rider1Pickup, rider1Drop, RideType.Premium);
		// Driver availableDriver = rideBookingService.GetAvailableDriver();
		// rideBookingService.AcceptRide(availableDriver, requestedRide);
		// rideBookingService.CancelRide(requestedRide);
		
		#endregion
		
		#region Ride Not Available Flow
		
		Ride requestedRide = rideBookingService.RequestRide(rider1, rider1Pickup, rider1Drop, RideType.Premium);
		Driver availableDriver = rideBookingService.GetAvailableDriver();
		rideBookingService.AcceptRide(availableDriver, requestedRide);
		Ride requestedRide2 = rideBookingService.RequestRide(rider1, rider1Pickup, rider1Drop, RideType.Mini);
		rideBookingService.GetAvailableDriver();
		
		#endregion

	}
}