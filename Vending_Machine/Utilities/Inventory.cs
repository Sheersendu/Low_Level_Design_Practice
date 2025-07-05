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
		if (_inventory.Count > _totalItems)
		{
			Console.WriteLine("Inventory already full!");
			return false;
		}
		
		_inventory.AddOrUpdate(item, quantity, (key, oldValue) => oldValue + quantity);
		return true;
	}

	public bool HasItem(string itemName, int quantity)
	{
		return _inventory.Any(kvp => kvp.Key.Name.Equals(itemName) && kvp.Value >= quantity);
	}
	
	public Item GetItem(string itemName)
	{
		var item = _inventory.Keys.FirstOrDefault(i => i.Name.Equals(itemName));
		return item;
	}

	public List<string> GetAllItems()
	{
		return _inventory.Select(kvp => kvp.Key.Name).ToList();
	}

	public bool RemoveItem(Item item, int quantity)
	{
		_inventory.AddOrUpdate(item, 0, (key, value) => value - quantity);
		return true;
	}

	public void ListInventory()
	{
		foreach((Item item, int count) in _inventory)
		{
			Console.WriteLine($"{count} no of {item.Name} available");
		}
	}
}