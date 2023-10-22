using Godot;

public partial class GameOver : Node3D
{
    [Export]
    private Label CollectedLabel;

    [Export]
    private Label KilledLabel;

    public override void _Ready()
    {
        base._Ready();

        CollectedLabel.Text = $"Collected: {GameState.Instance.ResourcesCollected}";
        KilledLabel.Text = $"Killed: {GameState.Instance.Kills}";
        // TODO Survived X nights?

        Input.MouseMode = Input.MouseModeEnum.Visible;
    }
}
