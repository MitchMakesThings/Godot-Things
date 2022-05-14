using CyberUnderground.Core;
using Godot;

namespace CyberUnderground.Entities
{
    public class Entity : Node2D
    {
        protected Area2D Area2D { get; private set; }
        protected CoreSystem System { get; private set; }
        
        public override void _Ready()
        {
            base._Ready();
            
            // Blurgh, but there might be a regression causing [Export] nodepaths to be empty in inherited scenes...
            // https://github.com/godotengine/godot/issues/36480 was my original report
            Area2D = GetNode<Area2D>("Area2D");
            
            System = GetNode<CoreSystem>("/root/System");
            System.Connect(nameof(CoreSystem.OnTick), this, nameof(OnTick));
        }

        public virtual void OnTick()
        {
        }
    }
}