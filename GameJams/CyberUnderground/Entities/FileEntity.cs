using System.Collections.Generic;
using Godot;

namespace CyberUnderground.Entities
{
    public class FileEntity : Entity
    {
        public void LowerIntensity()
        {
            var sprite = GetNode<Sprite>("Sprite");
            sprite.Modulate = new Color(sprite.Modulate.r, sprite.Modulate.g, sprite.Modulate.b, .5f);
        }
    }
}
