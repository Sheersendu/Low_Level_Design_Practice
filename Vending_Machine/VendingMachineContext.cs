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
		_currentState = new IdleState();
		_coinList = new ConcurrentDictionary<Coin, int>();
		_noteList = new ConcurrentDictionary<Note, int>();
	}

	public void UpdateCoinList()
	{
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

	public void SetState(IState state)
	{
		_currentState = state;
	}

	public IState GetCurrentState()
	{
		return _currentState;
	}
}