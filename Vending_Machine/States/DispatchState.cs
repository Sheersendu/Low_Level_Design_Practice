using Vending_Machine;
using Vending_Machine.Utilities;

namespace Design_Patterns.Strategy;

public class DispatchState : IState
{
	private readonly VendingMachineContext _context;
	private readonly Item _item;
	private readonly int _quantity;
	private readonly int _changeAmount;
	
	public DispatchState(VendingMachineContext context, Item item, int quantity, int changeAmount)
	{
		Console.WriteLine("-------------------- Product Dispatch --------------------\n\n");
		_context = context;
		_item = item;
		_quantity = quantity;
		_changeAmount = changeAmount;
	}
	public bool Process()
	{
		_context.RemoveItem(_item, _quantity);
		Console.WriteLine($"{_quantity} X {_item.Name} Dispatched!");
		Console.WriteLine($"Change returned: {_changeAmount}");
		Console.WriteLine("\n\n");
		// _context.SetState(new IdleState(_context));
		return false;
	}
}