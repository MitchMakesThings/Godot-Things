using BloodHarvest.scripts;
using Godot;
using BloodHarvest.scripts.interfaces;

public partial class Buildable : Node3D, IInteractable
{
    [Export]
    private ShaderMaterial GhostMaterial;

    [Export]
    private PackedScene BuildScene;

    [Export]
    private MeshInstance3D? Mesh;

    [Export]
    private int Cost { get; set; } = 10;

    [Export]
    private bool Rebuildable = true;

    [Export]
    private AudioStreamPlayer3D? _audio;

    public string Title { get; set; } = "Build";

    public string Subtitle => $"{Cost} {GameState.Instance.ResourceName}";

    public override void _Ready()
    {
        base._Ready();

        if (Mesh is null) return;

        for (int i = 0; i < Mesh.GetSurfaceOverrideMaterialCount(); i++)
        {
            Mesh.SetSurfaceOverrideMaterial(i, GhostMaterial);
        }
    }

    public void Interact(InteractionSystem interactionSystem)
    {
        if (GameState.Instance.Resource < Cost)
        {
            GameState.Instance.PlayRejectSound();
            return;
        }

        GameState.Instance.RemoveResource(Cost);
        
        var built = BuildScene.Instantiate<Attackable>();
        if (Rebuildable)
        {
            built.Builder = this;
        }
        GetParent().AddChild(built);
        built.GlobalPosition = GlobalPosition;
        built.GlobalRotation = GlobalRotation;
        built.AddToGroup(Groups.Built);

        // Aniamte in
        var intendedScale = built.Scale;
        built.Scale = Vector3.Zero;
        var tween = GetTree().CreateTween();
        tween.SetTrans(Tween.TransitionType.Bounce);
        tween.TweenProperty(built, "scale", intendedScale, .5);

        if (_audio is not null)
        {
            _audio.Play();
        }

        NavigationHelper.Instance.Rebuild();
 
        Hide();
    }

    public void Show()
    {
        Visible = true;
        ProcessMode = ProcessModeEnum.Inherit;
    }

    public void Hide()
    {
        Visible = false;
        ProcessMode = ProcessModeEnum.Disabled;
    }
}
