using System;
using BloodHarvest.scripts.interfaces;
using Godot;

namespace BloodHarvest.scripts.characters;

public partial class BaseEnemy : CharacterBody3D, ITakeDamage
{
    [Export]
    private HealthSystem HealthSystem { get; set; } = null!;
    
    [Export]
    protected AnimationTree? AnimationTree { get; set; } = null!;
    
    [Signal]
    public delegate void DiedEventHandler();

    private Vector3? _knockback;
    
    public override void _Ready()
    {
        base._Ready();

        HealthSystem.Died += OnDeath;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        // Set our animation based on how fast we're moving
        AnimationTree.Set("parameters/speed/blend_amount", Mathf.Min(Velocity.Length(), 1f));
        
        // Paranoia kill-box, in case the enemy falls off the map
        if (GlobalPosition.Y > 100 || GlobalPosition.Y < -100) QueueFree();

        if (_knockback is not null)
        {
            Velocity += _knockback.Value;
            MoveAndSlide();
            _knockback = null;
        }
    }

    public void ApplyKnockback(Vector3 knockbackVector)
    {
        _knockback = knockbackVector;
    }


    #region ITakeDamage

    // Called from the loading screen to kill a mushnub & trigger particles etc.
    public bool InstantKill() => TakeDamage(HealthSystem.Health, null);

    public bool IsDead => HealthSystem.Health <= 0;
    
    public bool TakeDamage(int amount, Vector3? globalHitLocation)
    {
        if (NativeInstance == IntPtr.Zero) return true;
        
        // TODO Use signals instead. Don't check return value.
        // We could have monsters notify gamestate when they die, and the gamestate could then emit OnEnemyDied to let the UI update
        var died = HealthSystem.TakeDamage(amount, globalHitLocation);

        if (!died)
        {
            if (AnimationTree?.NativeInstance == IntPtr.Zero) return died;
            AnimationTree?.Set("parameters/damage/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
        }

        return died;
    }

    public void Heal(int amount)
    {
        HealthSystem.Heal(amount);
    }

    private void OnDeath()
    {
        if (NativeInstance == IntPtr.Zero) return;
        
        AnimationTree.Set("parameters/death/blend_amount", 1);
        SetPhysicsProcess(false);

        // Disable collision shapes immediately, so that monsters can walk through corpses / players can shoot through
        foreach (var child in GetChildren())
        {
            if (child is CollisionShape3D collisionShape3D)
            {
                collisionShape3D.Disabled = true;
            }
        }
        
        EmitSignal(SignalName.Died);
    }

    private void OnAttack()
    {
        if (NativeInstance == IntPtr.Zero) return;
        
        AnimationTree.Set("parameters/attack/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
    }

    #endregion
}