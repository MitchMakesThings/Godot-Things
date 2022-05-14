using CyberUnderground.Entities;
using Godot;

namespace CyberUnderground.Core
{
    public class CoreSystem : Node
    {
        private float _timeSinceTick = 0f;

        [Export]
        public float TimePerTick = 10f;
    
        [Signal]
        public delegate void OnTick();

        public readonly EntityManager EntityManager = new EntityManager();

        public float TickPercentage => (100f / TimePerTick) * _timeSinceTick;
    
        // TODO include a collection that Entitities can register themselves in.
        // When Tick() happens a signal is sent.
        // When all entities have reported that they've finished their tick action the next tick can begin.
        // Or does movement just _start_ on the tick, and takes some portion of a tick to complete?

        public override void _Ready()
        {
        
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
