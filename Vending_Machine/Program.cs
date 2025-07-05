using Vending_Machine;
using Vending_Machine.Enums;
using Vending_Machine.Utilities;

VendingMachine vendingMachine = new();

Item mangoJuice = new Item("Mango Juice", 45.00, ItemType.Drink);
Item potatoChips = new Item("Potato Chips", 25.00, ItemType.Snack);
Item proteinBar = new Item("Protein Bar", 59.00, ItemType.Chocolate);

vendingMachine.RestockItem(mangoJuice, 10);
vendingMachine.RestockItem(potatoChips, 15);
vendingMachine.RestockItem(proteinBar, 12);

vendingMachine.Init();