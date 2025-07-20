enum Channel
{
	EMAIL, SMS, PUSH
}

interface INotification
{
	Channel GetChannel();
	string GetRecipient();
	string GetContent();
}

class EmailNotification : INotification
{
	private string _toEmail;
	private string _subject;
	private string _body;

	public EmailNotification(string toEmail, string subject, string body)
	{
		_toEmail = toEmail;
		_subject = subject;
		_body = body;
	}
	
	public Channel GetChannel()
	{
		return Channel.EMAIL;
	}
	
	public string GetRecipient()
	{
		return _toEmail;
	}
	
	public string GetContent()
	{
		return _body;
	}
	
	public string GetSubject()
	{
		return _subject;
	}
}

class SmsNotification : INotification
{
	private string _phoneNumber;
	private string _content;

	public SmsNotification(string phoneNumber, string content)
	{
		_phoneNumber = phoneNumber;
		_content = content;
	}
	
	public Channel GetChannel()
	{
		return Channel.SMS;
	}
	public string GetRecipient()
	{
		return _phoneNumber;
	}
	public string GetContent()
	{
		return _content;
	}
}

class PushNotification : INotification
{
	private string _deviceId;
	private string _content;
	private string _title;

	public PushNotification(string deviceId, string content, string title)
	{
		_deviceId = deviceId;
		_content = content;
		_title = title;
	}
	
	public Channel GetChannel()
	{
		return Channel.PUSH;
	}
	
	public string GetRecipient()
	{
		return _deviceId;
	}
	
	public string GetContent()
	{
		return _content;
	}
}

interface INotificationSender
{
	void send(INotification notification);
}

class EmailNotificationSender : INotificationSender
{
	public void send(INotification notification)
	{
		Console.WriteLine($"Sending Email to {notification.GetRecipient()}");
	}
}

class SmsNotificationSender : INotificationSender
{
	public void send(INotification notification)
	{
		Console.WriteLine($"Sending SMS to {notification.GetRecipient()}");
	}
}

class PushNotificationSender : INotificationSender
{
	public void send(INotification notification)
	{
		Console.WriteLine($"Sending Push notification to {notification.GetRecipient()}");
	}
}

class NotificationFactory
{
	private Dictionary<Channel, INotificationSender> _notificationSenderMap;
	public NotificationFactory()
	{
		_notificationSenderMap = new();
		INotificationSender emailNotificationSender = new EmailNotificationSender();
		INotificationSender pushNotificationSender = new PushNotificationSender();
		INotificationSender smsNotificationSender = new SmsNotificationSender();
		_notificationSenderMap.Add(Channel.EMAIL, emailNotificationSender);
		_notificationSenderMap.Add(Channel.PUSH, pushNotificationSender);
		_notificationSenderMap.Add(Channel.SMS, smsNotificationSender);
	}
	
	public INotificationSender GetNotificationSender(Channel channel)
	{
		var notificationSender = _notificationSenderMap.GetValueOrDefault(channel);
		if (notificationSender == default)
		{
			throw new InvalidOperationException($"No notification sender found for {channel}");
		}

		return notificationSender;
	}
}

class NotificationService
{
	private readonly NotificationFactory _notificationFactory = new();

	public void Notify(INotification notification)
	{
		INotificationSender notificationSender = _notificationFactory.GetNotificationSender(notification.GetChannel());
		notificationSender.send(notification);
	}
}

class App
{
	public static void Main(string[] args)
	{
		NotificationService notificationService = new();
		EmailNotification emailNotification = new ("abc@email.com", "Test Email!", "This is a test email!");
		notificationService.Notify(emailNotification);

		SmsNotification smsNotification = new("9876543210", "Hello from SMS service!");
		notificationService.Notify(smsNotification);

		PushNotification pushNotification = new("D_1289Swty", "You have a push message!", "Knock! Knock!");
		notificationService.Notify(pushNotification);
	}
}