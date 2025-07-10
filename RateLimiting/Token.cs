namespace RateLimiting;

public class Token
{
	private DateTime _lastRefilledTimeStamp = DateTime.UtcNow;
	public volatile int TokenCount = 2;
	private readonly object _lockObject = new();

	public DateTime LastRefilledTimeStamp
	{
		get
		{
			lock (_lockObject)
			{
				return _lastRefilledTimeStamp;
			}
		}
		set
		{
			lock (_lockObject)
			{
				_lastRefilledTimeStamp = value;
			}
		}
	}
}