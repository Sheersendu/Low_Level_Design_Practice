namespace Design_Patterns.Adapter;

public interface PaymentGateway
{
	void pay(string orderId, double amount);
}