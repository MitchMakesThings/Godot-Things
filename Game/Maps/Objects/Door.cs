using Godot;
using System;

public class Door : Spatial, IActivatable
{
	[Export]
	NodePath AnimationPlayerNodePath;

	AnimationPlayer AnimationPlayer;

	public override void _Ready()
	{
		base._Ready();

		AnimationPlayer = GetNode<AnimationPlayer>(AnimationPlayerNodePath);
		AnimationPlayer.AssignedAnimation = AnimationPlayer.GetAnimationList()[0];
	}

	public void Activate()
	{
		AnimationPlayer.Play();
	}

	public void Deactivate()
	{
		AnimationPlayer.PlayBackwards();
	}
}
