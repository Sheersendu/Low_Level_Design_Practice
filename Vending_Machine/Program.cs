using Vending_Machine;
using Vending_Machine.Enums;
using Vending_Machine.Utilities;

Item mangoJuice = new Item("Mango Juice", 45.37, ItemType.Drink);
Item potatoChips = new Item("Potato Chips", 25.52, ItemType.Snack);
Item proteinBar = new Item("Protein Bar", 59.89, ItemType.Chocolate);

VendingMachineContext vendingMachineContext = new ();
vendingMachineContext.UpdateCoinList();
vendingMachineContext.UpdateNoteList();

Inventory inventory = vendingMachineContext.GetInventory();
inventory.RestockItem(mangoJuice,10);
inventory.RestockItem(potatoChips,15);
inventory.RestockItem(proteinBar,5);

vendingMachineContext.GetCurrentState().Process(vendingMachineContext);

Console.WriteLine(vendingMachineContext.GetCurrentState());


