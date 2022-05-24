using System.Collections.Generic;
using CyberUnderground.Entities;
using Godot;

namespace CyberUnderground.Maps
{
    public class RandomMap : Level
    {
        [Export]
        private PackedScene fileScene;

        [Export]
        private PackedScene scannerScene;

        [Export]
        private NodePath fileSpawnNodePath;

        public override void _Ready()
        {
            base._Ready();
            
            var fileSpawnParent = GetNode<Node2D>(fileSpawnNodePath);
            var filePositions = new List<Vector2>();
            for (int i = 0; i < Rnd.Next(6, 15); i++)
            {
                var file = fileScene.Instance<FileEntity>();
                fileSpawnParent.AddChild(file);
                
                // Randomize position
                // TODO make sure it's not too close to another file
                bool inSafePlace = true;
                Vector2 pos;
                do
                {
                    pos = new Vector2(Rnd.Next(40, 720), Rnd.Next(120, 550));
                    foreach (var existingPos in filePositions)
                    {
                        if (existingPos.DistanceSquaredTo(pos) < 6400)
                        {
                            inSafePlace = false;
                            break;
                        }

                        inSafePlace = true;
                    }
                } while (inSafePlace == false);
                filePositions.Add(pos);
                file.Position = pos;
            }

            for (int i = 0; i < Rnd.Next(2, 5); i++)
            {
                var scanner = scannerScene.Instance<Node2D>();
                AddChild(scanner);
                scanner.Position = new Vector2(-100 + (i * Rnd.Next(100) * -1), 500 * i);
            }
            
            ObjectiveManager.GenerateRandomObjectives();
        }
    }
}