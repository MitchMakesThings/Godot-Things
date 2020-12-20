using Godot;
using System;

public class PressurePlate : Spatial
{
	[Export]
	NodePath ActivationNodePath;

	IActivatable ActivationNode;

	public override void _Ready()
	{
		base._Ready();

		ActivationNode = GetNode<IActivatable>(ActivationNodePath);
	}

	private void _on_Area_body_entered(object body)
	{
		// TODO - check type of body, we might only want to trigger for the player etc
		ActivationNode.Activate();
	}

	private void _on_Area_body_exited(object body)
	{
		// TODO - ensure all the bodies that entered have now exited
		ActivationNode.Deactivate();
	}
}