using System.Collections.Concurrent;

namespace Vending_Machine.Utilities;

public class Inventory
{
	private readonly ConcurrentDictionary<string, int> _inventory;
	private readonly int _totalItems;
	
	public Inventory(int totalItems)
	{
		_totalItems = totalItems;
		_inventory = new ConcurrentDictionary<string, int>();
	}

	public bool RestockItem(Item item, int quantity)
	{
		if (_inventory.Count >= _totalItems)
		{
			Console.WriteLine("Inventory already full!");
			return false;
		}
		
		_inventory.AddOrUpdate(item.Name, quantity, (key, oldValue) => oldValue + quantity);
		return true;
	}

	public bool HasItem(string itemName, int quantity)
	{
		return _inventory.ContainsKey(itemName) && (_inventory.GetValueOrDefault(itemName, 0) >= quantity);
	}

	public List<string> GetAllItems()
	{
		return _inventory.Keys.ToList();
	}
}