using CyberUnderground.Entities.Tools;
using Godot;

namespace CyberUnderground.Entities.Mobs
{
    public class Scanner : Tool
    {
        private Entity _targetEntityInternal;
        protected Entity TargetEntity
        {
            get => _targetEntityInternal;
            set
            {
                _targetEntityInternal = value;
                SetMovementTarget(Vector2.Zero);
            }
        }

        [Export]
        private float _movementSpeed = 100f;

        public override void _Ready()
        {
            base._Ready();

            Area2D.Connect("area_entered", this, nameof(OnAreaEntered));
            
            PickTarget();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            if (IsWorking || !IsInstanceValid(TargetEntity) || TargetEntity.IsQueuedForDeletion()) return;
            if(TargetEntity == null) PickTarget();

            GlobalPosition = GlobalPosition.MoveToward(TargetEntity.GlobalPosition, _movementSpeed * delta);
        }

        private void PickTarget()
        {
            TargetEntity = System.EntityManager.GetRandomEntity( new []{ this, TargetEntity});
            if (TargetEntity == null) return;   // TODO pick area off map to go and despawn

            foreach (var area in Area2D.GetOverlappingAreas())
            {
                var areaEntity = (area as Area2D)?.GetParent<Entity>();
                if (areaEntity == TargetEntity)
                {
                    ActivateTool(TargetEntity);
                    break;
                }
            }
        }

        public void OnAreaEntered(Area2D area)
        {
            var areaOwner = area.GetParentOrNull<Entity>();
            if (areaOwner == null || areaOwner != TargetEntity) return;
            
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
