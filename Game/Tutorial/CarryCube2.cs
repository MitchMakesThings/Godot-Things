using Godot;
using System;

public class CarryCube2 : RigidBody, IInteractable
{

    public string InteractionText => OriginalParent == GetParent() ? "Pick up" : "Throw";

	public Spatial OriginalParent;

	[Export]
	float ThrowStrength = 100;

	public override void _Ready()
	{
		base._Ready();

		OriginalParent = GetParent() as Spatial;
	}

	public bool CanInteract(Node caller)
	{
		return caller is Character2;
	}

	public void Interact(Node caller)
	{
		if (Mode == ModeEnum.Rigid) {
			Mode = ModeEnum.Kinematic;

			var currentTransform = GlobalTransform;
			GetParent().RemoveChild(this);
			var attachPoint = ((Character2)caller).GetAttachPoint();
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
			
			Character2 thrower = caller as Character2;
			ApplyCentralImpulse(-thrower.GlobalTransform.basis.z * ThrowStrength);
		}
	}
}
