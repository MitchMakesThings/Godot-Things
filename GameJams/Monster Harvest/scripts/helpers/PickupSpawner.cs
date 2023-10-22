using Godot;
using System;

public partial class PickupSpawner : Node3D
{
    [Export]
    private PackedScene PickupScene;

    [Export]
    private Shape3D QueryShape;

    public override void _Ready()
    {
        base._Ready();

        GameState.Instance.PickupSpawner = this;
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        GameState.Instance.PickupSpawner = null;
    }

    public void SpawnRandom()
    {
        Vector3? spawnPosition = null;

        // Make ten attempts at finding a safe spawn position. If we can't it's time to just give up!
        for (int i = 0; i < 2; i++)
        {
            var posMarker = GetChildren().PickRandom() as Node3D;

            if (IsSafe(posMarker.GlobalPosition))
            {
                spawnPosition = posMarker.GlobalPosition;
                break;
            }
        }

        if (spawnPosition is null)
        {
            return;
        }

        var pickup = PickupScene.Instantiate<Node3D>();
        GetParent().AddChild(pickup);
        pickup.GlobalPosition = spawnPosition.Value;
    }

    bool IsSafe(Vector3 position)
    {
        var spaceState = GetWorld3D().DirectSpaceState;
        var query = new PhysicsShapeQueryParameters3D()
        {
            CollideWithBodies = true,
            Shape = QueryShape,
            Transform = new Transform3D(Basis.Identity, position)
        };
        //var query = PhysicsRayQueryParameters3D.Create(GlobalPosition, globalPosition);
        query.CollisionMask = 512; // Collision layer 10, aka Pickups

        var test = spaceState.IntersectShape(query);
        
        if (test.Count == 0) return true;

        // We don't actually care what we collide with - if we collided on this layer a pickup was in the way
        return false;
    }
}
