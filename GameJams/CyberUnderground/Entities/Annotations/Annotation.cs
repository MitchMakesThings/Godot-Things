using Godot;

namespace CyberUnderground.Entities.Annotations
{
    public class Annotation : Node2D
    {
        private Sprite _sprite;

        public override void _Ready()
        {
            base._Ready();
            
            _sprite = GetNode<Sprite>("Sprite");
        }

        public void SetColor(Color newColor)
        {
            _sprite.Modulate = newColor;
        }
    }
}
