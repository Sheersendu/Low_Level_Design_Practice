using Vending_Machine;

namespace Design_Patterns.Strategy;

public class SelectProductState : IState
{
	public SelectProductState()
	{
		Console.WriteLine("-------------------- Select Product --------------------\n\n");
	}
	
	public bool Process(VendingMachineContext context)
	{
		var selectedProduct = Console.ReadLine();
		if (string.IsNullOrEmpty(selectedProduct))
		{
			Console.WriteLine("Product not available");
		}

		return true;
	}
}