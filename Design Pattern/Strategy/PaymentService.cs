namespace Design_Patterns.Strategy;

public class PaymentService
{
	private IPaymentStrategy _paymentStrategy;
	
	public PaymentService(IPaymentStrategy paymentStrategy)
	{
		_paymentStrategy = paymentStrategy;
	}
	
	public void SetPaymentStrategy(IPaymentStrategy paymentStrategy)
	{
		_paymentStrategy = paymentStrategy;
	}

	public void Pay(decimal amount)
	{
		_paymentStrategy.Pay(amount);
	}
}