using CyberUnderground.Entities;
using Godot;

namespace CyberUnderground.Core
{
    public class CoreSystem : Node
    {
        private float _timeSinceTick;

        [Export]
        public float TimePerTick = 10f;
    
        [Signal]
        public delegate void OnTick();

        public readonly EntityManager EntityManager = new EntityManager();
        public ObjectiveManager ObjectiveManager { get; private set; }

        public float TickPercentage => (100f / TimePerTick) * _timeSinceTick;

        public override void _Ready()
        {
            base._Ready();

            ObjectiveManager = new ObjectiveManager(EntityManager);

            ObjectiveManager.GenerateRandomObjectives();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _timeSinceTick += delta;

            if (_timeSinceTick >= TimePerTick)
            {
                _timeSinceTick = 0f;

                EmitSignal(nameof(OnTick));
            }
        }
    }
}
