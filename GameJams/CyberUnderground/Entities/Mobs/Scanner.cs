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
            
            PickTarget();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            if (IsWorking || !IsInstanceValid(_targetEntity) || _targetEntity.IsQueuedForDeletion()) return;

            GlobalPosition = GlobalPosition.MoveToward(_targetEntity.GlobalPosition, _movementSpeed * delta);
        }

        private void PickTarget()
        {
            _targetEntity?.Disconnect("tree_exiting", this, nameof(AbortTool));
            
            _targetEntity = System.EntityManager.GetRandomEntity( new []{ this, _targetEntity});
            if (_targetEntity == null) return;

            _targetEntity.Connect("tree_exiting", this, nameof(AbortTool));
            
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

        public override void AbortTool()
        {
            base.AbortTool();
            
            PickTarget();
        }

        protected override void ToolFinished()
        {
            base.ToolFinished();

            PickTarget();
        }
    }
}
