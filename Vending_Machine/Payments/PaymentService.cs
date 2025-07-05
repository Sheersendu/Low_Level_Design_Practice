using Vending_Machine.Enums;

namespace Vending_Machine.Payments;

public class PaymentService
{
	private IPaymentStrategy _paymentStrategy;
	private readonly VendingMachineContext _context;
	
	public PaymentService(VendingMachineContext context)
	{
		_paymentStrategy = new CoinPaymentStrategy();
		_context = context;
	}

	public void SetPaymentStrategy(IPaymentStrategy paymentStrategy)
	{
		_paymentStrategy = paymentStrategy;
	}

	public bool ProcessPayment(List<(Enum paymentType, int count)> payments, double totalAmountToBePaid)
	{
		double amountPaid = 0;
		double returnChange;
		totalAmountToBePaid = Math.Ceiling(totalAmountToBePaid);
		foreach (var (payment, typeCount) in payments)
		{
			amountPaid += (GetValue(payment) * typeCount);
		}

		if (totalAmountToBePaid > amountPaid)
		{
			Console.WriteLine("Payment failed! Insufficient amount paid!");
			return false;
		}
		_paymentStrategy.ProcessPayment(payments, _context);
		returnChange = totalAmountToBePaid - amountPaid;
		if (returnChange > 0)
		{
			
		}

		return true;
	}

	private double GetValue(Enum paymentType)
	{
		return paymentType switch
		{
			Coin.One => 1,
			Coin.Five => 5,
			Coin.Ten => 10,
			Note.Ten => 10,
			Note.Twenty => 20,
			Note.Fifty => 50,
			Note.Hundred => 100,
			_ => throw new ArgumentException("Invalid payment type.")
		};
	}
	
}