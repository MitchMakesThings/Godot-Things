using System.Collections.Generic;
using System.Linq;
using CyberUnderground.Entities;
using Godot;

namespace CyberUnderground.Core
{
    public enum ObjectiveType
    {
        Delete,
        Download,
        Upload // TODO
    }

    public class Objective
    {
        private string GetDeleteFilePurpose()
        {
            var deleteReasons = new string[]
            {
                "log",
                "research",
                "blackmail",
                "crypto",
                "password"
            };
            return deleteReasons[new System.Random().Next(deleteReasons.Length)];
        }

        private string GetDownloadFilePurpose()
        {
            var reasons = new string[]
            {
                "log",
                "evidence",
                "blackmail",
                "selfie",
                "password"
            };
            return reasons[new System.Random().Next(reasons.Length)];
        }

        public bool Complete => Targets.All(t => CompletedTargets.Contains(t));
        public string Instructions { get; set; }

        private ObjectiveType _type;

        public ObjectiveType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                UpdateInstructions();
            }
        }

        private ICollection<Entity> Targets { get; set; } = new HashSet<Entity>();
        private ICollection<Entity> CompletedTargets { get; set; } = new HashSet<Entity>();

        public void AddTarget(Entity e)
        {
            Targets.Add(e);
            UpdateInstructions();
        }

        public void CompleteTarget(Entity e)
        {
            CompletedTargets.Add(e);
        }

        private void UpdateInstructions()
        {
            switch (_type)
            {
                case ObjectiveType.Delete:
                    Instructions = $"Delete {Targets.Count()} {GetDeleteFilePurpose()}";
                    break;
                case ObjectiveType.Download:
                    Instructions = $"Download {Targets.Count()} {GetDownloadFilePurpose()}";
                    break;
            }
        }
    }
}