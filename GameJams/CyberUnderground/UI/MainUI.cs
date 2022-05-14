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

        public override void _Ready()
        {
            _system = GetNode<CoreSystem>("/root/System");
            _progressBar = GetNode<ProgressBar>(TickProgressNodePath);
            _animation = GetNode<AnimationPlayer>(AnimationPlayerNodePath);

            _system.Connect(nameof(CoreSystem.OnTick), this, nameof(OnTick));
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
    }
}
