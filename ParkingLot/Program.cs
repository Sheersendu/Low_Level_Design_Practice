enum VehicleType
{
	CAR,
	BIKE
}

abstract class Vehicle
{
	public string licenceNumber { get; }
	public VehicleType type { get; }
	public Vehicle(string vehicleLicenceNumber, VehicleType vehicleType)
	{
		licenceNumber = vehicleLicenceNumber;
		type = vehicleType;
	}

}

class Car(string licenceNumber) : Vehicle(licenceNumber, VehicleType.CAR);
class Bike(string licenceNumber) : Vehicle(licenceNumber, VehicleType.BIKE);

class ParkingSpot
{
	public int _spotNumber { get; }
	public VehicleType _vehicleType { get; }
	public bool _isAvailable;
	public Vehicle _parkedVehicle { get; set; }

	public ParkingSpot(int spotNumber, VehicleType vehicleType)
	{
		_spotNumber = spotNumber;
		_vehicleType = vehicleType;
		_isAvailable = true;
	}

	public void Reserve(Vehicle currentVehicle)
	{
		lock (this)
		{
			if !(_isAvailable)
			{
				Console.WriteLine("Spot is already reserved!");
			}
			else
			{
				_isAvailable = false;
				_parkedVehicle = currentVehicle;
			}
		}
	}

	public void UnReserve()
	{
		lock (this)
		{
			_isAvailable = true;
			_parkedVehicle = null;
		}
	}
}

class Floor
{
	private List<ParkingSpot> _parkingSpots = new();
	public int _floorNumber { get; }

	public Floor(int floorNumber)
	{
		_floorNumber = floorNumber;
		for (int index = 0; index < 5; index++)
		{
			_parkingSpots.Add(new ParkingSpot(index+1, VehicleType.CAR));
		}
		for (int index = 5; index < 10; index++)
		{
			_parkingSpots.Add(new ParkingSpot(index+1, VehicleType.BIKE));
		}
	}

	public List<ParkingSpot> GetAvailableSpots(VehicleType type)
	{
		lock (this)
		{
			return _parkingSpots.Where(spot => spot._vehicleType.Equals(type) && spot._isAvailable).ToList();
		}
	}

}


class ParkingLot
{
	private PriorityQueue<ParkingSpot, (int floorNumber, int parkingSpotNumber)> parkingSpots;
	private List<Floor> floorList = new();
	private readonly object _lock = new();
	
	public ParkingLot(int totalFloors)
	{
		for (int index = 0; index < totalFloors; index++)
		{
			Floor floor = new (index + 1);
			floorList.Add(floor);
		}
	}

	public ParkingSpot GetParkingSpot(VehicleType type)
	{
		lock (_lock)
		{
			parkingSpots = new();
			foreach (Floor floor in floorList)
			{
				var availableSpots = floor.GetAvailableSpots(type);
				foreach (ParkingSpot spot in availableSpots)
				{
					parkingSpots.Enqueue(spot, (floor._floorNumber, spot._spotNumber));
				}
			}
			
			if (parkingSpots.Count == 0)
			{
				Console.WriteLine("No Slots available!");
				return null;
			}

			return parkingSpots.Dequeue();
		}
	}
}

class Program
{
	public static void Main(string[] args)
	{
		ParkingLot parkingLot = new(2);
		for(int index = 0; index < 6; index++)
		{
			Car car = new($"C-{index + 1}");
			ParkingSpot? spot = parkingLot.GetParkingSpot(VehicleType.CAR);
			if (spot != null)
			{
				spot.Reserve(car);
				Console.WriteLine($"A vehicle of type '{spot._vehicleType}' with licence number: '{spot._parkedVehicle.licenceNumber}' is parked at spot no: '{spot._spotNumber}'");
				// spot.UnReserve();
			}
		}
		
		for(int index = 0; index < 3; index++)
		{
			Bike bike = new($"B-{index + 1}");
			ParkingSpot? spot = parkingLot.GetParkingSpot(VehicleType.BIKE);
			if (spot != null)
			{
				spot.Reserve(bike);
				Console.WriteLine($"A vehicle of type '{spot._vehicleType}' with licence number: '{spot._parkedVehicle.licenceNumber}' is parked at spot no: '{spot._spotNumber}'");
			}
		}
	}
}

