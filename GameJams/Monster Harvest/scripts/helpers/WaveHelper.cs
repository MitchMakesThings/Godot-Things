using System;
using System.Linq;
using BloodHarvest.scripts;
using BloodHarvest.scripts.characters;
using Godot;
using Godot.Collections;

public partial class WaveHelper : Node3D
{
    [Export]
    public int WaveNumber { get; set; } = 1;

    [Export]
    public int WaveSize { get; set; } = 5;

    [Export]
    public int WaveSizeStep { get; set; } = 2; // We'll increase this each wave, so it gets progressively harder

    [Export]
    public int BossWaveMultiplier { get; set; } = 3;

    [Export]
    public int ModerateEnemyChance { get; set; } = 10;

    [Export]
    private Array<PackedScene> EasyEnemies { get; set; } = new();

    [Export]
    private Array<PackedScene> ModerateEnemies { get; set; } = new();
    
    [Export]
    private Array<PackedScene> Bosses { get; set; } = new();

    private Array<Node3D> SpawnPoints { get; set; } = new();

    [Signal] // This ain't great. But it's fine for the game jam
    public delegate void MonsterKilledEventHandler();
    
    [Signal]
    public delegate void WaveFinishedEventHandler();
    
    [Signal]
    public delegate void WaveStartedEventHandler(int wave);

    public override void _Ready()
    {
        base._Ready();

        foreach (var node in GetChildren())
        {
            if (node is Node3D spawnPoint)
            {
                SpawnPoints.Add(spawnPoint);
            }
        }

        WaveFinished += OnWaveEnd;

        GameState.Instance.WaveHelper = this;

        GameState.Instance.EnvironmentChanged += OnEnvironmentChanged;
    }

    public void StartWave()
    {
        // Give a little grace period before the start of the wave
        GetTree().CreateTimer(2).Timeout += SpawnEnemies;

        EmitSignal(SignalName.WaveStarted, WaveNumber);
    }

    private async void SpawnEnemies()
    {
        for (int i = 0; i < WaveSize; i++)
        {
            var spawnPoint = SpawnPoints.PickRandom();
            var toSpawn = WaveNumber == 1 // First wave only spawn easy enemies
                ? EasyEnemies.PickRandom() 
                : Random.Shared.Next(1, ModerateEnemyChance + 1) == 1 // After that chance is determined by how many waves in we are
                    ? ModerateEnemies.PickRandom() 
                    : EasyEnemies.PickRandom();
            var randomOffset = new Vector3(Random.Shared.Next(-1, 1), 0, Random.Shared.Next(-1, 1));

            var enemy = toSpawn.Instantiate<BaseEnemy>();
            AddChild(enemy);
            enemy.GlobalPosition = spawnPoint.GlobalPosition + randomOffset;
            enemy.AddToGroup(Groups.WaveEnemies);
            enemy.Died += OnMonsterKilled;

            await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
        }

        var isBossWave = WaveNumber % BossWaveMultiplier == 0;
        if (isBossWave)
        {
            // TODO Add bosses!
        }
    }

    private void OnMonsterKilled()
    {
        if (NativeInstance == IntPtr.Zero) return;

        EmitSignal(SignalName.MonsterKilled);
        var remainingEnemies = GetTree()?.GetNodesInGroup(Groups.WaveEnemies);
        if (remainingEnemies is not null && remainingEnemies.All(e => ((BaseEnemy)e).IsDead))
        {
            EmitSignal(SignalName.WaveFinished);
        }
    }

    public void OnWaveEnd()
    {
        if (NativeInstance == IntPtr.Zero) return;

        GameState.Instance.TimeWarp(.2f, .5f);
        
        WaveSize += WaveSizeStep;
        if (ModerateEnemyChance > 1)
        {
            ModerateEnemyChance--;
        }

        WaveNumber++;
    }

    public void OnEnvironmentChanged()
    {
        if (NativeInstance == IntPtr.Zero) return;
        
        // If the player survives until dawn make sure we wipe out all the remaining monsters
        if (GameState.Instance.CurrentEnvironment == Environment.Day)
        {
            foreach (var monster in GetTree().GetNodesInGroup(Groups.WaveEnemies))
            {
                if (monster is BaseEnemy enemy)
                {
                    enemy.InstantKill();
                }
            }
        }
    }
}
