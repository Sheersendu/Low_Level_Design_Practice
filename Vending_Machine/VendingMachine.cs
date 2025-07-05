using Vending_Machine.Utilities;

namespace Vending_Machine;

public class VendingMachine
{
	private readonly VendingMachineContext _vendingMachineContext;
	private readonly Inventory _inventory;
	
	public VendingMachine()
	{
		_vendingMachineContext = new ();
		_vendingMachineContext.UpdateCoinList();
		_vendingMachineContext.UpdateNoteList();
		_inventory = _vendingMachineContext.GetInventory();
	}
	
	public void Init()
	{
		_vendingMachineContext.GetCurrentState();
		bool continueFlow = false;
		Console.WriteLine("Please choose operation:\n1. Choose Product\n2. Restock Item\n");
		var operation = Console.ReadLine();
		if (int.TryParse(operation, out int number) && number < 3)
		{
			continueFlow = true;
		}
		else
		{
			Console.WriteLine("Invalid input. Please enter a valid integer.");
		}

		while (continueFlow)
		{
			continueFlow = _vendingMachineContext.GetCurrentState().Process();
		}
		
		_vendingMachineContext.SetDefaultState();
	}

	public void RestockItem(Item item, int quantity)
	{
		_inventory.RestockItem(item,quantity);
	}
}