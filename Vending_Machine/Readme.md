# Designing a Vending Machine

## Requirements
1. The vending machine should support multiple products with different prices and quantities.
2. The machine should accept coins and notes of different denominations.
3. The machine should dispense the selected product and return change if necessary.
4. The machine should keep track of the available products and their quantities.
5. The machine should handle multiple transactions concurrently and ensure data consistency.
6. The machine should provide an interface for restocking products and collecting money.
7. The machine should handle exceptional scenarios, such as insufficient funds or out-of-stock products.

# Design patterns Applicable

1. Since different products with prices and quantities : `Factory` as we can produce a product with same price in bulk quantity. Reuse for different products
2. `State` design pattern for handling different states
3. `Strategy` for different payment methods(for future by default keeping cash)

# Classes/Interfaces/Enums

1. Coins, Notes, Products : `ENUM`
2. For handling multiple transactions concurrently : `Locking or concurrency control`
3. Restocking and Collection of money : `Interface` which is implemented by the Admin which also has an object of the vending machine
4. Rest all requirements can be handled in different states

# User Happy flow
1. User initiates the process 
2. User chooses product and its quantity to buy
3. Machine validates the quantity 
4. User pays for the product
5. Machine validates the amount 
6. Machine dispenses the product
7. Machine subtracts the quantity from totalQuantity of that product

# Owner Happy Flow
1. Owner (re)stocks the products (existing ones no need of feature of adding new products, basically adds the quantity of stocks)
2. Owner collects money

### Note: 
1. Not using any login for now for user/owner just an interface 
2. Assuming there are buttons for item selection and for entering the quantity as well 

# Implementation
1. Inventory : Have a dictionary of items and its quantity, AddItem method, Make the total items to be configurable
2. Item : name, price
3. ItemType, Coin, Note : ENUM
4. VendingMachineContext : Contain the inventory, coinList, noteList