using Vending_Machine.Enums;

namespace Vending_Machine.Payments;

public class CoinPaymentStrategy : IPaymentStrategy
{
	public bool ProcessPayment(List<(Enum payment, int count)> payments, VendingMachineContext context)
	{
		foreach(var (payment, count) in payments)
		{ 
			context.AddCoin((Coin) payment, count);
		}

		return true;
	}
}