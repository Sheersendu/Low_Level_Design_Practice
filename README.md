# Low Level Design Practice

## Top 10 Design Patterns:

1. **Singleton**: Ensures a class has only one instance and provides a global point of access to it.
2. **Factory Method**: Defines an interface for creating an object, but lets subclasses alter the type of objects that will be created.
3. **Builder**: Separates the construction of a complex object from its representation, allowing the same construction process to create different representations.
4. **Strategy**: Defines a family of algorithms, encapsulates each one, and makes them interchangeable. Strategy lets the algorithm vary independently from clients that use it.
5. **Observer**: Defines a one-to-many dependency between objects so that when one object changes state, all its dependents are notified and updated automatically.
6. **State**: Allows an object to alter its behavior when its internal state changes. The object will appear to change its class.
7. **Command**: Encapsulates a request as an object, thereby allowing for parameterization of clients with queues, requests, and operations.
8. **Adapter**: Allows the interface of an existing class to be used as another interface. It acts as a bridge between two incompatible interfaces.
9. **Decorator**: Attaches additional responsibilities to an object dynamically. Decorators provide a flexible alternative to subclassing for extending functionality.
10. **Prototype**: Creates new objects by copying an existing object, known as the prototype. This pattern is useful when the cost of creating a new instance of an object is more expensive than copying an existing one.

## System Focus Areas & Suggested Patterns

1) Parking Lot Strategy Pattern (slot selection), Singleton
   (EntryManager), OCP
2) Elevator System Observer Pattern (request queue), State Pattern (Idle,
   Moving, etc.)
3) Library Management System SRP, Builder (Book/User creation), OCP
4) Amazon Locker Service Observer (notifications), Scheduling logic, Factory
5) Vending Machine State Pattern (Idle, HasMoney, Dispensing), Strategy
6) Online Blackjack Game Strategy (game rules), Factory (cards), State (player turns)
7) Meeting Scheduler Composite Pattern (recurring meetings), ConflictResolver
   class
8) Movie Ticket Booking System Decorator (seat types), Factory (tickets), Singleton
   (TheatreService)
9) Car Rental System Strategy (pricing), Factory (car objects), Builder
10) ATM State (Card Inserted, Pin Entered, Cash Dispensed),
    Command (transactions)
11) Chess Game Command (move/undo), Strategy (piece logic), Memento
12) Hotel Management System Builder (rooms), Strategy (pricing), SRP (cleaners, billing)
13) Amazon Online Shopping System Observer (order updates), Factory (items), Strategy
    (discount rules)
14) Stack Overflow Observer (comment/vote notify), Decorator (badges),
    Command
15) Restaurant Management System Command (place order), State (preparing, served), SRP
16) Facebook Feed Observer (post updates), Strategy (ranking), Caching
17) Online Stock Brokerage System Strategy (order matching), Command (trades), Singleton
    (Exchange)
18) Jigsaw Puzzle Game OOP modeling, Grid system, Factory (piece shapes)
19) Airline Management System Strategy (pricing), Composite (flights/legs), Observer
    (updates)
20) Cricinfo Engine Observer (live events), Decorator (score highlights),
    Command
21) LinkedIn Graph traversal (connections), Caching (profile info), SRP
22) Uber-like Cab Booking Strategy (matching), Observer (live location), State (trip)
23) Zomato/Food Delivery Observer (delivery updates), Strategy (ETA calculation)
24) Instagram Reels Observer (likes/comments), Caching (feeds), Factory
    (content)
25) Twitter Clone Observer (followers), Composite (retweets), Command
    (tweets)
26) Flipkart Flash Sale Locking, Singleton (InventoryManager), State (sale states)
27) Learning Platform (e.g. Coursera) Builder (Course), Strategy (pricing plans), SRP
28) Zoom/Google Meet Clone Composite (participants), Observer (chat/video),
    Command
29) Fantasy League App Strategy (scoring rules), Factory (players), Decorator
30) GitHub Clone Command (pull requests), Observer (notifications), SRP
31) Discord/Slack Observer (messages), Strategy (notification settings), SRP
32) Online Compiler Strategy (language interpreters), Isolation (sandboxing),
    SRP
33) Sports Tournament System Strategy (single-elim, round-robin), Builder
    (teams/schedule)
34) Quiz App Builder (quiz setup), Command (answer flow), Timer
35) Ride Pooling System Graph modeling, Strategy (match algorithm), Observer
    (status)
36) Banking System Singleton (AccountService), Command (transaction),
    Strategy (fees)
37) Interview Scheduler System Strategy (panel matching), SRP, Factory (round types)
38) Course Recommender Graph search (BFS/DFS), Observer (alerts), Caching
39) Hospital Management SRP (appointments, records), Observer (reminders),
    Factory
40) Ticketmaster Booking System Locking (seat hold), Builder (event setup), Strategy
41) WhatsApp Clone Observer (message delivery), Command (chat history),
    SRP
42) News Aggregator Strategy (ranking), Adapter (external APIs), Observer
43) Resume Parser Strategy (file formats), Builder (user profile), Factory
44) Cab Billing System Strategy (pricing models), Factory (fare), Singleton
45) Smart Home Automation Observer (sensor updates), Command (device control)
46) Calendar App Composite (events), Command (reminders), SRP
47) Online Game Store Command (purchases), Observer (download status),
    Decorator
48) PDF Editor Command (undo/redo), Builder (PDF structure), State
49) Expense Tracker (Splitwise) Strategy (split logic), Builder (expense group), Observer
50) Voice Assistant System Command (user intent), Strategy (action resolution), SRP