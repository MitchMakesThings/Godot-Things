using Godot;
using System;
using System.Reflection.Metadata.Ecma335;
using BloodHarvest.scripts.interfaces;

public partial class Pistol : Node3D, IShoot
{
    [Export]
    private AnimationPlayer _animationPlayer; // Set in editor

    [Export]
    public Vector3 NormalPosition { get; protected set; }

    [Export]
    public Vector3 ADSPosition { get; protected set; }
    
    [Export]
    private MeshInstance3D Mesh { get; set; }

    public Node3D Node => this;
    public PlayerCharacter Wielder { get; set; }
    
    #region Damage
    // TODO move to IShoot, or parent class. A resource would even make a lot of sense!
    [Export]
    private int MaxDamage { get; set; } = 13;
    [Export]
    private int MinDamage { get; set; } = 8;
    [Export]
    private double ADSModifier { get; set; } = 1.2;
    #endregion

    [Export]
    public float Cooldown { get; set; } = .3f;
    
    [Export]
    public int ShotCount { get; set; } = 1;
    
    [Export]
    public float ShotSpread { get; set; } = .1f;

    [Export]
    public float DelayBetweenShots { get; set; } = 0f;
    
    [Export]
    public int ClipSize { get; set; }
    
    [Export]
    public int TotalAmmo { get; set; }
    
    public int Ammo { get; set; }
    
    [Export]
    private float ReloadSpeed { get; set; }

    [Signal]
    public delegate void AmmoChangedEventHandler(int newAmmo, int newTotal);

    [Export]
    public float PlayerKnockback { get; private set; } = 10f;
    
    [Export]
    public float EnemyKnockback { get; private set; }

    [Export]
    public float ShotCameraTrauma { get; private set; } = 1;

    [Export]
    private AudioStream? _outOfAmmoAudio;

    [Export]
    private AudioStream? _reloadAudio;

    [Export]
    private AudioStream? _fireAudio;

    [Export]
    private AudioStreamPlayer _audioStreamPlayer;

    private GpuParticles3D? Particles { get; set; }
    private Light3D? MuzzleFlash { get; set; }

    public bool CanShoot
    {
        get
        {
            if (IsReloading) return false;

            if (Ammo <= 0 && _outOfAmmoAudio is not null)
            {
                _audioStreamPlayer.Stream = _outOfAmmoAudio;
                _audioStreamPlayer.Play();
            }
            
            return Ammo > 0;
        }
    }

    public bool IsReloading { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        Ammo = ClipSize;

        foreach (var child in GetChildren())
        {
            if (child is GpuParticles3D particles)
            {
                Particles = particles;
            }

            if (child is Light3D light)
            {
                light.Visible = false;
                MuzzleFlash = light;
            }
        }
    }

    public int CalculateDamage()
    {
        var damageAmount = Random.Shared.Next(MinDamage, MaxDamage);
        if (Wielder.IsAimingDownSights)
        {
            damageAmount = (int)(damageAmount * ADSModifier);
        }

        return damageAmount;
    }

    public void Reload()
    {
        if (IsReloading) return;
        IsReloading = true;

        if (_reloadAudio is not null && TotalAmmo > 0)
        {
            _audioStreamPlayer.Stream = _reloadAudio;
            _audioStreamPlayer.Play();
        }

        if (_outOfAmmoAudio is not null && TotalAmmo <= 0)
        {
            _audioStreamPlayer.Stream = _outOfAmmoAudio;
            _audioStreamPlayer.Play();
        }
        
        var desiredAmmo = Mathf.Min(ClipSize, TotalAmmo);
        TotalAmmo -= desiredAmmo - Ammo; // Don't drop ammo from the clip being swapped. This is quality of life for chronic reloaders.
        Ammo = desiredAmmo;
        EmitSignal(SignalName.AmmoChanged, Ammo, TotalAmmo);

        var defaultRotation = Mesh.RotationDegrees;
        var tween = GetTree().CreateTween();
        tween.SetTrans(Tween.TransitionType.Cubic);
        tween.TweenProperty(Mesh, "rotation_degrees", new Vector3(-15, defaultRotation.Y, defaultRotation.Z), 0.1);
        tween.TweenProperty(Mesh, "rotation_degrees", new Vector3(0, defaultRotation.Y, -360), ReloadSpeed);
        tween.TweenCallback(Callable.From(() =>
        {
            Mesh.RotationDegrees = defaultRotation;
            return IsReloading = false;
        }));
    }

    public void Fire()
    {
        if (Particles is not null)
        {
            Particles.Emitting = true;
        }

        if (MuzzleFlash is not null)
        {
            MuzzleFlash.Visible = true;
            GetTree().CreateTimer(.1).Timeout += () => MuzzleFlash.Visible = false;
        }
        _animationPlayer.Stop();
        _animationPlayer.Play("recoil");

        if (_fireAudio is not null)
        {
            _audioStreamPlayer.Stream = _fireAudio;
            _audioStreamPlayer.Play();
        }
        
        Ammo -= ShotCount;
        if (Ammo < 0) Ammo = 0;
        EmitSignal(SignalName.AmmoChanged, Ammo, TotalAmmo);
    }
}