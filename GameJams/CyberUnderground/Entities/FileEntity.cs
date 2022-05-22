using System.Collections.Generic;
using System.Linq;
using CyberUnderground.Core;
using Godot;

namespace CyberUnderground.Entities
{
    public class FileEntity : Entity
    {
        [Export]
        private bool exportIsDeleteObjective
        {
            get
            {
                return System.ObjectiveManager.GetObjectives().Any(o => o.Type == ObjectiveType.Delete && o.HasTarget(this));
            }
            set { }
        }
        
        [Export]
        private bool exportIsDownloadObjective
        {
            get
            {
                return System.ObjectiveManager.GetObjectives().Any(o => o.Type == ObjectiveType.Download && o.HasTarget(this));
            }
            set { }
        }
        
        public void LowerIntensity()
        {
            var sprite = GetNode<Sprite>("Sprite");
            sprite.Modulate = new Color(sprite.Modulate.r, sprite.Modulate.g, sprite.Modulate.b, .5f);
        }
    }
}
