using System.Collections.Concurrent;

namespace Vending_Machine.Utilities;

public class Inventory
{
	private readonly ConcurrentDictionary<Item, int> _inventory;
	private readonly int _totalItems;
	
	public Inventory(int totalItems)
	{
		_totalItems = totalItems;
		_inventory = new ConcurrentDictionary<Item, int>();
	}

	public bool RestockItem(Item item, int quantity)
	{
		if (_inventory.Count >= _totalItems)
		{
			Console.WriteLine("Inventory already full!");
			return false;
		}
		
		_inventory.AddOrUpdate(item, quantity, (key, oldValue) => oldValue + quantity);
		return true;
	}

	public bool HasItem(Item item)
	{
		return _inventory.ContainsKey(item) && (_inventory.GetValueOrDefault(item, 0) > 0);
	}
}