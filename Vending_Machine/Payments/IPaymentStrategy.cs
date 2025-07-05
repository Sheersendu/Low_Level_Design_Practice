namespace Vending_Machine.Payments;

public interface IPaymentStrategy
{
	void ProcessPayment(List<(Enum payment, int count)> payments);
	bool ProcessChange(int changeAmount);
}