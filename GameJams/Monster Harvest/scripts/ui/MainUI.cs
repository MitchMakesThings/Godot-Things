using Godot;
using System;

public partial class MainUI : CanvasLayer
{
    [Export]
    private Label HarvestTotalLabel = null!; // Set in editor

    [Export]
    private Label KillLabel;

    [Export]
    private ColorRect DamageIndicator;

    [Export]
    public PlayerCharacter Player { get; private set; } = null!;

    [Export]
    private Label NightCountdownLabel;

    [Export]
    private Label ResourceLabel;

    [Export]
    private Label AmmoLabel;

    [Export]
    private Label TotalAmmoLabel;

    [Export]
    private TextureProgressBar HealthBar;

    [Export]
    private Container AmmoContainer;

    [Export]
    private Control NightTransitionUI;

    public override void _Ready()
    {
        base._Ready();

        GameState.Instance.ChunksUpdated += OnChunksUpdated;
        OnChunksUpdated(); // Ensure our initial UI is correct
        
        // Initialise the resource name and register to detect changes
        ResourceLabel.Text = GameState.Instance.ResourceName;
        GameState.Instance.ResourceNameChanged += OnResourceNameChanged;

        GameState.Instance.WaveHelper.MonsterKilled += OnMonsterKilled;
        GameState.Instance.Nightfall += OnNightfall;
        GameState.Instance.Reject += OnReject;
        
        Player.GunChanged += OnGunChanged;
        Player.Healed += OnPlayerHealed;
        Player.TookDamage += OnPlayerTakesDamage;
        Player.Reload += OnReload;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        
        GameState.Instance.ChunksUpdated -= OnChunksUpdated;
        GameState.Instance.ResourceNameChanged -= OnResourceNameChanged;
        
        GameState.Instance.WaveHelper.MonsterKilled -= OnMonsterKilled;
        GameState.Instance.Nightfall -= OnNightfall;
        GameState.Instance.Reject -= OnReject;
        
        Player.GunChanged -= OnGunChanged;
        Player.Healed -= OnPlayerHealed;
        Player.TookDamage -= OnPlayerTakesDamage;
        Player.Reload -= OnReload;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (GameState.Instance.CurrentEnvironment == Environment.Day && Mathf.IsEqualApprox(Engine.TimeScale, 1))
        {
            NightCountdownLabel.Text = $"Nightfall in {GameState.Instance.TimerCountdown:F1} seconds";
        }
    }

    private void OnReject()
    {
        var tween = GetTree().CreateTween();
        tween.SetTrans(Tween.TransitionType.Elastic);
        tween.TweenProperty(HarvestTotalLabel, "scale", Vector2.One * 3, .25);
        tween.TweenProperty(HarvestTotalLabel, "scale", Vector2.One, .1);
    }

    private void OnResourceNameChanged()
    {
        ResourceLabel.Text = GameState.Instance.ResourceName;
    }

    private void OnMonsterKilled()
    {
        KillLabel.Visible = true;
        GetTree().CreateTimer(0.1).Timeout += () =>
        {
            KillLabel.Visible = false;
        };
    }

    private void OnGunChanged()
    {
        OnAmmoChanged(Player.Gun.Ammo, Player.Gun.TotalAmmo);
        Player.Gun.AmmoChanged += OnAmmoChanged;
    }

    private void OnChunksUpdated()
    {
        HarvestTotalLabel.Text = GameState.Instance.Resource.ToString();
    }

    public void OnPlayerTakesDamage(int amount, int remainingHealth)
    {
        // Flash red on screen
        var tween = GetTree().CreateTween();
        tween.SetTrans(Tween.TransitionType.Elastic);
        tween.TweenProperty(DamageIndicator, "color", new Color("8200008e"), .1);
        if (remainingHealth > 0)
        {
            tween.TweenProperty(DamageIndicator, "color", new Color("ffffff00"), .2);
        }
        
        WiggleHealthBar();

        HealthBar.Value = remainingHealth;
    }

    public void OnPlayerHealed(int amount, int newHealth)
    {
        var scale = HealthBar.Scale;
        var tween = GetTree().CreateTween();
        tween.SetTrans(Tween.TransitionType.Bounce);
        tween.TweenProperty(HealthBar, "scale", new Vector2(scale.X, scale.Y * 1.6f), .2);
        tween.SetTrans(Tween.TransitionType.Cubic);
        tween.TweenProperty(HealthBar, "scale", scale, .1);
        
        HealthBar.Value = newHealth;
    }

    private void WiggleHealthBar()
    {
        var tween = GetTree().CreateTween();
        tween.SetTrans(Tween.TransitionType.Elastic);
        tween.TweenProperty(HealthBar, "rotation_degrees", 1.5, .05);
        tween.TweenProperty(HealthBar, "rotation_degrees", -1.5, .05);
        tween.TweenProperty(HealthBar, "rotation_degrees", 0, .025);
    }

    private void OnNightfall()
    {
        NightCountdownLabel.Text = ""; // Clear off the text for the night
        
        NightTransitionUI.GetChild<Label>(0).Text = $"Night {GameState.Instance.WaveHelper.WaveNumber}";
        var tween = GetTree().CreateTween();
        tween.SetTrans(Tween.TransitionType.Bounce);
        tween.TweenProperty(NightTransitionUI, "position", Vector2.Zero, .25);
        tween.TweenInterval(1);
        tween.SetTrans(Tween.TransitionType.Elastic);
        tween.TweenProperty(NightTransitionUI, "position", new Vector2(0, -1000), .25);
    }

    void OnAmmoChanged(int newAmmo, int newTotal)
    {
        if (Player.Gun is null || Player.Gun.NativeInstance == IntPtr.Zero) return;

        AmmoLabel.Text = newAmmo.ToString();
        TotalAmmoLabel.Text = newTotal.ToString();
    }

    void OnReload()
    {
        var ammoPos = AmmoContainer.Position;
        
        var tween = GetTree().CreateTween();
        tween.SetTrans(Tween.TransitionType.Expo);
        tween.TweenProperty(AmmoContainer, "position", new Vector2(ammoPos.X, ammoPos.Y + 100), .2);
        tween.TweenProperty(AmmoContainer, "position", ammoPos, .2);
    }
}
