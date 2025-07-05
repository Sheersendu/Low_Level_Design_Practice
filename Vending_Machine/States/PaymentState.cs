using Vending_Machine;
using Vending_Machine.Enums;
using Vending_Machine.Payments;
using Vending_Machine.Utilities;

namespace Design_Patterns.Strategy;

public class PaymentState : IState
{
	private readonly VendingMachineContext _context;
	private readonly PaymentService _paymentService;
	private readonly Item _item;
	private readonly int _quantity;

	public PaymentState(VendingMachineContext context, Item item, int quantity)
	{
		Console.WriteLine("-------------------- Payment --------------------\n\n");
		_context = context;
		_paymentService = new PaymentService(_context);
		_item = item;
		_quantity = quantity;
	}
	
	public bool Process()
	{
		Console.WriteLine("Enter payment mode\n1. Coin\n2. Note");
		var paymentMode = Console.ReadLine();
		var payments = new List<(Enum paymentType, int count)>();
		
		if (int.Parse(paymentMode) == 2)
		{
			_paymentService.SetPaymentStrategy(new NotePaymentStrategy(_context));
			foreach (Note note in _context.GetNoteList())
			{
				Console.WriteLine($"Input no of {note} note: ");
				var noteInput = Console.ReadLine();
				payments.Add((note, int.Parse(noteInput)));
			}
		}
		else
		{
			foreach (Coin coin in _context.GetCoinList())
			{
				Console.WriteLine($"Input no of {coin} coin: ");
				var coinInput = Console.ReadLine();
				payments.Add((coin, int.Parse(coinInput)));
			}
		}
		
		var totalAmount = _item.Price*_quantity;
		var changeAmount = _paymentService.ProcessPayment(payments, totalAmount);
		if (changeAmount == -1)
		{
			return false;
		}

		_context.SetState(new DispatchState(_context, _item, _quantity, changeAmount));
		return true;

	}
}