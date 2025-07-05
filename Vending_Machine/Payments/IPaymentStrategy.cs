namespace Vending_Machine.Payments;

public interface IPaymentStrategy
{
	bool ProcessPayment(List<(Enum payment, int count)> payments, VendingMachineContext context);
}