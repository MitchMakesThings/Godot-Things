using Godot;
using System;
using BloodHarvest.scripts;
using BloodHarvest.scripts.characters;
using BloodHarvest.scripts.helpers;
using BloodHarvest.scripts.interfaces;

public partial class PlayerCharacter : CharacterBody3D, ITakeDamage
{
	const float WalkSpeed = 5f;
	const float RunSpeed = 10f;
	const float JumpVelocity = 4.5f;
	
	const float MouseSensitivity = 0.1f;
	const float HeadBobFrequency = 2f;
	const float HeadBobAmplitude = 0.08f;

	const float FOVModifier = .2f;
	private float _FOVdefault;
	
	double _headBobTime = 0f;

	[Export]
	private AudioStream JumpSound;

	[Export]
	private AudioStream DamageSound;

	[Export]
	private AudioStreamPlayer Audio;

	[Export]
	protected Node3D Head = null!; // Assigned in editor

	[Export]
	protected Camera3D Camera = null!; // Assigned in editor

	[Export]
	protected ShapeCast3D InteractionRayCast = null!; // Assigned in editor

	[Export]
	protected Node3D WeaponAttach = null!;

	[Export]
	protected RayCast3D HitScan = null!;

	[Export]
	protected Node3D WeaponAttachmentPoint = null!;

	[Signal]
	public delegate void GunChangedEventHandler();
	private Pistol? _gun;
	public Pistol? Gun
	{
		get => _gun;
		private set
		{
			_gun = value;
			if (_gun is not null)
			{
				_gun.Wielder = this;
				_gun.Visible = true;
				EmitSignal(SignalName.GunChanged);
			}
		}
	}

	[Signal]
	public delegate void ReloadEventHandler();
	
	public bool IsAimingDownSights { get; private set; }

	[Export]
	private Timer WeaponCooldown;
	
	[Export]
	private PackedScene SparkAnimationScene { get; set; }
	
	[Signal]
	public delegate void EnemyKilledEventHandler();

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Ready()
	{
		base._Ready();
		
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_FOVdefault = Camera.Fov;

		CameraShake.Instance.RegisterCamera(Camera);

		AddToGroup(Groups.Players);
		AddToGroup(Groups.Attackable);

		Health = MaxHealth;
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		CameraShake.Instance.UnregisterCamera();
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("reload") && Gun is not null && !Gun.IsReloading)
		{
			Gun.Reload();
			EmitSignal(SignalName.Reload);
		}
		
