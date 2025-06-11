using Design_Patterns.Builder;
using Design_Patterns.Factory;
using Design_Patterns.Strategy;
using Design_Patterns.Observer;
using Design_Patterns.Singleton;

// Strategy Pattern Example
/*
IPaymentStrategy paymentStrategy = new CreditCardPayment();
PaymentService paymentService = new(paymentStrategy);

paymentService.Pay(100.00m);
paymentService.SetPaymentStrategy(new DebitCardPayment());
paymentService.Pay(50.00m);
*/

// Observer Pattern Example

/*
Publisher publisher = new();
ISubscriber emailSubscriber = new EmailSubscriber();
publisher.AddSubscriber(emailSubscriber);
ISubscriber smsSubscriber = new SMSSubscriber();
publisher.AddSubscriber(smsSubscriber);
publisher.NotifySubscribers("New product launch!");
publisher.RemoveSubscriber(emailSubscriber);
publisher.NotifySubscribers("Price drop on existing products!");
*/

// Factory Pattern Example

/*
IVehicle vehicle1 = VehicleFactory.GetVehicle("Car");
vehicle1.Drive();
IVehicle vehicle2 = VehicleFactory.GetVehicle("Bike");
vehicle2.Drive();
*/

// Singleton Pattern Example

/*
int threadCount = 5;
Thread[] threads = new Thread[threadCount];
for (int index = 0; index < threadCount; index++)
{
	threads[index] = new Thread(() =>
	{
		DbConnection instance = DbConnection.GetInstance();
	});
	threads[index].Start();
}

foreach (var thread in threads)
{
	thread.Join();
}

Console.WriteLine($"DbConnection constructor called: {DbConnection.ConstructorCallCount} time(s)");
*/

// Builder Pattern Example

Student student = new Student.StudentBuilder()
	.setName("Rudra")
	.setEmail("Rudra@Roop.com")
	.setPhoneNumber("86786905431")
	.build();

student.getDetails();









