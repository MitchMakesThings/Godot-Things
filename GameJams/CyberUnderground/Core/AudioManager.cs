using System;
using Godot;

namespace CyberUnderground.Core
{
    public class AudioManager : Node
    {
        [Export]
        private NodePath _effectsPlayerNodePath;
        private AudioStreamPlayer _effectsPlayer;

        public static AudioManager Instance { get; private set; }
        
        public override void _Ready()
        {
            if (Instance != null) throw new Exception("AudioManager already registered!");
            
            Instance = this;
            _effectsPlayer = GetNode<AudioStreamPlayer>(_effectsPlayerNodePath);
        }

        public void PlayEffect(AudioStream stream)
        {
            _effectsPlayer.Stream = stream;
            _effectsPlayer.Play();
        }
    }
}
