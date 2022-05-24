using System.Collections.Generic;
using System.Linq;
using CyberUnderground.Core;
using CyberUnderground.Maps;
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

        private Level _system;

        private Label _alertLevelLabel;

        [Export]
        private NodePath AlertLevelNodePath;

        [Export]
        private NodePath ObjectivesNodePath;

        private Node _objectivesParentNode;

        public override void _Ready()
        {
            _system = Level.Instance;
            _progressBar = GetNode<ProgressBar>(TickProgressNodePath);
            _animation = GetNode<AnimationPlayer>(AnimationPlayerNodePath);

            _alertLevelLabel = GetNode<Label>(AlertLevelNodePath);
            _objectivesParentNode = GetNode<Node>(ObjectivesNodePath);

            _system.Connect(nameof(Level.OnTick), this, nameof(OnTick));
            _system.Connect(nameof(Level.OnAlertLevelUpdated), this, nameof(OnAlertLevelUpdated));
            _system.Connect(nameof(Level.OnObjectivesUpdated), this, nameof(OnObjectivesUpdated));

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

            // TODO repurpose instead of deleting and replacing!
            foreach (var child in _objectivesParentNode.GetChildren())
            {
                (child as Node)?.QueueFree();
            }

            foreach (var objective in objectives)
            {
                Label label = new Label();
                _objectivesParentNode.AddChild(label);

                if (objective.Complete)
                {
                    label.Set("custom_colors/font_color", new Color("fff073"));
                }

                label.Text = objective.Instructions;
            }
        }
    }
}