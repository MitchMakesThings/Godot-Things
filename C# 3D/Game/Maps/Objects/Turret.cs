using Godot;
using System;
using System.Linq;

public class Turret : Spatial
{
    private Spatial _target;
    private TargetingComponent _targetingComponent;

    [Export]
    private NodePath _targetingComponentNodePath;
    
    public override void _Ready()
    {
        // Acquire target
        var playerNodes = GetTree().GetNodesInGroup("player");    // TODO make the group name static somewhere.
        foreach (var node in playerNodes)
        {
            if (node is Character character)
            {
                _target = character;
                break;
            }
        }

        _targetingComponent = GetNode<TargetingComponent>(_targetingComponentNodePath);
        if (_target != null)
        {
            // TODO move this into a _target setter / OnTargetChanged signal
            _targetingComponent.SetTarget(_target);
        }
    }

}
