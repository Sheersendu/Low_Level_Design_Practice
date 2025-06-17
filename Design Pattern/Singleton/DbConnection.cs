namespace Design_Patterns.Singleton;

public class DbConnection
{
	private static DbConnection _instance;
	private static readonly object _lock = new();
	private static int _constructorCallCount = 0;
	
	// Made ctor private so that no instances can be created outside this class
	private DbConnection()
	{
		Interlocked.Increment(ref _constructorCallCount);
	}
	
	public static DbConnection GetInstance()
	{
		if (_instance == null) // If instance already exists
		{
			lock (_lock)
			{
				if (_instance == null) // If instance created on some other thread
				{
					_instance = new DbConnection();
				}
			}
		}

		return _instance;
	}
	
	public static int ConstructorCallCount => _constructorCallCount;
}