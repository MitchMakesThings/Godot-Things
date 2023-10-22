using System;
using Godot;
using System.Linq;
using BloodHarvest.scripts;

public partial class MovementSystem : Node3D
{
    [Signal]
    public delegate void AttackRequestEventHandler(Vector3 position);
    
    [Export]
    private NavigationAgent3D Navigation { get; set; } = null!;
    
    [Export]
    private float Speed { get; set; } = 1.0f;

    private CharacterBody3D Parent { get; set; } = null!;

    [Export] // TODO remove after debugging weirdness
    private Node3D? _target;
    private Node3D? Target
    {
        get => _target;
        set
        {
            if (_target is not null)
            {
                _target.TreeExiting -= NullTarget;
            }

            _target = value;
            
            if (_target is not null)
            {
                _target.TreeExiting += NullTarget;
            }
        }
    }

    private Vector3? TargetPosition;

    public override void _Ready()
    {
        base._Ready();

        Parent = GetParent<CharacterBody3D>();
        
        // Fetch a new target whenever we reach the old one
        //Navigation.NavigationFinished += UpdateTarget;
        Navigation.VelocityComputed += OnSafeVelocityComputed;

        Navigation.NavigationFinished += () =>
        {
            EmitSignal(SignalName.AttackRequest, TargetPosition.Value);
            TargetPosition = null;
        };

        GetTree().CreateTimer(1).Timeout += ReacquireTarget;
    }
    
    float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    
    
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        Vector3 velocity = Parent.Velocity;

        if (!Parent.IsOnFloor())
        {
            velocity.Y -= _gravity * (float)delta;
        }

        if (Target is not null)
        {
            TargetPosition = Target.GlobalPosition;
        }
        
        if (TargetPosition is not null)
        {
            Navigation.TargetPosition = TargetPosition.Value;
        }
        
        var nextPoint = Navigation.GetNextPathPosition();
        var direction = nextPoint - GlobalPosition;

        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Parent.Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Parent.Velocity.Z, 0, Speed);
        }

        Navigation.Velocity = velocity;
    }

    public void Disable()
    {
        Navigation.VelocityComputed -= OnSafeVelocityComputed;
    }

    private void ReacquireTarget()
    {
        if (TargetPosition is not null) return;   
        Target = GetTree().GetNodesInGroup(Groups.Players).FirstOrDefault() as Node3D;
    }
    
    private void NullTarget()
    {
        if (NativeInstance == IntPtr.Zero) return;
        _target = null;
        TargetPosition = null;
    }
    
    private void UpdateTarget()
    {
        if (Target is null) ReacquireTarget();
        if (Target is null) return;
        
        var spaceState = GetWorld3D().DirectSpaceState;
        var query = PhysicsRayQueryParameters3D.Create(GlobalPosition, Target.GlobalPosition);
        query.CollisionMask = 128; // Collision layer 8, aka Attackable

        var result = spaceState.IntersectRay(query);
        if (result is null || result.Count == 0) return;

        var collider = (PhysicsBody3D)result["collider"];
        TargetPosition = (Vector3)result["position"];
        Target = null;
    }
    
    private void OnSafeVelocityComputed(Vector3 safeVelocity)
    {
        // Look in the direction we're moving
        var currentAngle = Parent.Rotation.Y;
        var intendedAngle = Mathf.Atan2(safeVelocity.X, safeVelocity.Z);
        Parent.Rotation = Parent.Rotation with { Y = Mathf.RotateToward(currentAngle, intendedAngle, .1f) };

        // Move
        Parent.Velocity = safeVelocity;
        Parent.MoveAndSlide();
    }
}
