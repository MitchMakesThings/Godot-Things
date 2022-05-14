using System.Data.SqlTypes;
using CyberUnderground.Entities.Tools;
using Godot;

namespace CyberUnderground.Entities.Mobs
{
    public class Scanner : Tool
    {
        private Entity _targetEntity;

        [Export]
        private float _movementSpeed = 100f;

        public override void _Ready()
        {
            base._Ready();

            Area2D.Connect("area_entered", this, nameof(OnAreaEntered));
            
            IdentifyTarget();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            if (IsWorking || _targetEntity is null) return;

            GlobalPosition = GlobalPosition.MoveToward(_targetEntity.GlobalPosition, _movementSpeed * delta);
        }

        private void IdentifyTarget()
        {
            _targetEntity = System.EntityManager.GetRandomEntity( new []{ this, _targetEntity});
            foreach (var area in Area2D.GetOverlappingAreas())
            {
                var areaEntity = (area as Area2D)?.GetParent<Entity>();
                if (areaEntity == _targetEntity)
                {
                    ActivateTool(_targetEntity);
                    break;
                }
            }
        }

        public void OnAreaEntered(Area2D area)
        {
            var areaOwner = area.GetParentOrNull<Entity>();
            if (areaOwner != _targetEntity) return;
            
            ActivateTool(areaOwner);
        }

        protected override void ToolFinished()
        {
            base.ToolFinished();

            IdentifyTarget();
        }
    }
}
