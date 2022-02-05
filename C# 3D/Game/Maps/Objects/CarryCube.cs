using Godot;
using System;

public class CarryCube : RigidBody, IInteractable
{
	public string InteractionText => OriginalParent == GetParent() ? "Pick up" : "Throw";

	public Spatial OriginalParent;

	public override void _Ready()
	{
		base._Ready();

		OriginalParent = GetParent() as Spatial;
	}

	public bool CanInteract(Node caller)
	{
		return caller is Character;
	}

	public void Interact(Node caller)
	{
		if (Mode == ModeEnum.Rigid) {
			Mode = ModeEnum.Kinematic;

			var currentTransform = GlobalTransform;
			GetParent().RemoveChild(this);
			var attachPoint = ((Character)caller).GetAttachPoint();
			if (attachPoint != null) {
				attachPoint.AddChild(this);	
			} else {
				caller.AddChild(this);
			}
			GlobalTransform = currentTransform;
		} else {
			Mode = ModeEnum.Rigid;

			var currentTransform = GlobalTransform;
			GetParent().RemoveChild(this);
			OriginalParent.AddChild(this);
			GlobalTransform = currentTransform;
			
			// TODO Get throw strength from Character?
			Character thrower = caller as Character;
			ApplyCentralImpulse(-thrower.GlobalTransform.basis.z * 100);
		}
	}
}
