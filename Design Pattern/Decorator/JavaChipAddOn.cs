namespace Design_Patterns.Decorator;

public class JavaChipAddOn(Coffee coffee): AddOn(coffee)
{
	public override double getPrice()
	{
		return 17.90 + coffee.getPrice();
	}
}