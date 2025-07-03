using Vending_Machine.Enums;

namespace Vending_Machine.Utilities;

public class Item(string name, double price, ItemType itemType)
{
	public string Name { get; set; } = name;
	public double Price { get; set; } = price;
	public ItemType Type { get; set; } = itemType;
}