using Godot;

public class Pedestal : StaticBody, IInteractable
{
	[Export]
	NodePath ActivatableTargetNodePath;
	IActivatable Target;

	[Export]
	NodePath CSGNodePath;
	CSGBox CSGBox;

	[Export]
	Color PressedColor;

	[Export]
	Color NotPressedColor;

	bool HasBeenPressed = false;

	public override void _Ready() {
		Target = GetNode(ActivatableTargetNodePath) as IActivatable;	// Note: Could be null!

		CSGBox = GetNode<CSGBox>(CSGNodePath);
		SetColour(NotPressedColor);
	}

	public string InteractionText => "Activate";

	public bool CanInteract(Node caller)
	{
		return !HasBeenPressed && caller is Character;
	}

	public void Interact(Node caller)
	{
		if (!HasBeenPressed && Target != null) {
			Target.Activate();
			HasBeenPressed = true;
			SetColour(PressedColor, 2);
		}
	}

	private void SetColour(Color newColour, float glowIntensity = 100) {
		((ShaderMaterial)CSGBox.Material).SetShaderParam("Color", newColour);
		((ShaderMaterial)CSGBox.Material).SetShaderParam("EmissionMultiplier", glowIntensity);
	}
}
