using Godot;
using System;

public class WinArea : Spatial
{

	// Called from the Area signal being raised
	private void _on_Area_body_entered(object body)
	{
		Character character = body as Character;

		if (character != null) {
			// There are a couple of ways this could go
			// We could raise a signal of our own, and the GameMode could try hunting down all WinArea's to Connect to the signals
			// That might be cleaner, but I'm taking the quick way out - we'll just reach out and call a method on the GameMode
			// It violates the whole "call methods down to children, raise signals up the tree" thing, but meh.
			TestGameMode.Instance.EnteredWinArea(this, character);

			// TODO instead of hardcoding, this could call a Group method!
			// That way the GameMode can register itself as part of the Group, and anyone can call up to it directly
		}
	}

}
