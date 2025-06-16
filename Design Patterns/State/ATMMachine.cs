namespace Design_Patterns.State;

public class ATMMachine
{
	private IAtmState _atmState;

	public void SetState(IAtmState state)
	{
		_atmState = state;
	}

	public void Process()
	{
		_atmState.Handle();
	}
}