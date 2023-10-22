using Godot;

namespace BloodHarvest.scripts.interfaces;

public interface ITakeDamage
{
    /// <summary>
    /// Take an amount of damage.
    /// </summary>
    /// <param name="amount">Damage amount.</param>
    /// <param name="globalHitLocation">The hit location.</param>
    /// <returns>True if this was a killing blow, otherwise false.</returns>
    public bool TakeDamage(int amount, Vector3? globalHitLocation);
    public void Heal(int amount);
    
    public bool IsDead { get; }
}