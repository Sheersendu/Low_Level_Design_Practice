using Vending_Machine;
using Vending_Machine.Enums;
using Vending_Machine.Payments;

namespace Design_Patterns.Strategy;

public class PaymentState : IState
{
	private readonly VendingMachineContext _context;
	private readonly PaymentService _paymentService;

	public PaymentState(VendingMachineContext context)
	{
		Console.WriteLine("-------------------- Payment --------------------\n\n");
		_context = context;
		_paymentService = new PaymentService(_context);
	}
	
	public bool Process()
	{
		Console.WriteLine("Enter payment mode\n1. Coin\n2. Note");
		var paymentMode = Console.ReadLine();
		// int.TryParse(paymentMode, out var paymentType);
		if (int.Parse(paymentMode) == 2)
		{
			_paymentService.SetPaymentStrategy(new NotePaymentStrategy());
		}
		else
		{
			foreach (Coin coin in _context.GetCoinList())
			{
				Console.WriteLine(coin);
			}
		}
		// return _paymentService.ProcessPayment(paymentType);
		return true;
	}
}