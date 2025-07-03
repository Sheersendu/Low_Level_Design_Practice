using Vending_Machine;

namespace Design_Patterns.Strategy;

public class IdleState : IState
{
	public IdleState()
	{
		Console.WriteLine("-------------------- Welcome to Vending Machine --------------------\n\n");
	}
	
	public bool Process(VendingMachineContext context)
	{
		context.SetState(new SelectProductState());
		return true;
	}
}