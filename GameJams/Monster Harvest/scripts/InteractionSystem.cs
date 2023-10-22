using BloodHarvest.scripts;
using Godot;
using BloodHarvest.scripts.interfaces;

public partial class InteractionSystem : Node3D
{
    [Export]
    private ShapeCast3D Cast { get; set; }
    
    [Export]
    private CanvasLayer Layer { get; set; }

    [Export]
    private Label _outcomeLabel;

    [Export]
    private Label _costLabel;
    
    public PlayerCharacter Player { get; private set; } = null!; // Set in _Ready

    public override void _Ready()
    {
        base._Ready();

        Player = GetTree().GetNodesInGroup(Groups.Players)[0] as PlayerCharacter; // TODO multiplayer won't work like this!
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (!Cast.IsColliding() || Cast.GetCollider(0) is not IInteractable interactable)
        {
            Layer.Visible = false;
            return;
        }
        
        // Update the UI
        Layer.Visible = true;
        _outcomeLabel.Text = interactable.Title;
        _costLabel.Text = interactable.Subtitle;
        
        // Handle input
        if (Input.IsActionJustPressed("interact"))
        {
            interactable.Interact(this);
        }
    }
}
