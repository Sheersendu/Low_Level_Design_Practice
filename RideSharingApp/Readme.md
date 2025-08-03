### 💡 Core Requirements

- Focus on basic ride-booking flow:

    User Types

        Rider (can book a ride)

        Driver (can accept a ride)

    Core Entities

        User (base)

        Rider extends User

        Driver extends User

        Location (latitude, longitude)

        Ride (with status: REQUESTED, ONGOING, COMPLETED, CANCELLED)

    Main Functional Requirements

        A rider can request a ride.

        A driver can accept a ride (maybe closest one).

        A ride has a start & end location.

        Show driver availability.

        Maintain ride history per user.