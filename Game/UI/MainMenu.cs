using Godot;
using System;

public class MainMenu : CanvasLayer
{
	[Export]
	NodePath ipInputNodePath;
	TextEdit ipInput;

	[Export]
	PackedScene GameScene;

	public override void _Ready()
	{
		base._Ready();

		ipInput = GetNode<TextEdit>(ipInputNodePath);

		Network.Instance.Connect(nameof(Network.ServerCreated), this, nameof(_on_ready_to_play));
		Network.Instance.Connect(nameof(Network.JoinSuccess), this, nameof(_on_ready_to_play));
		Network.Instance.Connect(nameof(Network.JoinFailed), this, nameof(_on_join_failed));
	}

	private void _on_ready_to_play() {
		GetTree().ChangeSceneTo(GameScene);

		// TODO - we need some way of having a generic game mode, instead of it being a hardcoded singleton
		// Could be a singleton on a base GameMode? Then we load the appropriate game mode from here?
		TestGameMode.Instance.StartGame();
	}

	private void _on_join_failed() {
		GD.Print("Failed to join server");
	}

	private void _on_btnJoin_pressed()
	{
		Network.Instance.JoinServer(ipInput.Text, Network.Port);
	}
	
	
	private void _on_btnHost_pressed()
	{
		Network.Instance.CreateServer();
	}
}



