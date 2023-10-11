using Godot;
using System;
using System.Collections.Generic;
using Com.IsartDigital.Utils.Events;

// Author : Lefevre Florian
namespace Com.IsartDigital.Sokoban.Managers {

    public class SoundManager : Node
    {
        private static SoundManager instance;

        [Export] private int nSoundEmitter = 60;
        [Export] private NodePath musicEmitterPath = default;

        [Export(PropertyHint.Range,"-80f, 24f, 1f")] private float baseMusicVolume = 0f;
        [Export(PropertyHint.Range, "-80f, 24f, 1f")] private float baseSFXVolume = 0f;

        private List<AudioStreamPlayer> audioPlayerPool = new List<AudioStreamPlayer>();
        private List<AudioStreamPlayer> activeAudioPlayer = new List<AudioStreamPlayer>();

        private AudioStreamPlayer musicEmitter = null;

        public const double BASE_VOLUME_MUSIC = 1;
        public const double BASE_VOLUME_SFX = 1;

        public static float globalSFX = (float)BASE_VOLUME_SFX;
        public static float globalMusic = (float)BASE_VOLUME_MUSIC;

        public const int DB_CONVERTER = 100;

        private SoundManager() : base() { }

        public override void _Ready()
        {
            if (instance != null)
            {
                QueueFree();
                return;
            }
            instance = this;

            Connect(EventNode.TREE_EXITING, this, nameof(Destructor));

            if (musicEmitterPath != null)
            {
                musicEmitter = GetNode<AudioStreamPlayer>(musicEmitterPath);
                musicEmitter.VolumeDb = baseMusicVolume;
            }

            for (int i = 0; i < nSoundEmitter; i++)
                audioPlayerPool.Add(new AudioStreamPlayer());
        }

        public static SoundManager GetInstance()
        {
            if (instance == null) 
                instance = new SoundManager();
            return instance;
        }

        public void GetAudioPlayer(AudioStreamMP3 pStream, Node pTarget, bool pLooping = false, PauseModeEnum pPauseMode = PauseModeEnum.Inherit)
        {
            AudioStreamPlayer lAudio = audioPlayerPool[0];
            if (lAudio == null)
                lAudio = new AudioStreamPlayer();
            pStream.Loop = pLooping;
            lAudio.Stream = pStream;
            lAudio.Autoplay = true;
            lAudio.VolumeDb = baseSFXVolume;
            lAudio.PauseMode = pPauseMode;

            activeAudioPlayer.Add(lAudio);

            lAudio.Connect(EventAudioStreamPlayer2D.FINISHED, this, nameof(CleanAudioPlayer), new Godot.Collections.Array(lAudio, pTarget));
            if (!pTarget.IsConnected(EventNode.TREE_EXITING, this, nameof(CleanAudioPlayer)))
                pTarget.Connect(EventNode.TREE_EXITING, this, nameof(CleanAudioPlayer), new Godot.Collections.Array(lAudio, pTarget));
            audioPlayerPool.Remove(lAudio);

            pTarget.AddChild(lAudio);

        }

        private void CleanAudioPlayer(AudioStreamPlayer pAudio, Node pTarget)
        {
            pAudio.Stop();
            if (pTarget.IsConnected(EventNode.TREE_EXITING, this, nameof(CleanAudioPlayer)))
                pTarget.Disconnect(EventNode.TREE_EXITING, this, nameof(CleanAudioPlayer));
            pAudio.Disconnect(EventAudioStreamPlayer2D.FINISHED, this, nameof(CleanAudioPlayer));
            pAudio.Stream = null;
            pAudio.PauseMode = PauseModeEnum.Inherit;

            activeAudioPlayer.Remove(pAudio);

            pTarget.RemoveChild(pAudio);

            audioPlayerPool.Add(pAudio);
        }

        public void PauseAudioPlayers(bool pState)
        {
            int lLength = activeAudioPlayer.Count - 1;
            for (int i = lLength; i >= 0; i--)
            {
                activeAudioPlayer[i].StreamPaused = pState;
            }
            musicEmitter.StreamPaused = pState;
        }

        public void SaveVolumeConfiguration(float pSFX, float pMusic)
        {
            baseSFXVolume = pSFX;
            baseMusicVolume = pMusic;
        }

        public void ChangeAudioPlayersVFXVolume(float pDBVolume)
        {
            baseSFXVolume = pDBVolume;
            int lLength = activeAudioPlayer.Count - 1;
            for (int i = lLength; i >= 0; i--)
            {
                activeAudioPlayer[i].VolumeDb = pDBVolume;
            }
        }

        public void ChangeAudioPlayerMusicVolume(float pDBVolume)
        {
            musicEmitter.VolumeDb = pDBVolume;
            baseMusicVolume = pDBVolume;
        }

        public void ChangeMainMusic(AudioStreamMP3 pMusic, bool pLooping = true, bool pPausedPlay = false)
        {
            musicEmitter.Stop();
            musicEmitter.Stream = null;
            musicEmitter.Stream = pMusic;
            musicEmitter.PauseMode = PauseModeEnum.Inherit;
            if (pPausedPlay)
                musicEmitter.PauseMode = PauseModeEnum.Process;
            pMusic.Loop = pLooping;
            musicEmitter.Play();
        }

        private void Destructor()
        {
            Disconnect(EventNode.TREE_EXITING, this, nameof(Destructor));
            QueueFree();
        }

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && instance == this) instance = null;
            base.Dispose(pDisposing);
        }
    }
}