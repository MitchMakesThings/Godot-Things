using Godot;
using System;
using BloodHarvest.scripts.interfaces;

public partial class HealthSystem : Node3D
{
    [Export]
    private GpuParticles3D DamageParticles { get; set; } = null!;

    [Export]
    private GpuParticles3D DeathParticles { get; set; } = null!;


    public int Health { get; private set; }

    [Export]
    public int MaxHealth { get; private set; } = 25;

    /// <summary>
    /// How often we should spawn a reward. Eg, 2 = half the time, 3 = one third of the time etc.
    /// Our actual reward calculation is randomised, so you might get a series of bad 'rolls' 
    /// </summary>
    [Export]
    public int RewardOdds { get; private set; } = 2;

    [Signal]
    public delegate void DiedEventHandler();

    public override void _Ready()
    {
        base._Ready();

        Health = MaxHealth;

        // When our death particles die away we'll remove the parent entirely.
        // This should probably be a signal to the parent, but this is one less thing to forget to configure...
        DeathParticles.Finished += () => GetParent().QueueFree();
    }

    public bool TakeDamage(int amount, Vector3? globalHitLocation)
    {
        if (Health <= 0) return false;

        Health -= amount;
        
        // Display blood splash & Damage numbers
        ShowDamage(amount, globalHitLocation);

        if (Health > 0) return false;

        HandleDeath();
        return true;
    }

    private void ShowDamage(int amount, Vector3? globalHitLocation)
    {
        if (globalHitLocation is not null)
        {
            DamageParticles.GlobalPosition = globalHitLocation.Value;
        }
        else
        {
            DamageParticles.GlobalPosition = GlobalPosition;
        }
        DamageParticles.Restart();
        DamageHelper.Instance.ShowDamage(DamageParticles.GlobalPosition, amount);
    }

    private void HandleDeath()
    {
        GetTree().CreateTimer(1).Timeout += () =>
        {
            RandomlySpawnLoot();
            DeathParticles.Restart(); // Show death particles
            RemoveCorpse();
        };

        EmitSignal(SignalName.Died);
    }

    private void RandomlySpawnLoot()
    {
        var spawnChance = Random.Shared.Next(1, RewardOdds + 1); // Adding 1 because the upper bound is exclusive
        if (spawnChance == 1)
        {
            GameState.Instance.SpawnReward(GlobalPosition);
        }
    }

    private void RemoveCorpse()
    {
        // This is tightly coupled to the parent, which ain't great design.
        // It makes it easy for us to drop a HealthSystem onto any enemy though, and then just figure out the animations on the enemy
        foreach (var child in GetParent().GetChildren())
        {
            if (child != this)
            {
                child.QueueFree();
            }
        }
    }

    public void Heal(int amount)
    {
        // TODO healing particles
        Health += amount;
    }
}
