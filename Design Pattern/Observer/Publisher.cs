namespace Design_Patterns.Observer;

public class Publisher
{
	private List<ISubscriber> subscriberList;
	
	public Publisher()
	{
		subscriberList = [];
	}
	
	public void AddSubscriber(ISubscriber subscriber)
	{
		subscriberList.Add(subscriber);
	}
	
	public void RemoveSubscriber(ISubscriber subscriber)
	{
		subscriberList.Remove(subscriber);
	}
	
	public void NotifySubscribers(string message)
	{
		foreach (var subscriber in subscriberList)
		{
			subscriber.update(message);
		}
	}
}