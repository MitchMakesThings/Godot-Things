using Godot;
using System;

public class VictoryScreen : Control
{
	private void _on_Restart_pressed()
	{
		TestGameMode.Instance.Restart();
	}
	
	private void _on_Exit_pressed()
	{
		GetTree().Quit();
	}
}
