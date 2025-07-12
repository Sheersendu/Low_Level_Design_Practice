using System.Collections.Concurrent;

namespace RateLimiting;

public class TokenBucketStrategy : IStrategy
{
	private readonly ConcurrentDictionary<int, Token> userToken;
	private readonly int bucketSize = 10;
	private readonly int refillRate = 2;
	
	public TokenBucketStrategy()
	{
		userToken = new ConcurrentDictionary<int, Token>();
	}

	public void Refill(Token currentUserToken)
	{
		lock (currentUserToken)
		{
			DateTime currentTimeStamp = DateTime.UtcNow;
			DateTime lastRefillTimeStamp = currentUserToken.LastRefilledTimeStamp;
			int elapsedMinutes = (int)(currentTimeStamp - lastRefillTimeStamp).TotalMinutes;
			int tokensToRefill = elapsedMinutes*refillRate;
			Interlocked.Add(ref currentUserToken.TokenCount, Math.Min(bucketSize, tokensToRefill));
			Console.WriteLine($"Updated user token: {currentUserToken.TokenCount}");
			currentUserToken.LastRefilledTimeStamp = currentTimeStamp;
		}
	}
	
	public void ProcessRequest(int userId)
	{
		var currentUserToken = userToken.GetOrAdd(userId, id => new Token());
		if (currentUserToken.TokenCount == 0)
		{
			Refill(currentUserToken);
		}

		if (currentUserToken.TokenCount > 0)
		{
			Interlocked.Decrement(ref currentUserToken.TokenCount);
			Console.WriteLine($"200: Processing request for {userId}");
		}
		else
		{
			Console.WriteLine("429: Too many requests! Try again in sometime!");
		}
	}
}