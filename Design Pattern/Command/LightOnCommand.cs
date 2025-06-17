namespace Design_Patterns.Command;

public class LightOnCommand(Light light) : Command
{
	public void Execute()
	{
		light.TurnOn();
	}
}