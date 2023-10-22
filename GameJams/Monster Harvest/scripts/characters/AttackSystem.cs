using System;
using BloodHarvest.scripts;
using BloodHarvest.scripts.characters;
using BloodHarvest.scripts.interfaces;
using Godot;

public partial class AttackSystem : Area3D
{
    [Signal]
    public delegate void AttackEventHandler();

    [Export]
    public int DamageAmount { get; set; } = 10;

    [Export]
    public float InitialAttackDelay { get; set; } = 1f;

    [Export]
    public float RepeatAttackDelay { get; set; } = 3;

    private bool _isDisabled { get; set; }

    public override void _Ready()
    {
        base._Ready();
        
        BodyEntered += OnBodyEntered;
    }

    private void OnAttackRequested(Vector3 globalPosition)
    {
        var spaceState = GetWorld3D().DirectSpaceState;
        var query = PhysicsRayQueryParameters3D.Create(GlobalPosition, globalPosition);
        query.CollisionMask = 128; // Collision layer 8, aka Attackable
        
        var result = spaceState.IntersectRay(query);
        if (result is null || result.Count == 0) return;
        
        var collider = (PhysicsBody3D)result["collider"];
        AttackIfValid(collider, globalPosition);
    }

    private void AttackIfValid(Node3D target, Vector3? position = null)
    {
        // For various reasons, C# event handlers are not detached when the godot object is disposed by Godot.
        // So _all_ delegate methods should really check to make sure their object is still valid before executing
        if (NativeInstance == IntPtr.Zero || IsQueuedForDeletion() || target.NativeInstance == IntPtr.Zero) return;

        // If the character has moved away from us we should abort the attack
        if (!GetOverlappingBodies().Contains(target)) return;

        if (_isDisabled) return;

        if (target is not ITakeDamage damagable) return;
        
        if (target is BaseEnemy) return;

        OnAttack(damagable, position);
        GetTree().CreateTimer(RepeatAttackDelay).Timeout += () => AttackIfValid(target);
    }

    private void OnAttack(ITakeDamage target, Vector3? globalPosition = null)
    {
        target.TakeDamage(DamageAmount, globalPosition);
        EmitSignal(SignalName.Attack);
    }

    void OnBodyEntered(Node3D body)
    {
        // Don't attack monsters!
        if (body.IsInGroup(Groups.WaveEnemies)) return;
        
        if (body is ITakeDamage)
        {
            GetTree().CreateTimer(InitialAttackDelay).Timeout += () => AttackIfValid(body);
        }
    }
    
    public void Disable()
    {
        _isDisabled = true;
        BodyEntered -= OnBodyEntered;
    }
}
