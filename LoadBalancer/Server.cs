namespace LoadBalancer;

public class Server
{
	public readonly Guid Id = Guid.NewGuid();
	public readonly string Name;
	public volatile bool Status = true; //volatile to ensure visibility across threads
	public int _totalConnections = 0;

	public Server(string name)
	{
		Name = name;
	}

	public void SetStatus(bool status)
	{
		Status = status;
	}

	public void AddConnection()
	{
		Interlocked.Increment(ref _totalConnections);
	}

	public void Process()
	{
		Console.WriteLine($"Server {Name} is processing request!");
		AddConnection();
	}
}