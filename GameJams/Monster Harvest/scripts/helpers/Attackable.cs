using Godot;
using System;
using BloodHarvest.scripts;
using BloodHarvest.scripts.interfaces;

public partial class Attackable : StaticBody3D, ITakeDamage
{
    [Export]
    private int MaxHealth;
    
    [Export]
    private AudioStreamPlayer3D? _audio;

    private int Health;

    public bool IsDead => Health <= 0;
    
    // Null if this object can't be rebuilt. If populated, we'll show the builder when destroyed.
    public Buildable? Builder { get; set; }

    public override void _Ready()
    {
        base._Ready();

        Health = MaxHealth;

        GameState.Instance.EnvironmentChanged += OnEnvironmentChanged;
        
        AddToGroup(Groups.Attackable);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        
        GameState.Instance.EnvironmentChanged -= OnEnvironmentChanged;
    }

    private void OnEnvironmentChanged()
    {
        // Heal each morning
        if (GameState.Instance.CurrentEnvironment == Environment.Day)
        {
            Health = MaxHealth;
        }
    }

    public bool TakeDamage(int amount, Vector3? globalHitLocation = null)
    {
        Health -= amount;
        
        DamageHelper.Instance.ShowDamage(globalHitLocation ?? GlobalPosition, amount);

        if (Health <= 0)
        {
            // TODO Destruction animation & audio
            if (_audio is not null)
            {
                _audio.Play();
            }
            Builder?.Show();

            // Ensure our collision shapes are turned off before the nav rebuild starts.
            // This hopefully solves a race condition between the nav rebuild & queue free
            foreach (var child in GetChildren())
            {
                if (child is CollisionShape3D collisionShape3D)
                {
                    collisionShape3D.Disabled = true;
                }
            }
            NavigationHelper.Instance.Rebuild();
            QueueFree();
            return true;
        }

        return false;
    }

    public void Heal(int amount)
    {
        throw new NotImplementedException();
    }
}
