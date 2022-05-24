using System.Data.SqlTypes;
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
        private PackedScene _tutorialScene;

        [Export]
        private NodePath _lftNodePath;

        private Label _lftLabel;

        [Export]
        private NodePath _popupNodepath;

        private Popup _popup;

        [Export]
        private NodePath _popupLabelNodePath;

        private Label _popupLabel;

        [Export]
        private NodePath _timerNodePath;

        private Timer _timer;

        [Export]
        private NodePath _earningsLabelNodePath;

        private Label _earningsLabel;

        private Control _mainMenuControl;

        public int LifetimeEarnings = 0;

        private bool _connecting = false;

        public override void _Ready()
        {
            _mainMenuControl = GetNode<Control>("MainMenu");

            _lftLabel = GetNode<Label>(_lftNodePath);
            _lftLabel.Text = LifetimeEarnings.ToString();

            _timer = GetNode<Timer>(_timerNodePath);
            _timer.Connect("timeout", this, nameof(OnTimerTimeout));

            _popup = GetNode<Popup>(_popupNodepath);
            _popupLabel = GetNode<Label>(_popupLabelNodePath);

            _earningsLabel = GetNode<Label>(_earningsLabelNodePath);
        }

        public void OnGameEnded(bool serverDisconnected, int score)
        {
            if (_game is Tutorials.Tutorial)
            {
                // No money from the tutorial!
                score = 0;
            }

            if (!serverDisconnected)
            {
                LifetimeEarnings += score;

                _popupLabel.Text = "Another successful hack!";
                _earningsLabel.Text = "Earned: $" + score;
                _earningsLabel.Visible = true;
            }
            else
            {
                _popupLabel.Text = "Server security level exceeded. \n Disconnected from host.";
                _earningsLabel.Text = "$0 earned :(";
                _earningsLabel.Visible = true;
            }

            _lftLabel.Text = LifetimeEarnings.ToString();

            _mainMenuControl.Visible = true;

            _popup.Show();
            _timer.WaitTime = 2f;
            _timer.Start();
            _game.QueueFree();
        }

        // Called from button pressed signal, configured in editor
        public void StartGame()
        {
            _mainMenuControl.Visible = false;

            _popupLabel.Text = "Connecting...";
            _earningsLabel.Visible = false;
            _popup.Show();
            _timer.WaitTime = 1f;
            _timer.Start();

            _connecting = true;
        }

        // Called from button pressed signal, configured in editor
        public void ExitGame()
        {
            GetTree().Quit();
        }

        // Called from button pressed signal, configured in editor
        public void StartTutorial()
        {
            _mainMenuControl.Visible = false;

            _game = _tutorialScene.Instance();
            _game.Connect(nameof(Level.OnGameEnded), this, nameof(OnGameEnded));
            AddChild(_game);
        }

        public void OnTimerTimeout()
        {
            _popup.Hide();
            if (_connecting)
            {
                _game = gameScene.Instance();
                _game.Connect(nameof(Level.OnGameEnded), this, nameof(OnGameEnded));
                AddChild(_game);

                _connecting = false;
            }
        }
    }
}