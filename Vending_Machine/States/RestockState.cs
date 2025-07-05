using Vending_Machine;
using Vending_Machine.Utilities;

namespace Design_Patterns.Strategy;

public class RestockState : IState
{
	private readonly VendingMachineContext _context;

	public RestockState(VendingMachineContext vendingMachineContext)
	{
		_context = vendingMachineContext;
	}
	
	public bool Process()
	{
		var itemList = _context.GetAllItems();
		for(int index = 0; index < itemList.Count; index++)
		{
			Console.WriteLine($"{index+1}. {itemList[index]}");
		}
		var selectedProduct = Console.ReadLine();
		if (string.IsNullOrEmpty(selectedProduct))
		{
			Console.WriteLine("Select a valid product!");
			return false;
		}

		Console.WriteLine("Input quantity:\n");
		var inputQuantity = Console.ReadLine();
		int.TryParse(inputQuantity, out var quantity);
		
		Item item = _context.GetItem(selectedProduct);
		_context.RestockItem(item, quantity);
		_context.GetInventory().ListInventory();
		
		return false;
	}
}