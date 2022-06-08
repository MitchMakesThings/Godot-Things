using Godot;
using System;

public class TargetingComponent : Node
{
    [Export]
    private NodePath RotationNodeNodePath;

    [Export]
    private NodePath ElevationNodeNodePath;

    [Export]
    private float RotationSpeed = 1f;

    [Export]
    private float ElevationSpeed = 1f;

    private Spatial RotationNode;
    private Spatial ElevationNode;
    private Spatial Target;
    
    public override void _Ready()
    {
        RotationNode = GetNode<Spatial>(RotationNodeNodePath);
        ElevationNode = GetNode<Spatial>(ElevationNodeNodePath);
    }

    /// <summary>
    /// Assign the target that the TargetingComponent should aim at.
    /// </summary>
    /// <param name="target">Spatial target, or null to clear targeting</param>
    public void SetTarget(Spatial target)
    {
        Target = target;
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        if (Target is null) return;

        if (RotationNode != null)
        {
            Pivot(RotationNode, Vector3.Up, RotationSpeed, delta);
        }

        if (ElevationNode != null)
        {
            Pivot(ElevationNode, Vector3.Right, ElevationSpeed, delta);
        }
    }

    private void Pivot(Spatial pivotNode, Vector3 axis, float speed, float delta)
    {
        var current = new Quat(pivotNode.Transform.basis);
        var target = new Quat(pivotNode.GlobalTransform.LookingAt(Target.GlobalTransform.origin, Vector3.Up).basis);
        
        // Figure out how far we should travel between the current and target rotations
        var step = current.Slerp(target, speed * delta);
        
        // Simplify down to only the axis we want to rotate around.
        step = new Quat(axis * step.GetEuler());

        pivotNode.Transform = new Transform(new Basis(step), pivotNode.Transform.origin);
        pivotNode.Orthonormalize(); // Make sure floating-point accumulation doesn't drive things crazy!
    }
}
