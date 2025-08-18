using System.Collections.Concurrent;

enum Coin
{
	One = 1,
	Two = 2,
	Five = 5,
	Ten = 10
}

class Item(string code, string name, int price)
{
	public string Code { get; init; } = code;
	public string Name { get; set; } = name;
	public int Price { get; set; } = price;
}

class Inventory
{
	private ConcurrentDictionary<string, Item> _itemList = new();
	private ConcurrentDictionary<string, int> _itemQuantity = new();

	public void AddItem(Item newItem, int quantity)
	{
		_itemList.TryAdd(newItem.Code, newItem);
		_itemQuantity.TryAdd(newItem.Code, quantity);
	}

	public Item GetItem(string itemCode)
	{
		try
		{
			return _itemList[itemCode];
		}
		catch (Exception e)
		{
			Console.WriteLine("Item doesn't exists!");
		}

		return null;
	}

	public bool IsItemAvailable(string itemCode)
	{
		return _itemQuantity.GetValueOrDefault(itemCode, 0) > 0;
	}
}

class VendingMachine
{
	private VendingMachineState _state { get; set; }
	private Inventory _inventory { get; } = new();
	private int TotalBalance { get; set; }

	public VendingMachine()
	{
		_state = new IdleState(this);
	}

	public void SetState(VendingMachineState newState)
	{
		_state = newState;
	}

	public VendingMachineState GetState()
	{
		return _state;
	}

	public void AddItem(string itemCode, string name, int price, int quantity)
	{
		Item newItem = new(itemCode, name, price);
		_inventory.AddItem(newItem, quantity);
	}

	public Item GetItem(string itemCode)
	{
		return _inventory.GetItem(itemCode);
	}

	public bool IsItemAvailable(string itemCode)
	{
		return _inventory.IsItemAvailable(itemCode);
	}

	public void AddBalance(int balance)
	{
		TotalBalance += balance;
	}
	
	public int GetBalance()
	{
		return TotalBalance;
	}
}


abstract class VendingMachineState(VendingMachine machine)
{
	protected VendingMachine machine = machine;
	
	public abstract void Idle();
	public abstract void SelectItem(string itemCode);
	public abstract void InsertCoin(List<Coin> coins);
	public abstract void Dispense();
	public abstract void Refund();
}

class IdleState(VendingMachine machine) : VendingMachineState(machine)
{
	public override void Idle()
	{
		Console.WriteLine("Welcome to Vending Machine!");
		machine.SetState(new SelectItemState(machine));
	}
	public override void SelectItem(string itemCode)
	{
		Console.WriteLine("Please Select an Item");
	}
	public override void InsertCoin(List<Coin> coins)
	{
		Console.WriteLine("Please Select a item before inserting a coin!");
	}
	public override void Dispense()
	{
		Console.WriteLine("Nothing to dispense!");
	}
	public override void Refund()
	{
		Console.WriteLine("Nothing to refund!");
	}
}

class InsertCoinState(VendingMachine machine, string itemCode) : VendingMachineState(machine)
{
	private int totalAmount;
	
	public override void Idle()
	{
		Console.WriteLine("Insert coin to complete purchase!");
	}
	public override void SelectItem(string itemCode)
	{
		Console.WriteLine("Insert coin to complete purchase!");
	}
	public override void InsertCoin(List<Coin> coins)
	{
		totalAmount = coins.Sum(coin => (int)coin);
		int selectedItemPrice = machine.GetItem(itemCode).Price;
		if (totalAmount < selectedItemPrice)
		{
			Console.WriteLine("Insufficient amount!");
		}
		else
		{
			machine.AddBalance(totalAmount);
			
		}

		int refundAmount = selectedItemPrice > totalAmount ? totalAmount : (totalAmount - selectedItemPrice);
		machine.SetState(new DispenseState(machine, itemCode, refundAmount));
	}
	public override void Dispense()
	{
		Console.WriteLine("Complete payment to dispense product!");
	}
	public override void Refund()
	{
		Console.WriteLine("Payment incomplete!");
	}
}

class SelectItemState(VendingMachine machine) : VendingMachineState(machine)
{
	public override void Idle()
	{
		Console.WriteLine("Please Select an Item");
	}
	public override void SelectItem(string itemCode)
	{
		if (machine.IsItemAvailable(itemCode))
		{
			machine.SetState(new InsertCoinState(machine, itemCode));
		}
		else
		{
			Console.WriteLine("Item unavailable!");
			machine.SetState(new DispenseState(machine, itemCode, -1));
		}
	}
	public override void InsertCoin(List<Coin> coins)
	{
		Console.WriteLine("Please Select an Item");
	}
	public override void Dispense()
	{
		Console.WriteLine("Please Select an Item");
	}
	public override void Refund()
	{
		Console.WriteLine("Please Select an Item");
	}
}

class DispenseState(VendingMachine machine, string itemCode, int refundAmount) : VendingMachineState(machine)
{
	public override void Idle()
	{
		Console.WriteLine("Dispensing Item");
	}
	public override void SelectItem(string itemCode)
	{
		Console.WriteLine("Dispensing Item");
	}
	public override void InsertCoin(List<Coin> coins)
	{
		Console.WriteLine("Dispensing Item");
	}
	public override void Dispense()
	{
		if (refundAmount >= 0)
		{
			Item item = machine.GetItem(itemCode);
			Console.WriteLine($"Dispensing Item : {item.Name}");
		}

		machine.SetState(new RefundState(machine, refundAmount));
	}
	public override void Refund()
	{
		Console.WriteLine("Dispensing Item");
	}
}

class RefundState(VendingMachine machine, int refundAmount) : VendingMachineState(machine)
{
	public override void Idle()
	{
		Console.WriteLine("Refund Processing...");
	}
	public override void SelectItem(string itemCode)
	{
		Console.WriteLine("Refund Processing...");
	}
	public override void InsertCoin(List<Coin> coins)
	{
		Console.WriteLine("Refund Processing...");
	}
	public override void Dispense()
	{
		Console.WriteLine("Refund Processing...");
	}
	public override void Refund()
	{
		if (refundAmount > 0)
		{
			Console.WriteLine($"Processed refund of {refundAmount}");
		}
		machine.SetState(new IdleState(machine));
	}
}

class VendingMachineDemo
{
	public static void Main(string[] args)
	{
		VendingMachine machine = new ();
		machine.AddItem("A1", "Coke", 25, 1);
		machine.AddItem("A2", "Lays", 20, 5);
		
		machine.GetState().Idle();
		machine.GetState().SelectItem("A1");

		List<Coin> coins = new List<Coin>{ Coin.Ten, Coin.Ten, Coin.Five };
		machine.GetState().InsertCoin(coins);
		machine.GetState().Dispense();
		machine.GetState().Refund();
	}
}