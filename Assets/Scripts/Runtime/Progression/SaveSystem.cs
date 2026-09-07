using System;
using System.IO;
using UnityEngine;
using StencilRelay.Levels;

namespace StencilRelay.Progression
{
    /// <summary>
    /// Versioned local save. One JSON file in persistentDataPath. Handles first launch,
    /// missing save, corrupt save (backed up, then defaults), and an incompatible newer
    /// version (backed up, then defaults - never a destructive silent downgrade).
    /// A better rating or fewer moves is never overwritten by a worse replay.
    /// </summary>
    public sealed class SaveSystem
    {
        public const int CurrentVersion = 1;
        public const int LastLevelId = 12;

        readonly string _path;
        public SaveData Data { get; private set; }

        public event Action Changed;

        public SaveSystem(string pathOverride = null)
        {
            _path = pathOverride ?? DefaultPath();
            Load();
        }

        public static string DefaultPath()
            => Path.Combine(Application.persistentDataPath, "stencilrelay.save");

        public void Load()
        {
            if (!File.Exists(_path))
            {
                Data = new SaveData();
                return;
            }

            try
            {
                var json = File.ReadAllText(_path);
                var loaded = JsonUtility.FromJson<SaveData>(json);
                if (loaded == null)
                {
                    Backup(".corrupt");
                    Data = new SaveData();
                }
                else if (loaded.version > CurrentVersion)
                {
                    Backup($".v{loaded.version}");
                    Data = new SaveData();
                }
                else
                {
                    // loaded.version <= CurrentVersion: accept (add migration steps here later)
                    loaded.records ??= new System.Collections.Generic.List<LevelRecord>();
                    loaded.version = CurrentVersion;
                    Data = loaded;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SaveSystem] Corrupt save ({e.Message}); backing up and resetting.");
                Backup(".corrupt");
                Data = new SaveData();
            }
        }

        public void Save()
        {
            try
            {
                var dir = Path.GetDirectoryName(_path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
                // atomic-ish: write temp then replace
                var tmp = _path + ".tmp";
                File.WriteAllText(tmp, JsonUtility.ToJson(Data, prettyPrint: true));
                if (File.Exists(_path)) File.Delete(_path);
                File.Move(tmp, _path);
                Changed?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Save failed: {e.Message}");
            }
        }

        // ---- progression ----

        public bool IsUnlocked(int levelId) => Data.IsUnlocked(levelId);

        public void MarkTutorialDone()
        {
            if (Data.tutorialDone && Data.highestUnlockedId >= 1) return;
            Data.tutorialDone = true;
            Data.highestUnlockedId = Mathf.Max(Data.highestUnlockedId, 1);
            Save();
        }

        /// <summary>Record a successful completion. Never downgrades a better result.</summary>
        public void RecordCompletion(int levelId, Rating rating, int moves)
        {
            if (levelId == 0)
            {
                MarkTutorialDone();
                return;
            }

            var rec = Data.GetOrCreateRecord(levelId);
            rec.completed = true;
            if ((int)rating > rec.bestRating) rec.bestRating = (int)rating;
            if (moves >= 0 && moves < rec.bestMoves) rec.bestMoves = moves;

            if (levelId >= 1 && levelId < LastLevelId)
                Data.highestUnlockedId = Mathf.Max(Data.highestUnlockedId, levelId + 1);

            Save();
        }

        public void MarkFinaleShown()
        {
            if (Data.finaleShown) return;
            Data.finaleShown = true;
            Save();
        }

        // ---- settings ----

        public void SetAudio(float music, float sfx)
        {
            Data.musicVolume = Mathf.Clamp01(music);
            Data.sfxVolume = Mathf.Clamp01(sfx);
            Save();
        }

        public void SetHaptics(bool on)
        {
            Data.haptics = on;
            Save();
        }

        /// <summary>Full progression + settings reset. Caller must confirm first (§75).</summary>
        public void ResetProgress()
        {
            Data = new SaveData();
            Save();
        }

        void Backup(string suffix)
        {
            try
            {
                var dst = _path + suffix;
                if (File.Exists(dst)) File.Delete(dst);
                File.Copy(_path, dst);
            }
            catch { /* best effort */ }
        }
    }
}
