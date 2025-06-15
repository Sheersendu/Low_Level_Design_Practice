namespace Design_Patterns.Command;

public class Remote
{
	private Command _command;

	public void SetCommand(Command command)
	{
		_command = command;
	}

	public void PressButton()
	{
		_command.Execute();
	}
}