using System.Collections.Concurrent;
using Design_Patterns.Strategy;
using Vending_Machine.Enums;
using Vending_Machine.Utilities;

namespace Vending_Machine;

public class VendingMachineContext
{
	private readonly Inventory _inventory;
	private IState _currentState;
	private readonly ConcurrentDictionary<Coin, int> _coinList;
	private readonly ConcurrentDictionary<Note, int> _noteList;

	public VendingMachineContext()
	{
		_inventory = new Inventory(3);
		_coinList = new ConcurrentDictionary<Coin, int>();
		_noteList = new ConcurrentDictionary<Note, int>();
		_currentState = new IdleState(this);
	}

	public void UpdateCoinList()
	{
		_coinList.TryAdd(Coin.One, 10);
		_coinList.TryAdd(Coin.Five, 10);
		_coinList.TryAdd(Coin.Ten, 10);
	}
	
	public void UpdateNoteList()
	{
		_noteList.TryAdd(Note.Ten, 10);
		_noteList.TryAdd(Note.Twenty, 10);
		_noteList.TryAdd(Note.Fifty, 10);
		_noteList.TryAdd(Note.Hundred, 10);
	}

	public Inventory GetInventory()
	{
		return _inventory;
	}

	public void SetDefaultState()
	{
		_currentState = new IdleState(this);
	}
	
	public void SetState(IState state)
	{
		_currentState = state;
	}

	public IState GetCurrentState()
	{
		return _currentState;
	}
	
	public bool ItemInStock(string itemName, int quantity)
	{
		return _inventory.HasItem(itemName, quantity);
	}

	public List<string> GetAllItems()
	{
		return _inventory.GetAllItems();
	}

	public Item GetItem(string itemName)
	{
		return _inventory.GetItem(itemName);
	}

	public bool RemoveItem(Item item, int quantity)
	{
		return _inventory.RemoveItem(item, quantity);
	}

	public bool RestockItem(Item item, int quantity)
	{
		return _inventory.RestockItem(item, quantity);
	}
	
	public bool AddCoin(Coin coin, int quantity)
	{
		_coinList.AddOrUpdate(coin, 0, (key, count) => count + quantity);
		return true;
	}
	
	public bool AddNote(Note note, int quantity)
	{
		_noteList.AddOrUpdate(note, 0, (key, count) => count + quantity);
		return true;
	}
	
	public bool RemoveCoin(Coin coin, int quantity)
	{
		_coinList.AddOrUpdate(coin, 0, (key, count) => count - quantity);
		return true;
	}
	
	public bool RemoveNote(Note note, int quantity)
	{
		_noteList.AddOrUpdate(note, 0, (key, count) => count - quantity);
		return true;
	}

	public List<Coin> GetCoinList()
	{
		return _coinList.Keys.ToList();
	}
	
	public List<Note> GetNoteList()
	{
		return _noteList.Keys.ToList();
	}
}