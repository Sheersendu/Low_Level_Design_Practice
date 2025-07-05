using Vending_Machine;

namespace Design_Patterns.Strategy;

public class SelectProductState : IState
{
	private readonly VendingMachineContext _context;
	public SelectProductState(VendingMachineContext context)
	{
		Console.WriteLine("-------------------- Select Product --------------------\n\n");
		_context = context;
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

		if (_context.ItemInStock(selectedProduct, quantity))
		{
			_context.SetState(new PaymentState(_context));	
		}
		else
		{
			return false;
		}

		return true;
	}
}