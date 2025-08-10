### Core Features

#### Parking Operations

        Park a vehicle.

        Unpark a vehicle.

        Check available parking slots.

        Support multiple vehicle types (bike, car, truck, etc.).

#### Different Parking Ways (Slot Allocation Strategies)

        Nearest slot first (e.g., slot #1 before slot #2).

        Random slot allocation (e.g., load balancing across floors).

        Grouped allocation (e.g., park near similar vehicles).

        This is where Strategy Pattern can be used → IParkingStrategy with concrete strategies like NearestSlotStrategy, RandomSlotStrategy.

#### Different Pricing Models

        Hourly rate.

        Flat rate.

        Progressive rate (e.g., first hour $X, subsequent hours $Y).

        Event-based pricing (e.g., special rates on weekends).

        Strategy Pattern here too → IPricingStrategy with HourlyPricingStrategy, FlatPricingStrategy, etc.

#### Multi-Floor Support

        Parking lot may have multiple floors, each with its own slots and possibly different pricing rules.

#### Vehicle Tracking

        Maintain mapping between parked vehicles and their allocated slots.

        Track entry time for pricing calculation.

#### Receipts & Payment

        When a vehicle is unparked, generate a receipt with:

            Vehicle number

            Slot/floor details

            Entry & exit time

            Amount charged (based on chosen pricing strategy)

#### Time : 48mins