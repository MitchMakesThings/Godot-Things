using CyberUnderground.Core;
using Godot;

namespace CyberUnderground.Maps.Tutorials
{
    public class Tutorial : Node
    {

        public override void _Ready()
        {
            var system = GetNode<CoreSystem>("/root/System");
        
            system.ObjectiveManager.GenerateRandomObjectives();
            GD.Print("Ready to go");
        }

    }
}
