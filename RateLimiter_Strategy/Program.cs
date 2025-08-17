using System.Collections.Concurrent;

class User(string name)
{
	public Guid Id { get; init; } = Guid.NewGuid();
	public string Name { get; set; } = name;
}

interface IRateLimitingStrategy
{
	bool Process();
}

class TokeBucketStrategy : IRateLimitingStrategy
{
	private int BucketSize { get; } = 5;
	private int RefillRate { get; } = 2;
	private int CurrentTokenCount { get; set; } = 5;
	private DateTime LastRefillTimeStamp { get; set; } = DateTime.UtcNow;
	private readonly Object _tokenBucketLock = new();

	private void Refill()
	{
		lock (_tokenBucketLock)
		{
			DateTime currentTime = DateTime.UtcNow;
			var minutesElapsed = (int)(currentTime - LastRefillTimeStamp).TotalMinutes;
			if (minutesElapsed > 0) // ✅ update only when tokens actually added
			{
				LastRefillTimeStamp = currentTime;
				int tokensToAdd = RefillRate*minutesElapsed;
				CurrentTokenCount = Math.Min(CurrentTokenCount + tokensToAdd, BucketSize);
			}
		}
	}

	public bool Process()
	{
		lock (_tokenBucketLock)
		{
			Refill();
			if (CurrentTokenCount > 0)
			{
				CurrentTokenCount -= 1;
				if (CurrentTokenCount > 0)
					Console.WriteLine($"Request Processed! Now {CurrentTokenCount} tokens left!");
				return true;
			}

			Console.WriteLine("429 : Too Many requests!");
			return false;
		}
	}
}

class FixedWindowStrategy : IRateLimitingStrategy
{
	public bool Process()
	{
		Console.WriteLine("Request processed!");
		return true;
	}	
}

class RateLimiter
{
	private static RateLimiter _instance;
	private static Object _rateLimiterObject = new();
	private ConcurrentDictionary<Guid, IRateLimitingStrategy> _userRateLimits = new();
	
	private RateLimiter(){}

	public static RateLimiter GetInstance()
	{
		if (_instance == null)
		{
			lock (_rateLimiterObject)
			{
				if (_instance == null)
				{
					_instance = new RateLimiter();
				}
			}
		}

		return _instance;
	}

	public void SetUserStrategy(Guid userId, IRateLimitingStrategy newStrategy)
	{
		_userRateLimits[userId] = newStrategy;
	}

	public void ProcessRequest(Guid userId)
	{
		if (_userRateLimits.TryGetValue(userId, out var strategy))
		{
			strategy.Process();
		}
		else
		{
			Console.WriteLine($"{userId} has no strategy assigned!");
		}
	}
}

class RateLimitingDemo
{
	public static void Main(string[] args)
	{
		Console.WriteLine("Hello from Rate Limiter!");
		RateLimiter rateLimiter = RateLimiter.GetInstance();
		User user1 = new("Alice");
		User user2 = new("Bob");
		
		rateLimiter.SetUserStrategy(user1.Id, new TokeBucketStrategy());

		for (int index = 0; index <= 7; index++)
		{
			rateLimiter.ProcessRequest(user1.Id);
			if (index == 5)
			{
				Thread.Sleep(60000);
			}
		}
		rateLimiter.ProcessRequest(user2.Id);
	}
}
