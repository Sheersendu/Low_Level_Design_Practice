namespace Design_Patterns.Observer;

public class EmailSubscriber : ISubscriber
{
	public void update(string message)
	{
		Console.WriteLine($"Email received: {message}");
	}
}