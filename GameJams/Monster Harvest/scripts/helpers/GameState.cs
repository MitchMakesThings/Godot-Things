using System;
using Godot;

public enum Environment
{
    Day,
    Night
}

public partial class GameState : Node
{
    public static GameState Instance { get; private set; } = null!;

    [Signal]
    public delegate void ChunksUpdatedEventHandler();

    [Signal]
    public delegate void EnvironmentChangedEventHandler(); // Annoyingly we can't pass an enum arg, since it's not a variant.

    [Export]
    public PackedScene ChunkRewardScene { get; private set; } = null!;

    [Export]
    public PackedScene GameOverScene { get; private set; } = null!; // Set in editor

    [Export]
    private Timer NightCountdown { get; set; } = null!;

    [Export]
    private Timer DayCountdownTimer { get; set; } = null!;

    [Export]
    private AudioStreamPlayer RejectSound;

    [Signal]
    public delegate void RejectEventHandler();

    public Environment CurrentEnvironment { get; private set; }= Environment.Day;

    public double TimerCountdown => CurrentEnvironment switch
    {
        Environment.Day => NightCountdown.TimeLeft,
        Environment.Night => DayCountdownTimer.TimeLeft,
        _ => throw new ArgumentOutOfRangeException()
    };

    [Export]
    private PackedScene DayEnvironment;

    [Export]
    private PackedScene NightEnvironment;

    [Export]
    private Node3D EnvironmentParent;

    private WaveHelper _waveHelper;

    public WaveHelper WaveHelper
    {
        get => _waveHelper;
        set
        {
            _waveHelper = value;

            WaveHelper.WaveFinished += OnDayStart;
            WaveHelper.MonsterKilled += () => Kills++;
            
            SwitchEnvironment(Environment.Day);
        }
    }

    [Signal]
    public delegate void NightfallEventHandler();

    public PickupSpawner? PickupSpawner { get; set; }

    [Signal]
    public delegate void ResourceNameChangedEventHandler();

    private string _resourceName = "Mushrooms";

    public string ResourceName
    {
        get => _resourceName;
        set
        {
            _resourceName = value;
            EmitSignal(SignalName.ResourceNameChanged);
        }
    }

    private int _kills;

    public int Kills
    {
        get => _kills;
        set
        {
            _kills = value;
            // TODO signal? I think for now we only care about showing this on the game over screen
        }
    }

    public int ResourcesCollected { get; private set; }
    private int _resource;

    public int Resource
    {
        get => _resource;
        private set
        {
            _resource = value;
            EmitSignal(SignalName.ChunksUpdated);
        }
    }

    [Export]
    private AnimationPlayer Animation;

    [Export]
    private CanvasLayer timeWarpEffect;

    public override void _Ready()
    {
        base._Ready();

        NightCountdown.Timeout += OnNightStart;
        DayCountdownTimer.Timeout += OnDayStart;
    }

    public override void _EnterTree()
    {
        base._EnterTree();

        Instance = this;
    }

    public void CleanState()
    {
        Resource = 3000;
        Kills = 0;
        ResourcesCollected = 0;
        OnDayStart();
    }

    public void OnNightStart()
    {
        EmitSignal(SignalName.Nightfall);
        
        WaveHelper.StartWave();
        
        Animation.Play("nightfall");

        // TODO transform all mushroom pickups into monsters
        // TODO nighttime transition animation
        SwitchEnvironment(Environment.Night);
    }

    public void OnDayStart()
    {
        // TODO transform all monsters into mushroom pickups? Probably better if they just die in the sunrise
        // TODO daytime transition animation
        if (PickupSpawner is not null)
        {
            GetTree().CreateTimer(1).Timeout += SpawnPickup;
        }
        
        Animation.Play("daybreak");

        SwitchEnvironment(Environment.Day);
    }

    private void SpawnPickup()
    {
        PickupSpawner?.SpawnRandom();
        if (NightCountdown.TimeLeft > 3)
        {
            GetTree().CreateTimer(3).Timeout += SpawnPickup;
        }
    }

    private void SwitchEnvironment(Environment environment)
    {
        foreach (var envNode in EnvironmentParent.GetChildren())
        {
            envNode.QueueFree();
        }

        NightCountdown.Stop();
        DayCountdownTimer.Stop();

        switch (environment)
        {
            case Environment.Day:
                var day = DayEnvironment.Instantiate<Node3D>();
                EnvironmentParent.AddChild(day);

                NightCountdown.Start();
                break;
            case Environment.Night:
                var night = NightEnvironment.Instantiate<Node3D>();
                EnvironmentParent.AddChild(night);

                DayCountdownTimer.Start();
                break;
        }

        CurrentEnvironment = environment;
        EmitSignal(SignalName.EnvironmentChanged);
    }

    public void AddResource()
    {
        Resource += 1;
        ResourcesCollected++;
    }

    public void RemoveResource(int number)
    {
        Resource -= number;
    }

    public void SpawnReward(Vector3 globalPosition)
    {
        var reward = ChunkRewardScene.Instantiate<MonsterChunk>();
        AddChild(reward);
        reward.GlobalPosition = globalPosition;

        reward.ApplyImpulse(Vector3.Up * 5);
    }

    public void GameOver()
    {
        GetTree().ChangeSceneToPacked(GameOverScene);
    }

    public void TimeWarp(float lowSpeed, float warpTime)
    {
        Engine.TimeScale = lowSpeed;
        
        // Visible warble
        timeWarpEffect.Visible = true;
        
        var timeTween = GetTree().CreateTween();
        timeTween.SetTrans(Tween.TransitionType.Elastic);
        timeTween.TweenProperty(Engine.Singleton, "time_scale", 1, warpTime);
        timeTween.TweenProperty(timeWarpEffect, "visible", false, .1);
    }

    // This would be better in a sound helper file!
    public void PlayRejectSound()
    {
        RejectSound.Play();
        
        // This also causes a wiggle animation
        EmitSignal(SignalName.Reject);
    }
}