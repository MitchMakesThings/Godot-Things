using Godot;
using System;

public class PressurePlate : Spatial
{
	[Signal]
	public delegate void OnActivated();

	[Signal]
	public delegate void OnDeactivated();

	public override void _Ready()
	{
		base._Ready();
	}

	private void _on_Area_body_entered(object body)
	{
		// TODO - check type of body, we might only want to trigger for the player etc
		EmitSignal(nameof(OnActivated));
	}

	private void _on_Area_body_exited(object body)
	{
		// TODO - ensure all the bodies that entered have now exited
		EmitSignal(nameof(OnDeactivated));
	}
}