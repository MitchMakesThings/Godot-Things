using Godot;

namespace CyberUnderground.Core
{
    public class AudioManager : Node
    {
        [Export]
        private NodePath _effectsPlayerNodePath;
        private AudioStreamPlayer _effectsPlayer;
        public override void _Ready()
        {
            _effectsPlayer = GetNode<AudioStreamPlayer>(_effectsPlayerNodePath);
        }

        public void PlayEffect(AudioStream stream)
        {
            _effectsPlayer.Stream = stream;
            _effectsPlayer.Play();
        }
    }
}
