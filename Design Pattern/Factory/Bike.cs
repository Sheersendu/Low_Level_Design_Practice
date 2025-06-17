namespace Design_Patterns.Factory;

public class Bike : IVehicle
{
	public void Drive()
	{
		Console.WriteLine("I am riding a bike!");
	}
}