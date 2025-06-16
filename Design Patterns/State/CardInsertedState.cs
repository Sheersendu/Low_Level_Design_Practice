namespace Design_Patterns.State;

public class CardInsertedState : IAtmState
{
	public void Handle()
	{
		Console.WriteLine("Card inserted!");
	}
}