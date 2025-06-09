namespace Design_Patterns.Strategy;

public class CreditCardPayment : IPaymentStrategy
{
	public void Pay(decimal amount)
	{
		Console.WriteLine($"Processing credit card payment of {amount}");
	}
}