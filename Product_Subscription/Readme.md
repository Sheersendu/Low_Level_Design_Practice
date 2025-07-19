# DB schema design:
1. User : Id(ID), Name(varchar(50)), Email(varchar(50)), HashPassword(varchar(100))/not req if Googlesignin, Phone No(varchar(20)), IsActive(bool), IsBuyer(bool), IsSeller(bool)
2. Address: Line1(varchar(100)), Line2(varchar(100)), City(varchar(50)), State(varchar(50)), Country(varchar(50))
3. Product : Id(ID), Name(varchar(50)), Description(varchar(200)), Price(Double(8,2)), ProductCategory(ID)[FK]
4. ProductCategory: Id(ID), Name(varchar(50)), ProductType(ID)
5. ProductType : Id(ID), Name(varchar(50))
6. SubscriptionType: Id(ID), Name(varchar(50))
7. Subscription: Id(ID), UserID(ID)[FK], ProductID(ID)[FK], SubscriptionType(ID)[FK], SubscriptionDate(DateTime)
8. ProductSeller

🔹 Q0: “What if we need to do global queries — like getting the most subscribed product across all users? How would you handle that?”

    You're right, cross-shard queries are inefficient. So for any global or reporting need — like most subscribed product — I’d introduce an async aggregation pipeline. Each time a subscription is created or cancelled, we publish that event to a Kafka topic. A background consumer updates a centralized analytics store. This way, operational data stays fast, and analytics stay scalable.

🔹 Q1: “How does the script scale when you have millions of users?”

    You shard by userId

    Run parallel workers, each working on one shard

    Optionally use batch processing or a job queue (e.g., AWS SQS, Celery, Kafka consumers)

🔹 Q2: “How do you handle missed jobs or failures?”

    Use idempotent processing (don’t schedule the same delivery twice)

    Keep a status column (isProcessed, lastAttempt, retryCount) in a `DeliverySchedule` table

    Optionally use a dead-letter queue for permanent failures

🔹 Q3: “How do you ensure a subscription is processed only once?”

    Use a lock or row-level flag (isProcessed = true)

    Or wrap it in a transaction

    Or have the script update status after queuing delivery

🔹 Q4: “What if the job takes longer than expected?”

    Use time-limited workers (e.g., run per shard for N mins)

    Add job health monitoring

    Possibly split by region, userId range, or even subscriptionType if load is high

🔹 Q5: “Can you avoid querying the entire Subscription table daily?”

    Yes — add an index on nextDeliveryDate

    Use: SELECT * FROM Subscription WHERE nextDeliveryDate = CURDATE() + INTERVAL 1 DAY

    Efficient filtering over a small indexed set