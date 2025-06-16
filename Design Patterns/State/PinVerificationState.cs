namespace Design_Patterns.State;

public class PinVerificationState : IAtmState
{
	public void Handle()
	{
		Console.WriteLine("Pin Verification in progress!");
	}
}