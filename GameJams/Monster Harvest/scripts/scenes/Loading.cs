using Godot;
using System;

// This is a very crude attempt at forcing the game to pre-cache all the shaders etc.
// We cost a few seconds up front to hopefully avoid run-time stutters when things are instantiated for the first time.
public partial class Loading : Node3D
{
    [Export]
    private PackedScene _sceneToLoad = null!; // Set in editor
    
    // Called from the animation player
    public void OnFinishedLoading()
    {
        GetTree().ChangeSceneToPacked(_sceneToLoad);
    }
}
