namespace Design_Patterns.Command;

public class LightOffCommand(Light light) : Command
{
	public void Execute()
	{
		light.TurnOff();
	}
}