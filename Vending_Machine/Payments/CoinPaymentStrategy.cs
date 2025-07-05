using Vending_Machine.Enums;

namespace Vending_Machine.Payments;

public class CoinPaymentStrategy : IPaymentStrategy
{
	private readonly VendingMachineContext _context;
	
	public CoinPaymentStrategy(VendingMachineContext context)
	{
		_context = context;
	}
	public void ProcessPayment(List<(Enum payment, int count)> payments)
	{
		foreach(var (payment, count) in payments)
		{ 
			_context.AddCoin((Coin) payment, count);
		}
		
	}
	public bool ProcessChange(int changeAmount)
	{
		List<(Coin coin, int count)> possibleChange = new();
		foreach (Coin coin in _context.GetCoinList().OrderByDescending(c => (int)c))
		{
			if (changeAmount > 0)
			{
				int coinQuantity = (int) Math.Floor((double)(changeAmount/(int)coin));
				Console.WriteLine($"{coin} : {coinQuantity}");
				if (coinQuantity > 0)
				{
					changeAmount -= (int)coin*coinQuantity;
					possibleChange.Add((coin, coinQuantity));
				}
			}
		}

		if (changeAmount > 0)
		{
			return false;
		}

		foreach ((Coin coin, int count) in possibleChange)
		{
			_context.RemoveCoin(coin, count);
		}

		return true;
	}
}