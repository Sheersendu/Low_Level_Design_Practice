namespace Design_Patterns.Decorator;

public class WhippedCreamAddOn(Coffee coffee) : AddOn(coffee)
{
	public override double getPrice()
	{
		return 15.70 + coffee.getPrice();
	}
}