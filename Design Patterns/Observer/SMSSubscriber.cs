namespace Design_Patterns.Observer;

public class SMSSubscriber : ISubscriber
{
	public void update(string message)
	{
		Console.WriteLine($"SMS received: {message}");
	}
}