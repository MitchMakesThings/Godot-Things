using Godot;
using System;

public class Pedestal : StaticBody, IInteractable
{
	[Export]
	NodePath ActivatableTargetNodePath;
	IActivatable Target;

	public override void _Ready() {
		Target = GetNode(ActivatableTargetNodePath) as IActivatable;	// Note: Could be null!
	}

	public string InteractionText => "Activate";

	public bool CanInteract(Node caller)
	{
		return caller is Character;
	}

	public void Interact(Node caller)
	{
		if (Target != null) {
			Target.Activate();
		}
	}
}
