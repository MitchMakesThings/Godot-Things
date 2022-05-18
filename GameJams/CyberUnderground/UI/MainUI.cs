using System.Collections.Generic;
using System.Linq;
using CyberUnderground.Core;
using Godot;

namespace CyberUnderground.UI
{
    public class MainUI : Control
    {
        static class UIAnimations
        {
            public static string TickComplete = "TickComplete";
        }

        [Export]
        private NodePath TickProgressNodePath;

        private ProgressBar _progressBar;

        [Export]
        private NodePath AnimationPlayerNodePath;

        private AnimationPlayer _animation;

        private CoreSystem _system;

        private Label _alertLevelLabel;

        [Export]
        private NodePath AlertLevelNodePath;

        [Export]
        private NodePath ObjectivesNodePath;

        private Node _objectivesParentNode;

        public override void _Ready()
        {
            _system = GetNode<CoreSystem>("/root/System");
            _progressBar = GetNode<ProgressBar>(TickProgressNodePath);
            _animation = GetNode<AnimationPlayer>(AnimationPlayerNodePath);

            _alertLevelLabel = GetNode<Label>(AlertLevelNodePath);
            _objectivesParentNode = GetNode<Node>(ObjectivesNodePath);

            _system.Connect(nameof(CoreSystem.OnTick), this, nameof(OnTick));
            _system.Connect(nameof(CoreSystem.OnAlertLevelUpdated), this, nameof(OnAlertLevelUpdated));
            _system.Connect(nameof(CoreSystem.OnObjectivesUpdated), this, nameof(OnObjectivesUpdated));

            // Fetch our first list of objectives
            OnObjectivesUpdated();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            _progressBar.Value = _system.TickPercentage;
        }

        public void OnTick()
        {
            _animation.Play(UIAnimations.TickComplete);
        }

        public void OnAlertLevelUpdated(int newLevel)
        {
            _alertLevelLabel.Text = $"Alert Level {newLevel}";
        }

        public void OnObjectivesUpdated()
        {
            var objectives = _system.ObjectiveManager.GetObjectives();
            GD.Print("Hi");

            // TODO repurpose instead of deleting and replacing!
            foreach (var child in _objectivesParentNode.GetChildren())
            {
                (child as Node)?.QueueFree();
            }

            // TODO rejig this to support quests (ie, objectives should provide their Instructions!)
            var deleteObjectives = objectives.Where(o => o.Type == ObjectiveType.Delete);
            if (deleteObjectives.Any())
            {
                Label label = new Label();
                _objectivesParentNode.AddChild(label);

                if (deleteObjectives.All(o => o.Complete))
                {
                    label.Set("custom_colors/font_color", new Color("fff073"));
                }

                var count = deleteObjectives.Count();
                label.Text = $"Delete {(count > 1 ? count.ToString() : "")} {GetDeleteFilePurpose()}" + (count > 1 ? " files" : "");
            }
            
            var downloadObjectives = objectives.Where(o => o.Type == ObjectiveType.Download);
            if (downloadObjectives.Any())
            {
                Label label = new Label();
                _objectivesParentNode.AddChild(label);

                if (downloadObjectives.All(o => o.Complete))
                {
                    label.Set("custom_colors/font_color", new Color("fff073"));
                }

                var count = downloadObjectives.Count();
                label.Text = $"Download {(count > 1 ? count.ToString() : "")} {GetDownloadFilePurpose()}" +
                             (count > 1 ? " files" : "");
            }
        }

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
    }
}