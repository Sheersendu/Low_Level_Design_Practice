using RateLimiting;

RateLimiter rateLimiter = new ();
for (int request = 0; request < 8; request++)
{
	if (request == 3)
		Thread.Sleep(TimeSpan.FromMinutes(6));
	rateLimiter.ProcessRequest(1);
}
