using Godot;
using System;

public class TargetingComponent : Node
{
    [Export]
    private NodePath RotationNodeNodePath;

    [Export]
    private NodePath ElevationNodeNodePath;

    [Export]
    private float RotationSpeed = 30f;

    [Export]
    private float ElevationSpeed = 20f;

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
        
        // TODO it might be simpler to calculate rotation in the XZ plane

        var targetDirection = Target.GlobalTransform.origin - RotationNode.GlobalTransform.origin;
        var rotationForward = -RotationNode.GlobalTransform.basis.z;

        // If we're within 1 degree that's probably good enough.
        if (rotationForward.AngleTo(targetDirection) <= 0.0174533f) return;
        
        
        var stepTarget = rotationForward.LinearInterpolate(targetDirection, RotationSpeed * delta);
        RotationNode.Transform =
            RotationNode.Transform.Rotated(RotationNode.Transform.basis.y, rotationForward.AngleTo(stepTarget));
    }
}
