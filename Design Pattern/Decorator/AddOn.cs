namespace Design_Patterns.Decorator;

public abstract class AddOn(Coffee coffee) : Coffee
{
	public Coffee _coffee = coffee;
	public abstract double getPrice();
}