		HandleWeapon(delta);
		HandleMovement(delta);
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		
		// Handle camera rotation
		if (@event is InputEventMouseMotion mouseMotion) {
			HandleCameraRotation(mouseMotion);
		}
	}

	#region Movement
	private void HandleCameraRotation(InputEventMouseMotion mouseMotion)
	{
		// Rotate up / down
		Head.RotateX(Mathf.DegToRad(-mouseMotion.Relative.Y * MouseSensitivity));
		Head.RotationDegrees = new Vector3(Mathf.Clamp(Head.RotationDegrees.X, -70, 70), Head.RotationDegrees.Y, Head.RotationDegrees.Z);

		// Rotate left / right
		RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * MouseSensitivity));
	}

	private void HandleMovement(double delta)
	{
		var isOnFloor = IsOnFloor();
		
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!isOnFloor)
			velocity.Y -= _gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && isOnFloor)
		{
			velocity.Y = JumpVelocity;
			Audio.Stream = JumpSound;
			Audio.Play();
		}

		var speed = WalkSpeed;
		if (Input.IsActionPressed("sprint") && isOnFloor)
		{
			speed = RunSpeed;
		}

		// Get the input direction and handle the movement/deceleration.
		Vector2 inputDir = Input.GetVector("movement_left", "movement_right", "movement_forward", "movement_back");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (isOnFloor)
		{
			if (direction != Vector3.Zero)
			{
				velocity.X = direction.X * speed;
				velocity.Z = direction.Z * speed;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, speed);
			}
		}
		else
		{
			// Air movement deceleration. We don't want to stop horizontal/forward movement as soon as the player lets go.
			// Using MoveToward instead of Lerp gives less control. So jumping is a commitment.
			// Simply swapping MoveToward for Lerp would give you enough control to change direction, with the current delta amount.
			// (ie, jump forward then press the back button. With lerp you'll actually start going backwards mid jump. MoveToward will keep going forward)
			velocity.X = Mathf.MoveToward(Velocity.X, direction.X * speed, (float)delta * 2f);
			velocity.Z = Mathf.MoveToward(Velocity.Z, direction.Z * speed, (float)delta * 2f);
		}

		var velocityLength = velocity.Length();

		// Head bob
		if (isOnFloor)
		{
			_headBobTime += delta * velocityLength;
		}
		Camera.Position = CalculateHeadBobPosition();
		
		// FOV
		var targetFov = _FOVdefault + (FOVModifier * Mathf.Clamp(velocityLength, 0, RunSpeed * 2f));
		Camera.Fov = Mathf.Lerp(Camera.Fov, targetFov, .2f);

		Velocity = velocity;
		MoveAndSlide();
	}

	Vector3 CalculateHeadBobPosition()
	{
		// Taken from juicy player controller video - https://www.youtube.com/watch?v=A3HLeyaBCq4
		var position = Vector3.Zero;
		position.Y = (float)Mathf.Sin(_headBobTime * HeadBobFrequency) * HeadBobAmplitude;
		position.X = (float)Mathf.Cos(_headBobTime * HeadBobFrequency / 2) * HeadBobAmplitude;
		return position;
	}
	#endregion

	#region Weapons

	private void HandleWeapon(double delta)
	{
		if (Gun is null)
		{
			// Weapons are actually optional here...
			return;
		}
		
		if (Input.IsActionPressed("ads") && Gun.CanShoot)
		{
			IsAimingDownSights = true;
			Gun.Node.Position = Gun.Node.Position.MoveToward(Gun.ADSPosition, .1f);
		}
		else
		{
			IsAimingDownSights = false;
			Gun.Node.Position = Gun.Node.Position.MoveToward(Gun.NormalPosition, .1f);
		}

		if (Input.IsActionPressed("shoot"))
		{
			if (!WeaponCooldown.IsStopped()) return;
			if (Gun.Ammo <= 0)
			{
				// Automatically reload on dry-fire
				Gun.Reload();
				return;
			}

			if (!Gun.CanShoot) return;

			Gun.Fire();

			WeaponCooldown.Start(Gun.Cooldown);

			FireShot(Gun.ShotCount);
			
			// Enemy knockback only happens once per attack
			if (!HitScan.IsColliding()) return;
			var victim = HitScan.GetCollider() as BaseEnemy;
			if (victim is not null)
			{
				victim.ApplyKnockback(Transform.Basis * Vector3.Forward * Gun.EnemyKnockback);
			}
		}
	}

	private void FireShot(int i)
	{
		if (i <= 0) return;
		if (Gun is null || Gun.Ammo <= 0 || !Gun.CanShoot) return;

		var playerKnockback = Gun.PlayerKnockback;
		if (!IsAimingDownSights)
		{
			// Randomize shot a bit if not aiming down sights
			// var randomX = RandomInRange(-Gun.ShotSpread, Gun.ShotSpread);
			// var randomY = RandomInRange(-Gun.ShotSpread, Gun.ShotSpread);
			// HitScan.TargetPosition = new Vector3(randomX, randomY, HitScan.TargetPosition.Z);
			// HitScan.ForceRaycastUpdate();
		}
		else
		{
			playerKnockback *= 0.75f; // ADS reduces knockback by 25%
		}

		// Player knockback
		// Only applying when on the floor so that we don't rocket-launch ourselves.
		if (IsOnFloor())
		{
			Velocity += Transform.Basis * (Vector3.Back * playerKnockback);
		}

		// Camera shake
		CameraShake.Instance.AddTrauma(Gun.ShotCameraTrauma); // TODO Modify by ADS?

		if (!HitScan.IsColliding()) return;

		var globalHitPoint = HitScan.GetCollisionPoint();

		var victim = HitScan.GetCollider() as ITakeDamage;
		if (victim is null)
		{
			// Non-enemy hit, so show some sparks
			var hitAnimation = SparkAnimationScene.Instantiate<Node3D>();
			GetTree().Root.AddChild(hitAnimation);
			hitAnimation.GlobalPosition = globalHitPoint;
			
			return;
		}
				
		victim.TakeDamage(Gun.CalculateDamage(), globalHitPoint);
		// TODO enemy knockback

		if (Gun.DelayBetweenShots > 0)
		{
			GetTree().CreateTimer(Gun.DelayBetweenShots, processInPhysics: true).Timeout += () => FireShot(i - 1);
		}
		else
		{
			FireShot(i - 1);
		}
	}

	public void EquipWeapon(Pistol weapon)
	{
		foreach (var existingWeapon in WeaponAttachmentPoint.GetChildren())
		{
			existingWeapon.QueueFree();
		}
		WeaponAttachmentPoint.AddChild(weapon);
		Gun = weapon;
	}

	private float RandomInRange(float min, float max)
	{
		return (float)Random.Shared.NextDouble() * (max - min) + min;
	}
	#endregion
	
	#region ITakeDamage
	
	[Signal]
	public delegate void TookDamageEventHandler(int amount, int remainingHealth);

	[Signal]
	public delegate void HealedEventHandler(int amount, int newHealth);

	private int MaxHealth = 100;
	public int Health { get; private set; }
	public bool IsDead => Health <= 0;
	
	public bool TakeDamage(int amount, Vector3? globalHitLocation)
	{
		Health -= amount;

		Audio.Stream = DamageSound;
		Audio.Play();
		
		CameraShake.Instance.AddTrauma(1);
		EmitSignal(SignalName.TookDamage, amount, Health);
		
		if (Health <= 0)
		{
			SetProcess(false);
			SetPhysicsProcess(false);
			GetTree().CreateTimer(1).Timeout += GameState.Instance.GameOver;
			return true;
		}

		return false;
	}

	public void Heal(int amount)
	{
		Health += amount;
		if (Health > MaxHealth)
		{
			Health = MaxHealth;
			return;
		}

		EmitSignal(SignalName.Healed, amount, Health);
	}
	#endregion
}
