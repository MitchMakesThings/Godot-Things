using CyberUnderground.Core;
using Godot;

namespace CyberUnderground.Maps
{
    public class CoreGame : Node
    {
        [Export]
        private PackedScene gameScene;
        private Node _game;

        [Export]
        private NodePath _lftNodePath;

        private Label _lftLabel;

        private Control _mainMenuControl;

        public int LifetimeEarnings = 0;
    
        public override void _Ready()
        {
            _mainMenuControl = GetNode<Control>("MainMenu");

            GetNode<CoreSystem>("/root/System").Connect(nameof(CoreSystem.OnGameEnded), this, nameof(OnGameEnded));

            _lftLabel = GetNode<Label>(_lftNodePath);
            _lftLabel.Text = LifetimeEarnings.ToString();
        }

        public void OnGameEnded(int score)
        {
            LifetimeEarnings += score;

            _lftLabel.Text = LifetimeEarnings.ToString();

            _mainMenuControl.Visible = true;
            
            _game.QueueFree();
        }

        // Called from button pressed signal, configured in editor
        public void StartGame()
        {
            _mainMenuControl.Visible = false;
            
            _game = gameScene.Instance();
            AddChild(_game);
        }

        // Called from button pressed signal, configured in editor
        public void ExitGame()
        {
            GetTree().Quit();
        }
    }
}
