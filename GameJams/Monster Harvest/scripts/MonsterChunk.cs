using System;
using Godot;

public partial class MonsterChunk : RigidBody3D
{
    public override void _Ready()
    {
        base._Ready();
        
        RotateZ(Random.Shared.NextSingle());
    }

    public void OnBodyEntered(Node3D body)
    {
        if (body is not PlayerCharacter)
        {
            return;
        }
        
        GameState.Instance.AddResource();
        QueueFree();
    }
}
