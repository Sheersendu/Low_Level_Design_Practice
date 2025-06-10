using Design_Patterns.Strategy;
using Design_Patterns.Observer;

// Strategy Pattern Example
/*
IPaymentStrategy paymentStrategy = new CreditCardPayment();
PaymentService paymentService = new(paymentStrategy);

paymentService.Pay(100.00m);
paymentService.SetPaymentStrategy(new DebitCardPayment());
paymentService.Pay(50.00m);
*/

// Observer Pattern Example

Publisher publisher = new();
ISubscriber emailSubscriber = new EmailSubscriber();
publisher.AddSubscriber(emailSubscriber);
ISubscriber smsSubscriber = new SMSSubscriber();
publisher.AddSubscriber(smsSubscriber);
publisher.NotifySubscribers("New product launch!");
publisher.RemoveSubscriber(emailSubscriber);
publisher.NotifySubscribers("Price drop on existing products!");