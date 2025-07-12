namespace RateLimiting;

public class RateLimiter
{
	private IStrategy currentStrategy = new TokenBucketStrategy();

	public void SetStrategy(IStrategy rateLimitingStrategy)
	{
		currentStrategy = rateLimitingStrategy;
	}

	public void ProcessRequest(int userId)
	{
		currentStrategy.ProcessRequest(userId);
	}
}