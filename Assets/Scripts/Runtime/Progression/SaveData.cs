using System;
using System.Collections.Generic;
using StencilRelay.Levels;

namespace StencilRelay.Progression
{
    [Serializable]
    public sealed class LevelRecord
    {
        public int levelId;
        public bool completed;
        public int bestRating;              // (int)Rating; 0 = none
        public int bestMoves = int.MaxValue;

        public Rating BestRating => (Rating)bestRating;
    }

    [Serializable]
    public sealed class SaveData
    {
        public int version = SaveSystem.CurrentVersion;

        public bool tutorialDone;
        public int highestUnlockedId;       // 0..12  (0 = only Calibration available)
        public bool finaleShown;            // MASTER ALIGNMENT COMPLETE already played

        public List<LevelRecord> records = new List<LevelRecord>();

        public float musicVolume = 0.8f;
        public float sfxVolume = 0.9f;
        public bool haptics = true;

        public bool IsUnlocked(int levelId) => levelId <= 0 || levelId <= highestUnlockedId;

        public LevelRecord GetRecord(int levelId)
            => records.Find(r => r.levelId == levelId);

        public LevelRecord GetOrCreateRecord(int levelId)
        {
            var r = GetRecord(levelId);
            if (r == null)
            {
                r = new LevelRecord { levelId = levelId };
                records.Add(r);
            }
            return r;
        }
    }
}
