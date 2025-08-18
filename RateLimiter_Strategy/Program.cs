using System.Collections.Concurrent;
using System.ComponentModel.Design;

class User(string name)
{
	public Guid Id { get; init; } = Guid.NewGuid();
	public string Name { get; set; } = name;
}

class RateLimitConfig(int limit, int refillRate)
{
	public int Limit { get; set; } = limit;
	public int RefillRate { get; set; } = refillRate;
}

class UserState(int currentTokens, DateTime lastRefillTime)
{
	public int CurrentTokens { get; set; } = currentTokens;
	public DateTime LastRefillTime { get; set; } = lastRefillTime;
    public readonly Object Lock = new ();
}

interface IRateLimitingStrategy
{
	bool Process(RateLimitConfig config, UserState state);
}

class TokeBucketStrategy : IRateLimitingStrategy
{

	public bool Process(RateLimitConfig config, UserState state)
	{
		lock (state.Lock)
		{
			DateTime currentTime = DateTime.UtcNow;
			var minutesElapsed = (int)(currentTime - state.LastRefillTime).TotalMinutes;
			if (minutesElapsed > 0)
			{
				state.LastRefillTime = currentTime;
				int tokensToAdd = config.RefillRate * minutesElapsed;
				state.CurrentTokens = Math.Min(state.CurrentTokens + tokensToAdd, config.Limit);
			}
			if (state.CurrentTokens > 0)
			{
				state.CurrentTokens -= 1;
				Console.WriteLine($"Request Processed! Now {state.CurrentTokens} tokens left!");
				return true;
			}

			Console.WriteLine("429 : Too Many requests!");
			return false;
		}
	}
}

class FixedWindowStrategy : IRateLimitingStrategy
{
	public bool Process(RateLimitConfig config, UserState state)
	{
		Console.WriteLine("Request processed!");
		return true;
	}	
}

class RateLimiter
{
	private static RateLimiter _instance;
	private IRateLimitingStrategy _strategy = new TokeBucketStrategy();
	private static readonly Object RateLimiterObject = new();
	private ConcurrentDictionary<Guid, UserState> _userRateLimits = new();
	
	private RateLimiter(){}

	public static RateLimiter GetInstance()
	{
		if (_instance == null)
		{
			lock (RateLimiterObject)
			{
				if (_instance == null)
				{
					_instance = new RateLimiter();
				}
			}
		}

		return _instance;
	}

	public void SetStrategy(IRateLimitingStrategy newStrategy)
	{
		_strategy = newStrategy;
	}

	public void ProcessRequest(Guid userId, RateLimitConfig config)
	{
		var userState = _userRateLimits.GetOrAdd(userId, _ => new UserState(config.Limit, DateTime.UtcNow));
		_strategy.Process(config, userState);
	}
}

class RateLimitingDemo
{
	public static void Main(string[] args)
	{
		RateLimiter rateLimiter = RateLimiter.GetInstance();
		User user1 = new("Alice");
		User user2 = new("Bob");

		RateLimitConfig user1Config = new(5, 2);
		RateLimitConfig user2Config = new(10, 5);

		for (int index = 0; index <= 7; index++)
		{
			rateLimiter.ProcessRequest(user1.Id, user1Config);
			if (index == 5)
			{
				Thread.Sleep(60000);
			}
		}
		
		for (int index = 0; index <= 7; index++)
		{
			rateLimiter.ProcessRequest(user2.Id, user2Config);
		}
	}
}

#region Readme!!!
// I kept the strategy stateless; per-user runtime state (tokens, refill timestamp) is stored separately.
// Here, it’s an in-memory ConcurrentDictionary, but in production we will use Redis with TTL for resilience and scaling.
// Each user has its own config and lock, so concurrent requests don’t block across users. The refill logic is time-based and tunable.
// This makes the design extensible, thread-safe, and production-scalable.
#endregion