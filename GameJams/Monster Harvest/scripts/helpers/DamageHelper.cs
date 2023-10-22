using Godot;
using System.Collections.Generic;

public partial class DamageHelper : Node3D
{
    public static DamageHelper Instance { get; private set; } = null!; // Set in _Ready
    
    [Export]
    private Node PoolParent { get; set; } = null!;

    private readonly Queue<BouncyDamage> _availablePool = new();

    public override void _Ready()
    {
        base._Ready();

        Instance = this;

        foreach (var child in PoolParent.GetChildren())
        {
            if (child is BouncyDamage damage)
            {
                _availablePool.Enqueue(damage);
            }
        }
    }

    public void ShowDamage(Vector3 globalPosition, int amount)
    {
        if (_availablePool.TryDequeue(out var damage))
        {
            damage.Launch(globalPosition, amount);
        }
    }

    public void ReturnToPool(BouncyDamage damage)
    {
        _availablePool.Enqueue(damage);
    }
}
