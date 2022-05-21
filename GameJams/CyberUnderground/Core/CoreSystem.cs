using System.Collections.Generic;
using System.Linq;
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

        [Export]
        private NodePath _shaderRect;

        private Material _shaderMaterial;

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

        [Signal]
        public delegate void OnGameEnded(int fundsEarned);

        private List<Color> colors = new List<Color>()
        {
            new Color("0a3c3a"),
            new Color("173c0a"),
            new Color("3c2c0a"),
            new Color("3c260a"),
            new Color("3c0a0a")
        };

        public override void _Ready()
        {
            base._Ready();

            ObjectiveManager = new ObjectiveManager(this, EntityManager);

            AudioManager = GetNode<AudioManager>("/root/AudioManager");
            
            _shaderMaterial = GetNode<ColorRect>(_shaderRect).Material;

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

        public void Disconnect(bool playerControlled)
        {
            GD.Print("Disconnect");
            var wonFunds = ObjectiveManager.GetObjectives().Where(o => o.Complete).Sum(o => o.Value);
            
            EmitSignal(nameof(OnGameEnded), wonFunds);
            
            Reset();
        }

        private void Reset()
        {
            ObjectiveManager.Clear();

            _timeSinceTick = 0f;
            _isTicking = false;
            _alertLevel = 0;

            ChangeAlertShader();
        }

        public void RaiseAlertLevel()
        {
            _alertLevel++;
            _isTicking = _alertLevel > 0;

            if (_alertLevel >= 5)
            {
                Disconnect(false);
            }

            ChangeAlertShader();

            EmitSignal(nameof(OnAlertLevelUpdated), _alertLevel);
        }

        public void LowerAlertLevel()
        {
            _alertLevel--;
            _isTicking = _alertLevel <= 0;

            ChangeAlertShader();

            EmitSignal(nameof(OnAlertLevelUpdated), _alertLevel);
        }

        private void ChangeAlertShader()
        {
            if (_alertLevel >= 5) return;
            _shaderMaterial.Set("shader_param/grid_color", colors[_alertLevel]);
            _shaderMaterial.Set("shader_param/speed_scale", 1 + _alertLevel);
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