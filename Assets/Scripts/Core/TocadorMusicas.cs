using UnityEngine;

namespace PixelIdle.Core
{
    /// <summary>Toca uma música de fundo em loop. Cria o próprio AudioSource.</summary>
    public class MusicPlayer : MonoBehaviour
    {
        private AudioSource source;

        public static MusicPlayer Create(AudioClip clip, float volume)
        {
            var player = new GameObject("MusicPlayer").AddComponent<MusicPlayer>();
            player.source = player.gameObject.AddComponent<AudioSource>();
            player.source.clip = clip;
            player.source.loop = true;
            player.source.playOnAwake = false;
            player.source.volume = volume;
            if (clip != null) player.source.Play();
            return player;
        }

        public float Volume
        {
            get => source.volume;
            set => source.volume = Mathf.Clamp01(value);
        }

        public bool Muted
        {
            get => source.mute;
            set => source.mute = value;
        }
    }
}
