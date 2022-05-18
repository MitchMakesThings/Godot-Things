using CyberUnderground.Entities;
using Godot;

namespace CyberUnderground.Core
{
    public class CoreSystem : Node
    {
        private float _timeSinceTick;

        private bool _isTicking = false;

        [Export]
        public float TimePerTick = 20f;

        [Signal]
        public delegate void OnTick();

        public readonly EntityManager EntityManager = new EntityManager();
        public ObjectiveManager ObjectiveManager { get; private set; }
        public AudioManager AudioManager { get; private set; }

        public float TickPercentage => (100f / TimePerTick) * _timeSinceTick;

        private int _alertLevel = 0;

        [Signal]
        public delegate void OnAlertLevelUpdated(int newLevel);

        [Signal]
        public delegate void OnObjectivesUpdated();

        public override void _Ready()
        {
            base._Ready();

            ObjectiveManager = new ObjectiveManager(this, EntityManager);
            ObjectiveManager.GenerateRandomObjectives();

            AudioManager = GetNode<AudioManager>("/root/AudioManager");

            Connect(nameof(OnAlertLevelUpdated), this, nameof(AlertLevelChanged));
            Connect(nameof(OnTick), this, nameof(Tick));
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (_isTicking)
            {
                _timeSinceTick += delta;
            }

            if (_timeSinceTick >= TimePerTick)
            {
                _timeSinceTick = 0f;

                EmitSignal(nameof(OnTick));
            }
        }

        public void RaiseAlertLevel()
        {
            _alertLevel++;
            _isTicking = _alertLevel > 0;

            EmitSignal(nameof(OnAlertLevelUpdated), _alertLevel);
        }

        public void LowerAlertLevel()
        {
            _alertLevel--;
            _isTicking = _alertLevel <= 0;

            EmitSignal(nameof(OnAlertLevelUpdated), _alertLevel);
        }

        private void AlertLevelChanged(int alertLevel)
        {
            GetNode<AnimationPlayer>("AnimationPlayer").Play("alertchanged");
        }

        private void Tick()
        {
            RaiseAlertLevel();
        }
    }
}