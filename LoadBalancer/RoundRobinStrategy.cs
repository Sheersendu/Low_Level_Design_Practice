namespace LoadBalancer;

public class RoundRobinStrategy : ILoadBalancerStrategy
{
	private readonly List<Server> _serverList;
	private int _totalServers;
	private int _currentServerIndex = -1; 

	public RoundRobinStrategy(List<Server> serverList)
	{
		_serverList = serverList;
	}
	
	public void ProcessRequests()
	{
		if (_serverList.Count == 0)
		{
			Console.WriteLine("No healthy servers available to process the request.");
			return;
		}

		_currentServerIndex = Interlocked.Increment(ref _currentServerIndex);
		_totalServers = _serverList.Count;
		_currentServerIndex %= _totalServers;
		Server server = _serverList[_currentServerIndex];
		if (server.Status)
			server.Process();
	}
}