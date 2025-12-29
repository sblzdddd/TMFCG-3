using UnityEngine;

namespace Game.Client.Managers
{
    public class AudioManager : MonoBehaviour
    {

        private AudioSource uiAudioSource;
        private AudioSource sfxAudioSource;

        protected void Awake()
        {
            InitializeAudioSources();
        }

        private void InitializeAudioSources()
        {
            // Setup UI audio source (for button sounds, etc.)
            uiAudioSource = gameObject.AddComponent<AudioSource>();
            uiAudioSource.playOnAwake = false;
            uiAudioSource.loop = false;
            uiAudioSource.spatialBlend = 0f; // Pure 2D sound
            uiAudioSource.priority = 0; // Highest priority

            // Setup SFX audio source (for other sound effects)
            sfxAudioSource = gameObject.AddComponent<AudioSource>();
            sfxAudioSource.playOnAwake = false;
            sfxAudioSource.loop = false;
            sfxAudioSource.spatialBlend = 0f;
            sfxAudioSource.priority = 128; // Normal priority
        }

        public void PlayUISound(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
        
            // Stop any currently playing UI sound to prevent overlap
            uiAudioSource.PlayOneShot(clip, volume);
        }

        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            sfxAudioSource.PlayOneShot(clip, volume);
        }

        // Optional: Add volume control methods
        public void SetUIVolume(float volume)
        {
            if (uiAudioSource != null)
                uiAudioSource.volume = Mathf.Clamp01(volume);
        }

        public void SetSFXVolume(float volume)
        {
            if (sfxAudioSource != null)
                sfxAudioSource.volume = Mathf.Clamp01(volume);
        }
    }
} 