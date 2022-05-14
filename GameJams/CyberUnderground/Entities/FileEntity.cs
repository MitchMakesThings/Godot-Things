using System.Collections.Generic;
using Godot;

namespace CyberUnderground.Entities
{
    public class FileEntity : Entity
    {
        public override void _Ready()
        {
        
        }

        public void Delete()
        {
            // TODO popping/particle animation
            QueueFree();
        }
        
    }
}
