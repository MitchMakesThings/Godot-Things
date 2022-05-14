using System.Collections.Generic;
using System.Linq;
using CyberUnderground.Core;
using CyberUnderground.Entities.Tools;
using Godot;

namespace CyberUnderground.Entities
{
    public class Entity : Node2D
    {
        protected Area2D Area2D { get; private set; }
        protected CoreSystem System { get; private set; }

        #region Attachments

        [Export]
        private Vector2 AttachmentOffsetLocation = new Vector2(24, 24);

        // TODO replace Attachments as a List with a Y-Sort node, which will visually stack the attached children.
        // We can then retrieve our Attachments from the Y-Sort children. Easy :)
        protected List<Entity> Attachments { get; private set; } = new List<Entity>();
        private Vector2 _attachPoint = Vector2.Inf;

        protected Entity AttachmentTarget;

        #endregion

        public override void _Ready()
        {
            base._Ready();

            // Blurgh, but there might be a regression causing [Export] nodepaths to be empty in inherited scenes...
            // https://github.com/godotengine/godot/issues/36480 was my original report
            Area2D = GetNode<Area2D>("Area2D");

            System.Connect(nameof(CoreSystem.OnTick), this, nameof(OnTick));
        }

        public override void _EnterTree()
        {
            base._EnterTree();

            System = GetNode<CoreSystem>("/root/System");
            System.EntityManager.Add(this);
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (_attachPoint != Vector2.Inf)
            {
                GlobalPosition = GlobalPosition.LinearInterpolate(_attachPoint, 25 * delta);
                if (GlobalPosition.DistanceSquaredTo(_attachPoint) < 1f)
                {
                    // Snap the last of the way, and 'null' our _targetSnapPosition
                    GlobalPosition = _attachPoint;
                    _attachPoint = Vector2.Inf;
                }
            }
        }

        public virtual void OnTick()
        {
        }

        public virtual void Delete()
        {
            System.EntityManager.Remove(this);

            foreach (var attachment in new HashSet<Entity>(Attachments))
            {
                if (attachment is Tool tool)
                {
                    tool.AbortTool();
                }
            }

            // TODO popping/particle animation
            Free();
        }

        #region Attachments

        public void AttachTo(Entity target)
        {
            _attachPoint = target.GetAttachmentPoint();
            target.AddAttachment(this);
            AttachmentTarget = target;
        }

        public void DetachAsAttachment()
        {
            AttachmentTarget?.RemoveAttachment(this);
            _attachPoint = Vector2.Inf;
            AttachmentTarget = null;

            // TODO bounce away in a random direction
        }

        public Vector2 GetAttachmentPoint()
        {
            var lastAttachment = Attachments.LastOrDefault() ?? this;

            return lastAttachment.GlobalPosition + AttachmentOffsetLocation;
        }

        public void AddAttachment(Entity e)
        {
            Attachments.Add(e);
        }

        public void RemoveAttachment(Entity e)
        {
            Attachments.Remove(e);
        }

        #endregion
    }
}