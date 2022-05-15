using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace CyberUnderground.Entities
{
    public static class AttachmentHelper
    {
        private static readonly Dictionary<Entity, ICollection<Entity>> _attachments = new Dictionary<Entity, ICollection<Entity>>();
        private static readonly Dictionary<Entity, Entity> _childLookup = new Dictionary<Entity, Entity>();
        private static readonly RandomNumberGenerator _random = new RandomNumberGenerator();

        public static void AttachTo(this Entity child, Entity parent)
        {
            if (_childLookup.ContainsKey(child))
            {
                throw new ArgumentException($"Child is already attached to {_childLookup[child].Name}");
            }

            if (!_attachments.ContainsKey(parent))
            {
                _attachments.Add(parent, new List<Entity>());
            }

            _attachments[parent].Add(child);
            child.ZIndex = _attachments[parent].Count();
            _childLookup.Add(child, parent);

            var target = parent.ToGlobal(parent.AttachmentOffsetLocation * _attachments[parent].Count);
            child.SetMovementTarget(target);
        }

        public static void DetachAttachment(this Entity child, int quadrant = 0)
        {
            if (!_childLookup.ContainsKey(child)) return;

            var parent = _childLookup[child];

            _attachments[parent].Remove(child);

            _childLookup.Remove(child);
            
            ReAlignAttachments(parent);
            
            // Send the child in a random direction within a quadrant (0 top right, 1 bottom right etc)
            var rndAngle = _random.RandfRange(90f * quadrant, 90f * (quadrant + 1));
            var rndPoint = new Vector2(0, -120).Rotated(Mathf.Deg2Rad(rndAngle));
            child.SetMovementTarget(parent.GlobalPosition + rndPoint);
        }

        private static void ReAlignAttachments(Entity parent)
        {
            if (!_attachments.ContainsKey(parent)) return;

            var i = 0;
            foreach (var child in _attachments[parent])
            {
                i++;

                child.ZIndex = i;
                var target = parent.ToGlobal(parent.AttachmentOffsetLocation * i);
                child.SetMovementTarget(target);
            }
        }

        public static Entity GetAttachmentTarget(this Entity child)
        {
            if (!_childLookup.ContainsKey(child)) return null;

            return _childLookup[child];
        }

        public static IEnumerable<Entity> GetAttachments(this Entity parent)
        {
            return new List<Entity>(_attachments[parent]);
        }
    }
}