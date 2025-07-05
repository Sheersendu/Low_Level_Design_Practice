using Vending_Machine.Enums;

namespace Vending_Machine.Payments;

public class NotePaymentStrategy : IPaymentStrategy
{
	public bool ProcessPayment(List<(Enum payment, int count)> payments, VendingMachineContext context)
	{
		foreach(var (payment, count) in payments)
		{ 
			context.AddNote((Note) payment, count);
		}

		return true;
	}
}