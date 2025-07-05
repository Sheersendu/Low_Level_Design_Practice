using Vending_Machine;

namespace Design_Patterns.Strategy;

public class IdleState : IState
{
	private readonly VendingMachineContext _context;
	public IdleState(VendingMachineContext context)
	{
		Console.WriteLine("-------------------- Welcome to Vending Machine --------------------\n\n");
		_context = context;
	}
	
	public bool Process()
	{
		_context.SetState(new SelectProductState(_context));
		return true;
	}
}