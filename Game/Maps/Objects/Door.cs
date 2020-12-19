using Godot;
using System;

public class Door : Spatial, IInteractable
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

	public void Interact()
	{
		if (AnimationPlayer.CurrentAnimationPosition > 0) {
			AnimationPlayer.PlayBackwards();
		} else {
			AnimationPlayer.Play();
		}
	}
}