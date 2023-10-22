using Godot;

namespace BloodHarvest.scripts.interfaces;

public interface IShoot
{
    public Vector3 NormalPosition { get; }
    
    public Vector3 ADSPosition { get; }
    
    public Node3D Node { get; }
    public PlayerCharacter Wielder { get; set; }
    
    /// <summary>
    /// Cooldown between shots.
    /// </summary>
   float Cooldown { get; }

    int ShotCount { get; }
    float ShotSpread { get; }
    public float DelayBetweenShots { get; }

    int Ammo { get; }

    void Fire();
    
    int CalculateDamage();

    void Reload();

}