using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace StencilRelay.Editor
{
    /// <summary>
    /// Synthesises every Stencil Relay sound effect and the ambience loop as 16-bit PCM
    /// WAV assets (project-generated, no sample libraries). The palette is precise, light,
    /// optical, acrylic, glass-like, restrained. Regenerate: Stencil Relay/Rebuild Audio.
    ///
    /// Provenance for every clip: Assets/Documentation/AUDIO_PROVENANCE.md.
    /// </summary>
    public static class AudioSynthBuilder
    {
        const string Dir = "Assets/Content/Resources/Audio";
        const int SR = 44100;

        /// <summary>float[] sample buffer with +, * (scalar and per-sample) operators.</summary>
        readonly struct Sig
        {
            public readonly float[] D;
            public Sig(float[] d) { D = d; }
            public static implicit operator Sig(float[] d) => new Sig(d);
            public static implicit operator float[](Sig s) => s.D;

            public static Sig operator +(Sig a, Sig b)
            {
                var o = new float[Math.Max(a.D.Length, b.D.Length)];
                for (int i = 0; i < o.Length; i++)
                    o[i] = (i < a.D.Length ? a.D[i] : 0f) + (i < b.D.Length ? b.D[i] : 0f);
                return o;
            }
            public static Sig operator *(Sig a, float k)
            {
                var o = new float[a.D.Length];
                for (int i = 0; i < o.Length; i++) o[i] = a.D[i] * k;
                return o;
            }
            public static Sig operator *(Sig a, Sig b)
            {
                var o = new float[Math.Min(a.D.Length, b.D.Length)];
                for (int i = 0; i < o.Length; i++) o[i] = a.D[i] * b.D[i];
                return o;
            }
        }

        [MenuItem("Stencil Relay/Rebuild Audio")]
        public static void Rebuild()
        {
            Directory.CreateDirectory(Dir);

            Save("sfx_plate_pickup", Env(Blip(520, 0.06f, 0.35f) + Blip(780, 0.05f, 0.18f), 0.001f, 0.09f));
            Save("sfx_plate_slide", Env(Noise(0.16f, 0.10f, lp: 2200) * Sweep(0.16f, 1f, 0.7f), 0.01f, 0.16f));
            Save("sfx_plate_snap", Env(Blip(900, 0.04f, 0.5f) + Noise(0.04f, 0.25f, lp: 4000), 0.0005f, 0.06f));
            Save("sfx_pin_click", Env(Blip(1600, 0.025f, 0.45f) + Blip(2400, 0.02f, 0.2f), 0.0004f, 0.035f));
            Save("sfx_rotate", Env(Sweep2(0.14f, 400, 620) * 0.3f + Noise(0.14f, 0.06f, lp: 1800), 0.006f, 0.14f));
            Save("sfx_flip", Env(Sweep2(0.22f, 300, 900) * 0.28f + Noise(0.22f, 0.05f, lp: 1500), 0.01f, 0.22f));
            Save("sfx_table_touch", Env(Blip(220, 0.05f, 0.3f) + Noise(0.05f, 0.08f, lp: 900), 0.002f, 0.07f));
            Save("sfx_projector_switch", Env(Blip(140, 0.05f, 0.5f) + Blip(280, 0.03f, 0.2f) + Noise(0.03f, 0.3f, lp: 3000), 0.0006f, 0.08f));
            Save("sfx_projector_startup", Env(Sweep2(0.9f, 90, 220) * 0.35f + Hum(0.9f, 120, 0.12f), 0.05f, 0.9f, rel: 0.3f));
            Save("sfx_light_hum", Loop(Hum(2.0f, 110, 0.10f) + Hum(2.0f, 220, 0.03f) + Noise(2.0f, 0.008f, lp: 600)));
            Save("sfx_target_activate", Env(Bell(660, 0.5f, 0.4f) + Bell(990, 0.4f, 0.15f), 0.002f, 0.5f, rel: 0.25f));
            Save("sfx_forbidden", Env(Blip(180, 0.18f, 0.4f) + Blip(190, 0.18f, 0.35f), 0.004f, 0.2f)); // low beat, uneasy
            Save("sfx_success", Env(Bell(523, 0.9f, 0.32f) + Bell(659, 0.9f, 0.26f) + Bell(784, 0.9f, 0.20f) + Bell(1047, 0.8f, 0.12f), 0.003f, 0.9f, rel: 0.4f));
            Save("sfx_misalign", Env(Blip(300, 0.12f, 0.3f) + Blip(240, 0.16f, 0.28f), 0.004f, 0.18f, rel: 0.08f));
            Save("sfx_rack_slide", Env(Noise(0.35f, 0.09f, lp: 1400) * Sweep(0.35f, 1f, 0.6f) + Blip(160, 0.05f, 0.15f), 0.02f, 0.34f));
            Save("sfx_pause", Env(Sweep2(0.3f, 700, 400) * 0.28f + Noise(0.3f, 0.03f, lp: 1200), 0.02f, 0.3f));
            Save("sfx_settings", Env(Blip(880, 0.03f, 0.35f) + Blip(1320, 0.025f, 0.18f), 0.0006f, 0.04f));
            Save("sfx_final_complete", Env(
                Bell(392, 2.4f, 0.28f) + Bell(523, 2.4f, 0.24f) + Bell(659, 2.4f, 0.20f) +
                Bell(784, 2.2f, 0.16f) + Bell(1047, 2.0f, 0.12f) + Bell(1319, 1.8f, 0.08f),
                0.01f, 2.4f, rel: 0.8f));
            Save("amb_room_tone", Loop(Hum(4.0f, 55, 0.05f) + Hum(4.0f, 82, 0.02f) + Noise(4.0f, 0.010f, lp: 400) + Breath(4.0f, 0.02f)));

            AssetDatabase.Refresh();
            foreach (var f in Directory.GetFiles(Dir, "*.wav"))
            {
                var imp = AssetImporter.GetAtPath(f.Replace('\\', '/')) as AudioImporter;
                if (imp == null) continue;
                imp.forceToMono = true;
                var settings = imp.defaultSampleSettings;
                settings.loadType = f.Contains("amb_") || f.Contains("_hum")
                    ? AudioClipLoadType.Streaming : AudioClipLoadType.DecompressOnLoad;
                settings.compressionFormat = AudioCompressionFormat.Vorbis;
                settings.quality = 0.7f;
                imp.defaultSampleSettings = settings;
                imp.SaveAndReimport();
            }
            Debug.Log("[AudioSynth] Rebuilt Stencil Relay audio (" + Directory.GetFiles(Dir, "*.wav").Length + " clips) in " + Dir);
        }

        // ---------------------------------------------------------------- synthesis

        static Sig Blip(float freq, float dur, float amp)
        {
            int n = (int)(dur * SR);
            var s = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)SR;
                float e = Mathf.Exp(-t * (6f / dur));
                s[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * e * amp;
            }
            return s;
        }

        static Sig Bell(float freq, float dur, float amp)
        {
            int n = (int)(dur * SR);
            var s = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)SR;
                float e = Mathf.Exp(-t * (3.5f / dur));
                s[i] = (Mathf.Sin(2 * Mathf.PI * freq * t)
                        + 0.5f * Mathf.Sin(2 * Mathf.PI * freq * 2.01f * t)
                        + 0.25f * Mathf.Sin(2 * Mathf.PI * freq * 3.03f * t)) * e * amp * 0.6f;
            }
            return s;
        }

        static Sig Hum(float dur, float freq, float amp)
        {
            int n = (int)(dur * SR);
            var s = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)SR;
                s[i] = (Mathf.Sin(2 * Mathf.PI * freq * t) + 0.3f * Mathf.Sin(2 * Mathf.PI * freq * 2 * t)) * amp;
            }
            return s;
        }

        static Sig Noise(float dur, float amp, float lp)
        {
            int n = (int)(dur * SR);
            var s = new float[n];
            var rnd = new System.Random(12345);
            float prev = 0f;
            float a = Mathf.Clamp01(lp / SR * 2f);
            for (int i = 0; i < n; i++)
            {
                float w = (float)(rnd.NextDouble() * 2 - 1);
                prev = Mathf.Lerp(prev, w, a);
                s[i] = prev * amp;
            }
            return s;
        }

        static Sig Breath(float dur, float amp)
        {
            int n = (int)(dur * SR);
            var s = new float[n];
            var rnd = new System.Random(777);
            float prev = 0f;
            for (int i = 0; i < n; i++)
            {
                float w = (float)(rnd.NextDouble() * 2 - 1);
                prev = Mathf.Lerp(prev, w, 0.02f);
                float slow = 0.5f + 0.5f * Mathf.Sin(2 * Mathf.PI * 0.08f * (i / (float)SR));
                s[i] = prev * amp * slow;
            }
            return s;
        }

        static Sig Sweep(float dur, float from, float to)
        {
            int n = (int)(dur * SR);
            var s = new float[n];
            for (int i = 0; i < n; i++) s[i] = Mathf.Lerp(from, to, i / (float)n);
            return s;
        }

        static Sig Sweep2(float dur, float f0, float f1)
        {
            int n = (int)(dur * SR);
            var s = new float[n];
            float phase = 0f;
            for (int i = 0; i < n; i++)
            {
                float f = Mathf.Lerp(f0, f1, i / (float)n);
                phase += 2 * Mathf.PI * f / SR;
                s[i] = Mathf.Sin(phase);
            }
            return s;
        }

        static Sig Env(Sig sig, float attack, float dur, float rel = 0.04f)
        {
            var s = sig.D;
            int a = Mathf.Max(1, (int)(attack * SR)), r = Mathf.Max(1, (int)(rel * SR));
            var o = new float[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                float g = 1f;
                if (i < a) g = i / (float)a;
                else if (i > s.Length - r) g = Mathf.Max(0f, (s.Length - i) / (float)r);
                o[i] = s[i] * g;
            }
            return o;
        }

        static Sig Loop(Sig sig)
        {
            var s = sig.D;
            int f = Mathf.Min(s.Length / 8, SR / 4);
            for (int i = 0; i < f; i++)
            {
                float m = i / (float)f;
                s[i] = s[i] * m + s[s.Length - f + i] * (1 - m);
            }
            return s;
        }

        // ---------------------------------------------------------------- WAV write

        static void Save(string name, float[] samples)
        {
            // normalise to -1..1 with headroom
            float peak = 0.0001f;
            foreach (var v in samples) peak = Mathf.Max(peak, Mathf.Abs(v));
            float g = 0.85f / peak;

            var path = $"{Dir}/{name}.wav";
            using var fs = new FileStream(path, FileMode.Create);
            using var w = new BinaryWriter(fs);
            int n = samples.Length;
            int byteRate = SR * 2;
            w.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            w.Write(36 + n * 2);
            w.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
            w.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            w.Write(16); w.Write((short)1); w.Write((short)1);
            w.Write(SR); w.Write(byteRate); w.Write((short)2); w.Write((short)16);
            w.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            w.Write(n * 2);
            for (int i = 0; i < n; i++)
                w.Write((short)(Mathf.Clamp(samples[i] * g, -1f, 1f) * 32767));
        }
    }
}
