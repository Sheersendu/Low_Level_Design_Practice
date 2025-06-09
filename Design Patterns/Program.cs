using Design_Patterns.Strategy;

// Strategy Pattern Example

IPaymentStrategy paymentStrategy = new CreditCardPayment();
PaymentService paymentService = new(paymentStrategy);

paymentService.Pay(100.00m);
paymentService.SetPaymentStrategy(new DebitCardPayment());
paymentService.Pay(50.00m);