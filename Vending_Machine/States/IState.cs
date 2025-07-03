using Vending_Machine;

namespace Design_Patterns.Strategy;

public interface IState
{
	bool Process(VendingMachineContext context);
}