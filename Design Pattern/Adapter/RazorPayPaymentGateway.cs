namespace Design_Patterns.Adapter;

public class RazorPayPaymentGateway : PaymentGateway
{
	private RazorPayApi razorPayApi = new RazorPayApi();
	public void pay(string orderId, double amount)
	{
		String paymentMessage = razorPayApi.MakePayment(orderId, amount);
		Console.WriteLine(paymentMessage);
	}
}