using Godot;

namespace CyberUnderground.Maps
{
    public class CoreGame : Node
    {
        [Export]
        private PackedScene gameScene;

        private Node _game;

        private Control _mainMenuControl;
    
        public override void _Ready()
        {
            _mainMenuControl = GetNode<Control>("MainMenu");
        }

        public void StartGame()
        {
            _mainMenuControl.Visible = false;
            
            _game = gameScene.Instance();
            AddChild(_game);
        }

        public void ExitGame()
        {
            GetTree().Quit();
        }
    }
}
