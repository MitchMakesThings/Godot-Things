using System;
using CyberUnderground.Core;
using CyberUnderground.Entities.Tools;
using CyberUnderground.Maps;
using Godot;

namespace CyberUnderground.Entities
{
    public class Entity : Node2D
    {
        protected Area2D Area2D { get; private set; }
        protected Level System { get; private set; }
        
        [Export]
        public string EntityName { get; protected set; }

        [Export]
        public Vector2 AttachmentOffsetLocation = new Vector2(24, 24);

        private Vector2 _movementTarget = Vector2.Zero;


        public override void _Ready()
        {
            base._Ready();

            System = Level.Instance;
            System.EntityManager.Add(this);

            // Blurgh, but there might be a regression causing [Export] nodepaths to be empty in inherited scenes...
            // https://github.com/godotengine/godot/issues/36480 was my original report
            Area2D = GetNode<Area2D>("Area2D");

            if (!String.IsNullOrEmpty(EntityName))
            {
                var nameLabel = GetNode<Label>("LabelLocator/Label");//("Control/CenterContainer/Label");
                nameLabel.Text = EntityName;
            }

            System.Connect(nameof(Level.OnTick), this, nameof(OnTick));
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (_movementTarget != Vector2.Zero)
            {
                GlobalPosition = GlobalPosition.LinearInterpolate(_movementTarget, 5 * delta);
                if (GlobalPosition.DistanceSquaredTo(_movementTarget) < 1f)
                {
                    // Snap the last of the way, and 'null' out the _movementTarget
                    GlobalPosition = _movementTarget;
                    SetMovementTarget(Vector2.Zero);
                }
            }
        }

        public virtual void OnTick()
        {
        }

        public virtual void Delete()
        {
            System.EntityManager.Remove(this);

            foreach (var attachment in this.GetAttachments())
            {
                if (attachment is Tool tool)
                {
                    tool.AbortTool();
                }
                
                attachment.DetachAttachment();
            }

            // TODO popping/particle animation
            QueueFree();
        }

        public void SetMovementTarget(Vector2 globalPosition)
        {
            _movementTarget = globalPosition;
        }
    }
}