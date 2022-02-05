using Godot;
using System;

public class TestGameMode : Node
{
	[Export]
	private PackedScene VictoryScreenScene;
	private Control VictoryScreen;

	public static TestGameMode Instance { get; private set; }

	public override void _Ready()
	{
		base._Ready();

		Instance = this;

		// TODO - call this from the networking stuff after all players are loaded in
		StartGame();
	}

	public void EnteredWinArea(WinArea area, Character winningCharacter) {
		if (VictoryScreen == null) {
			VictoryScreen = (Control)VictoryScreenScene.Instance();
			AddChild(VictoryScreen);
		}

		VictoryScreen.Show();

		// Ensure the mouse can be used!
		Input.SetMouseMode(Input.MouseMode.Visible);

		// TODO disable character movement
		// This should use the character controller idea like UE4 - possess / unpossess etc
	}

	public void StartGame() {
		Input.SetMouseMode(Input.MouseMode.Captured);

		if (VictoryScreen != null) {
			VictoryScreen.Hide();
		}
	}

	public void Restart() {
		GetTree().ReloadCurrentScene();
		StartGame();
	}
}
