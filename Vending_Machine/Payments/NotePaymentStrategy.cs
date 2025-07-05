using Vending_Machine.Enums;

namespace Vending_Machine.Payments;

public class NotePaymentStrategy : IPaymentStrategy
{
	private readonly VendingMachineContext _context;

	public NotePaymentStrategy(VendingMachineContext context)
	{
		_context = context;
	}
	public void ProcessPayment(List<(Enum payment, int count)> payments)
	{
		foreach(var (payment, count) in payments)
		{ 
			_context.AddNote((Note) payment, count);
		}
	}
	public bool ProcessChange(int changeAmount)
	{
		List<(Note note, int count)> possibleChange = new();
		foreach (Note note in _context.GetNoteList().OrderByDescending(c => (int)c))
		{
			if (changeAmount > 0)
			{
				int coinQuantity = (int) Math.Floor((double)(changeAmount/(int)note));
				Console.WriteLine($"{note} : {coinQuantity}");
				if (coinQuantity > 0)
				{
					changeAmount -= (int)note*coinQuantity;
					possibleChange.Add((note, coinQuantity));
				}
			}
		}

		if (changeAmount > 0)
		{
			return false;
		}

		foreach ((Note note, int count) in possibleChange)
		{
			_context.RemoveNote(note, count);
		}

		return true;
	}
}