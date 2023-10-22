using Godot;
using BloodHarvest.scripts.interfaces;

public partial class MushroomPickup : StaticBody3D, IInteractable
{
    [Export]
    private MeshInstance3D Mesh;

    [Export]
    private AudioStreamPlayer PickupAudio;

    [Export]
    private AudioStreamPlayer3D SpawnAudio;
    
    public string Title { get; set; } = "Pick Mushroom";
    public string Subtitle { get; set; } = "A tasty treat!";
    public void Interact(InteractionSystem interactionSystem)
    {
        GameState.Instance.AddResource();
        
        interactionSystem.Player.Heal(10);

        // Immediately disable collision so the player can run on through
        foreach (var child in GetChildren())
        {
            if (child is CollisionShape3D shape)
            {
                shape.Disabled = true;
            }
        }

        var tween = GetTree().CreateTween();
        tween.SetTrans(Tween.TransitionType.Elastic);
        tween.TweenProperty(Mesh, "scale", Vector3.Zero, .5);

        PickupAudio.Play();
        tween.TweenCallback(Callable.From(QueueFree));
    }

    public override void _Ready()
    {
        base._Ready();

        var defaultScale = Mesh.Scale;
        Mesh.Scale = Vector3.Zero;

        SpawnAudio.Play();
        var tween = GetTree().CreateTween();
        tween.SetTrans(Tween.TransitionType.Bounce);
        //tween.TweenProperty(Mesh, "scale", new Vector3(defaultScale.X / 2, defaultScale.Y, defaultScale.Z / 2), .3);
        tween.TweenProperty(Mesh, "scale", defaultScale * 1.2f, .5);
        tween.SetTrans(Tween.TransitionType.Cubic);
        tween.TweenProperty(Mesh, "scale", defaultScale, .1);
    }
}
