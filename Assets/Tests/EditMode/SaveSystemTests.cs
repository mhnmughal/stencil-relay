using System.IO;
using NUnit.Framework;
using UnityEngine;
using StencilRelay.Levels;
using StencilRelay.Progression;

namespace StencilRelay.Tests
{
    public class SaveSystemTests
    {
        string _path;

        [SetUp]
        public void SetUp()
        {
            _path = Path.Combine(Application.temporaryCachePath, "srtest_" + System.Guid.NewGuid().ToString("N") + ".save");
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var f in Directory.GetFiles(Path.GetDirectoryName(_path), Path.GetFileNameWithoutExtension(_path) + "*"))
                File.Delete(f);
        }

        [Test]
        public void FirstLaunch_NoFile_GivesDefaults()
        {
            var s = new SaveSystem(_path);
            Assert.IsFalse(s.Data.tutorialDone);
            Assert.AreEqual(0, s.Data.highestUnlockedId);
            Assert.IsTrue(s.Data.IsUnlocked(0));
            Assert.IsFalse(s.Data.IsUnlocked(1));
        }

        [Test]
        public void Tutorial_Completion_UnlocksLevel1_AndPersists()
        {
            new SaveSystem(_path).RecordCompletion(0, Rating.Aligned, 3);
            var reloaded = new SaveSystem(_path);
            Assert.IsTrue(reloaded.Data.tutorialDone);
            Assert.IsTrue(reloaded.Data.IsUnlocked(1));
            Assert.IsFalse(reloaded.Data.IsUnlocked(2));
        }

        [Test]
        public void Completion_UnlocksNext_AndRecordsRatingAndMoves()
        {
            var s = new SaveSystem(_path);
            s.RecordCompletion(1, Rating.Precise, 4);
            Assert.IsTrue(s.Data.IsUnlocked(2));
            var rec = s.Data.GetRecord(1);
            Assert.IsTrue(rec.completed);
            Assert.AreEqual(Rating.Precise, rec.BestRating);
            Assert.AreEqual(4, rec.bestMoves);
        }

        [Test]
        public void Replay_NeverDowngradesRatingOrMoves()
        {
            var s = new SaveSystem(_path);
            s.RecordCompletion(3, Rating.PerfectRegister, 2);
            s.RecordCompletion(3, Rating.Aligned, 9);   // worse replay
            var rec = s.Data.GetRecord(3);
            Assert.AreEqual(Rating.PerfectRegister, rec.BestRating);
            Assert.AreEqual(2, rec.bestMoves);
        }

        [Test]
        public void Replay_ImprovesWhenBetter()
        {
            var s = new SaveSystem(_path);
            s.RecordCompletion(3, Rating.Aligned, 9);
            s.RecordCompletion(3, Rating.PerfectRegister, 2);
            var rec = s.Data.GetRecord(3);
            Assert.AreEqual(Rating.PerfectRegister, rec.BestRating);
            Assert.AreEqual(2, rec.bestMoves);
        }

        [Test]
        public void Level12_Completion_DoesNotUnlockBeyond()
        {
            var s = new SaveSystem(_path);
            s.Data.highestUnlockedId = 12;
            s.RecordCompletion(12, Rating.Aligned, 5);
            Assert.AreEqual(12, s.Data.highestUnlockedId);
        }

        [Test]
        public void CorruptFile_IsBackedUp_AndReset()
        {
            File.WriteAllText(_path, "{ not valid json ]");
            var s = new SaveSystem(_path);
            Assert.IsFalse(s.Data.tutorialDone);
            Assert.IsTrue(File.Exists(_path + ".corrupt"));
        }

        [Test]
        public void NewerVersionFile_IsBackedUp_AndReset()
        {
            var future = new SaveData { version = 999, tutorialDone = true, highestUnlockedId = 7 };
            File.WriteAllText(_path, JsonUtility.ToJson(future));
            var s = new SaveSystem(_path);
            Assert.IsFalse(s.Data.tutorialDone);
            Assert.AreEqual(0, s.Data.highestUnlockedId);
            Assert.IsTrue(File.Exists(_path + ".v999"));
        }

        [Test]
        public void Settings_PersistAndClamp()
        {
            new SaveSystem(_path).SetAudio(1.5f, -0.2f);
            var s = new SaveSystem(_path);
            Assert.AreEqual(1f, s.Data.musicVolume);
            Assert.AreEqual(0f, s.Data.sfxVolume);
        }

        [Test]
        public void ResetProgress_ClearsEverything()
        {
            var s = new SaveSystem(_path);
            s.RecordCompletion(1, Rating.Precise, 3);
            s.SetHaptics(false);
            s.ResetProgress();
            Assert.IsEmpty(s.Data.records);
            Assert.AreEqual(0, s.Data.highestUnlockedId);
            Assert.IsTrue(s.Data.haptics);
            var reloaded = new SaveSystem(_path);
            Assert.IsEmpty(reloaded.Data.records);
        }
    }
}
