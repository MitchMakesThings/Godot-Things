using Godot;

namespace CyberUnderground.Entities.Tools
{
    public class Tool : Entity
    {
        [Export]
        public bool IsPlayerControlled { get; protected set; } = true;
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

            if (!IsPlayerControlled) return;

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
            if (!IsPlayerControlled) return;
            if (!(@event is InputEventMouseButton buttonEvent))
            {
                return;
            }

            if (buttonEvent.Pressed && buttonEvent.ButtonIndex == (int)ButtonList.Left)
            {
                // Make sure we're the tool on top!
                bool imOnTop = true;
                foreach(var overlap in Area2D.GetOverlappingAreas())
                {
                    var area = overlap as Area2D;
                    var entity = area.GetParent<Entity>();
                    var sprite = entity.GetNode<Sprite>("Sprite");

                    // Ignore overlapping entities that aren't under the mouse!
                    if (!sprite.GetRect().HasPoint(sprite.ToLocal(GetGlobalMousePosition())))
                    {
                        continue;
                    }

                    if (area.ZIndex > ZIndex)
                    {
                        imOnTop = false;
                        break;
                    }

                    if (entity.GetPositionInParent() > GetPositionInParent())
                    {
                        imOnTop = false;
                        break;
                    }
                }

                if (imOnTop)
                {
                    PickupTool();
                }
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

        private void ReleaseTarget(bool succeeded)
        {
            _activationCounter = 0;
            IsWorking = false;
            this.DetachAttachment(succeeded ? 0 : 3);

            _progressBar.Visible = false;
        }
        
        private void PickupTool()
        {
            _selected = true;

            // Succeeded flag doesn't matter, since _selected will snap to the mouse all the time
            ReleaseTarget(false);
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
            else
            {
                SetMovementTarget(Vector2.Zero);
            }
            // TODO if activation fails do a little shake animation?
            // TODO bounce off to the side of the entity as well
        }

        protected virtual void ActivateTool(Entity target)
        {
            this.AttachTo(target);

            IsWorking = true;
            _activationCounter = 0;
            _progressBar.Visible = true;
        }
        
        public virtual void AbortTool()
        {
            ReleaseTarget(false);
        }

        protected virtual bool CanActivate(Entity target)
        {
            return true;
        }

        protected virtual void ToolFinished()
        {
            ReleaseTarget(true);
            IsWorking = false;
        }
    }
}