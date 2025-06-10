namespace Design_Patterns.Factory;

public class Car : IVehicle
{
	public void Drive()
	{
		Console.WriteLine("I am driving a car!");
	}
}