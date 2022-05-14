using System.Linq;
using Godot;

namespace CyberUnderground.Entities.Tools
{
    public class Tool : Entity
    {
        private bool _selected = false;

        private float _activationCounter;
        
        [Export]
        protected float ActivationTime = 5f;

        protected bool IsWorking { get; private set; } = false;

        public float ActivationPercentage => (100f / ActivationTime) * _activationCounter;

        private ProgressBar _progressBar;

        public override void _Ready()
        {
            base._Ready();
            Area2D.Connect("input_event", this, nameof(OnArea2DInputEvent));

            // TODO nodepath if we can figure out the inherited-nodepath problem
            _progressBar = GetNode<ProgressBar>("ToolUI/CenterContainer/ProgressBar");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            
            if (!IsWorking) return;
            
            _activationCounter += delta;
            if (ActivationPercentage >= 100)
            {
                ToolFinished();
            }
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            // Follow the mouse
            if (_selected)
            {
                GlobalPosition = GlobalPosition.LinearInterpolate(GetGlobalMousePosition(), 25 * delta);
            }

            UpdateUi();
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            base._UnhandledInput(@event);

            // Handle the mouse-button-up event.
            // This is for the case where the button is released while the Tool is lerping to the mouse position
            // AKA, OnArea2DInputEvent won't be called, because the Tool isn't there at the time.
            if (@event is InputEventMouseButton buttonEvent && !buttonEvent.Pressed)
            {
                DropTool();
            }
        }

        public void OnArea2DInputEvent(Node viewport, InputEvent @event, int shapeIndex)
        {
            if (!(@event is InputEventMouseButton buttonEvent))
            {
                return;
            }

            if (buttonEvent.Pressed && buttonEvent.ButtonIndex == (int)ButtonList.Left)
            {
                PickupTool();
            }
            else
            {
                DropTool();
            }
        }

        protected virtual void UpdateUi()
        {
            if (ActivationPercentage > 0)
            {
                _progressBar.Visible = true;
            }

            _progressBar.Value = ActivationPercentage;
        }

        private void ReleaseTarget()
        {
            _activationCounter = 0;
            IsWorking = false;
            DetachAsAttachment();

            _progressBar.Visible = false;
        }
        
        private void PickupTool()
        {
            _selected = true;

            ReleaseTarget();
        }

        private void DropTool()
        {
            if (!_selected) return;
            _selected = false;

            Entity target = null;
            foreach (var body in Area2D.GetOverlappingAreas())
            {
                if (!(body is Area2D area)) continue;

                target = area.GetParentOrNull<Entity>();
                if (target != null)
                {
                    break;
                }
            }

            if (target != null && CanActivate(target))
            {
                ActivateTool(target);
            }
            // TODO if activation fails do a little shake animation?
            // TODO bounce off to the side of the entity as well
        }

        protected virtual void ActivateTool(Entity target)
        {
            AttachTo(target);

            IsWorking = true;
            _activationCounter = 0;
            _progressBar.Visible = true;
        }

        protected virtual bool CanActivate(Entity target)
        {
            return true;
        }

        protected virtual void ToolFinished()
        {
            ReleaseTarget();
            IsWorking = false;
        }
    }
}