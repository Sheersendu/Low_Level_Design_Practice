namespace RateLimiting;

public interface IStrategy
{
	void Refill(Token currentUserToken);
	void ProcessRequest(int userId);
}