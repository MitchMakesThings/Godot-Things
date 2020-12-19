using Godot;
using System;

public class PressurePlate : Spatial
{
	[Export]
	NodePath ActivationNodePath;

	IInteractable ActivationNode;

	public override void _Ready()
	{
		base._Ready();

		ActivationNode = GetNode<IInteractable>(ActivationNodePath);
	}

	private void _on_Area_body_entered(object body)
	{
		// TODO - check type of body, we might only want to trigger for the player etc
		ActivationNode.Interact();
	}

	private void _on_Area_body_exited(object body)
	{
		// TODO - ensure all the bodies that entered have now exited
		ActivationNode.Interact();
	}
}