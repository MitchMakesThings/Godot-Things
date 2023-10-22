using Godot;
using System;

public partial class MainMenu : Control
{
    [Export]
    private PackedScene PlayScene;

    [Export]
    private PackedScene SettingsScene;
    
    public void OnPlay()
    {
        GameState.Instance.CleanState();
        GetTree().ChangeSceneToPacked(PlayScene);
    }

    public void OnSettings()
    {
        GetTree().ChangeSceneToPacked(SettingsScene);
    }
    
    public void OnExit()
    {
        GetTree().Quit();
    }
}
