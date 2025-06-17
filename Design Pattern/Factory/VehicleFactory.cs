namespace Design_Patterns.Factory;

public static class VehicleFactory
{
	public static IVehicle GetVehicle(string vehicleType)
	{
		return vehicleType.ToLower() switch
		{
			"car" => new Car(),
			"bike" => new Bike(),
			_ => throw new ArgumentException("Invalid vehicle type")
		};
	}
}