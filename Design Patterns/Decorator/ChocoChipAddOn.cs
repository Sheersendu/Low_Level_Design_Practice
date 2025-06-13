namespace Design_Patterns.Decorator;

public class ChocoChipAddOn(Coffee coffee) : AddOn(coffee)
{
	public override double getPrice()
	{
		return 10.50 + coffee.getPrice();
	}
}