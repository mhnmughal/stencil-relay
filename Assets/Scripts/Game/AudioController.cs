using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace StencilRelay.Game
{
    /// <summary>
    /// Audio service. All clips are project-generated originals loaded from
    /// Resources/Audio (see Assets/Documentation/AUDIO_PROVENANCE.md). Routes SFX / UI /
    /// music-ambience through the AudioMixer groups; the settings sliders drive the
    /// exposed MusicVol / SFXVol / UIVol parameters in dB.
    /// </summary>
    public sealed class AudioController : MonoBehaviour
    {
        [SerializeField] AudioMixer mixer;

        AudioSource _sfx;
        AudioSource _ui;
        AudioSource _amb;
        readonly Dictionary<string, AudioClip> _cache = new Dictionary<string, AudioClip>();

        float _music = 0.8f;
        float _sfxVol = 0.9f;

        void Awake()
        {
            if (mixer == null) mixer = Resources.Load<AudioMixer>("StencilRelayMixer");
            _sfx = MakeSource("SFX", loop: false, "SFX");
            _ui = MakeSource("UI", loop: false, "UI");
            _amb = MakeSource("Ambience", loop: true, "Music");
        }

        void Start()
        {
            // GameServices may configure us before our Awake ran; re-apply here.
            var svc = GameServices.I;
            if (svc != null) ApplyVolumes(svc.Save.Data.musicVolume, svc.Save.Data.sfxVolume);
            StartAmbience();
        }

        AudioSource MakeSource(string name, bool loop, string group)
        {
            var go = new GameObject("Audio_" + name);
            go.transform.SetParent(transform, false);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = loop;
            src.spatialBlend = 0f;
            if (mixer != null)
            {
                var groups = mixer.FindMatchingGroups(group);
                if (groups != null && groups.Length > 0) src.outputAudioMixerGroup = groups[0];
            }
            return src;
        }

        AudioClip Clip(string name)
        {
            if (_cache.TryGetValue(name, out var c)) return c;
            c = Resources.Load<AudioClip>("Audio/" + name);
            _cache[name] = c;
            return c;
        }

        public void ApplyVolumes(float music, float sfx)
        {
            _music = Mathf.Clamp01(music);
            _sfxVol = Mathf.Clamp01(sfx);
            if (mixer != null)
            {
                mixer.SetFloat("MusicVol", LinearToDb(_music));
                mixer.SetFloat("SFXVol", LinearToDb(_sfxVol));
                mixer.SetFloat("UIVol", LinearToDb(_sfxVol));
            }
            else
            {
                if (_sfx) _sfx.volume = _sfxVol;
                if (_ui) _ui.volume = _sfxVol;
                if (_amb) _amb.volume = _music;
            }
        }

        /// <summary>Play a gameplay one-shot ("sfx_plate_snap", etc.).</summary>
        public void Play(string clipName, float volumeScale = 1f)
        {
            var clip = Clip(clipName);
            if (clip != null && _sfx != null) _sfx.PlayOneShot(clip, Mathf.Clamp01(volumeScale));
        }

        /// <summary>Play a UI one-shot ("sfx_settings", "sfx_pause", ...).</summary>
        public void PlayUi(string clipName, float volumeScale = 1f)
        {
            var clip = Clip(clipName);
            if (clip != null && _ui != null) _ui.PlayOneShot(clip, Mathf.Clamp01(volumeScale));
        }

        public void StartAmbience(string clipName = "amb_room_tone")
        {
            var clip = Clip(clipName);
            if (clip == null || _amb == null) return;
            if (_amb.clip == clip && _amb.isPlaying) return;
            _amb.clip = clip;
            _amb.volume = 1f; // group volume handles level
            _amb.Play();
        }

        /// <summary>Restrained haptic tick, respecting the saved setting. Mobile only.</summary>
        public void Haptic()
        {
#if UNITY_IOS || UNITY_ANDROID
            var svc = GameServices.I;
            if (svc != null && svc.Save.Data.haptics) Handheld.Vibrate();
            // ponytail: Handheld.Vibrate is a single coarse buzz - the only cross-platform
            // hook without a native plugin. Upgrade to per-event patterns if it feels wrong on device.
#endif
        }

        static float LinearToDb(float v) => v <= 0.0005f ? -80f : Mathf.Log10(v) * 20f;
    }
}
