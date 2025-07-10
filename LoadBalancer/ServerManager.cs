namespace LoadBalancer;

public class ServerManager : IDisposable
{
	private readonly List<Server> _serverList;
	private readonly ReaderWriterLockSlim _lock;

	public ServerManager()
	{
		_serverList = new List<Server>();
		_lock = new ReaderWriterLockSlim();
	}

	public void AddServer(Server newServer)
	{
		_lock.EnterWriteLock();
		try
		{
			_serverList.Add(newServer);
		}
		finally
		{
			_lock.ExitWriteLock();
		}
	}
	
	public void RemoveServer(Guid serverId)
	{
		_lock.EnterWriteLock();
		try
		{
			_serverList.RemoveAll(s => s.Id == serverId);
		}
		finally
		{
			_lock.ExitWriteLock();
		}
	}

	public List<Server> GetAllServers()
	{
		_lock.EnterReadLock();
		try
		{
			return _serverList.ToList(); // Return a copy to prevent external modification
		}
		finally
		{
			_lock.ExitReadLock();
		}
	}

	public void Dispose()
	{
		_lock.Dispose();
	}
}