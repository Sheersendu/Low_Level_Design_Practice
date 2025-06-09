namespace Design_Patterns.Strategy;

public class DebitCardPayment : IPaymentStrategy
{
	public void Pay(decimal amount)
	{
		Console.WriteLine($"Processing debit card payment of {amount}");
	}
}