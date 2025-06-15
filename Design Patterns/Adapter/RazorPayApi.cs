namespace Design_Patterns.Adapter;

public class RazorPayApi
{
	public string MakePayment(string orderID, double amountToBePaid)
	{
		return $"Payment completed for Order ID: {orderID} of Rs. {amountToBePaid} via RazorPay";
	}
}