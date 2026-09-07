using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using StencilRelay.Core;
using StencilRelay.Levels;

namespace StencilRelay.Tests
{
    public class LevelManipulationTests
    {
        static Vector2Int V(int x, int y) => new Vector2Int(x, y);

        static StencilShapeAsset Sheet(Vector2Int hole)
        {
            var s = ScriptableObject.CreateInstance<StencilShapeAsset>();
            s.footprintW = 6; s.footprintH = 6;
            s.frontOpen = new List<Vector2Int> { hole };
            return s;
        }

        // 6x6 board, one sheet, hole at (1,1); optimal slides (0,0)->(2,2) to light (3,3).
        static StencilLevelDefinition OnePlateLevel()
        {
            var def = ScriptableObject.CreateInstance<StencilLevelDefinition>();
            def.levelId = 1;
            def.boardSize = V(6, 6);
            def.projectors = new List<ProjectorEntry> { new ProjectorEntry { id = "P", fullBoardAperture = true } };
            def.sensors = new List<SensorEntry>
            {
                new SensorEntry { label = "T", kind = ConditionKind.RequiredTarget, projectorId = "P", cells = { V(3, 3) } }
            };
            def.movePar = 1;
            def.plates = new List<PlateEntry>
            {
                new PlateEntry
                {
                    id = "A",
                    shape = Sheet(V(1, 1)),
                    initialOffset = V(0, 0),
                    allowedOffsets = new List<Vector2Int> { V(0, 0), V(2, 2), V(1, 1) },
                    allowedRotations = new List<Rot90> { Rot90.None, Rot90.CW90, Rot90.CW180, Rot90.CW270 },
                    flipAllowed = true,
                    optimalOffset = V(2, 2),
                }
            };
            return def;
        }

        [Test]
        public void Slide_ToAllowedOffset_Counts_And_Lights()
        {
            var m = new LevelManipulation(OnePlateLevel().BuildRuntime());
            Assert.IsFalse(m.Runtime.Evaluate().Success);
            Assert.IsTrue(m.TrySlide(0, V(2, 2)));
            Assert.AreEqual(1, m.MoveCount);
            Assert.IsTrue(m.Runtime.Evaluate().Success);
        }

        [Test]
        public void Slide_ToDisallowedOffset_Rejected_NoMove()
        {
            var m = new LevelManipulation(OnePlateLevel().BuildRuntime());
            Assert.IsFalse(m.TrySlide(0, V(5, 5)));
            Assert.AreEqual(0, m.MoveCount);
        }

        [Test]
        public void Slide_ToSameOffset_IsNoOp()
        {
            var m = new LevelManipulation(OnePlateLevel().BuildRuntime());
            Assert.IsFalse(m.TrySlide(0, V(0, 0)));
            Assert.AreEqual(0, m.MoveCount);
        }

        [Test]
        public void SlideToNearest_SnapsToClosestAllowed()
        {
            var m = new LevelManipulation(OnePlateLevel().BuildRuntime());
            Assert.IsTrue(m.TrySlideToNearest(0, new Vector2(1.9f, 2.1f)));
            Assert.AreEqual(V(2, 2), m.Plate(0).Transform.Offset);
        }

        [Test]
        public void RotateStep_CyclesThroughAllowedRotations()
        {
            var m = new LevelManipulation(OnePlateLevel().BuildRuntime());
            Assert.IsTrue(m.RotateStep(0));
            Assert.AreEqual(Rot90.CW90, m.Plate(0).Transform.Rotation);
            m.RotateStep(0); m.RotateStep(0); m.RotateStep(0);
            Assert.AreEqual(Rot90.None, m.Plate(0).Transform.Rotation);
            Assert.AreEqual(4, m.MoveCount);
        }

        [Test]
        public void Flip_Toggles_WhenAllowed_AndCounts()
        {
            var m = new LevelManipulation(OnePlateLevel().BuildRuntime());
            Assert.IsTrue(m.Flip(0));
            Assert.IsTrue(m.Plate(0).Transform.Flipped);
            Assert.AreEqual(1, m.MoveCount);
        }

        [Test]
        public void Flip_Rejected_WhenNotAllowed()
        {
            var def = OnePlateLevel();
            def.plates[0].flipAllowed = false;
            var m = new LevelManipulation(def.BuildRuntime());
            Assert.IsFalse(m.Flip(0));
            Assert.AreEqual(0, m.MoveCount);
        }

        [Test]
        public void ResetPlate_RestoresInitial_AndRefundsMoves()
        {
            var m = new LevelManipulation(OnePlateLevel().BuildRuntime());
            m.TrySlide(0, V(2, 2));
            m.RotateStep(0);
            Assert.AreEqual(2, m.MoveCount);
            m.ResetPlate(0);
            Assert.AreEqual(V(0, 0), m.Plate(0).Transform.Offset);
            Assert.AreEqual(Rot90.None, m.Plate(0).Transform.Rotation);
            Assert.AreEqual(0, m.MoveCount);
        }

        [Test]
        public void Project_LocksManipulation_UntilUnlock()
        {
            var m = new LevelManipulation(OnePlateLevel().BuildRuntime());
            m.Project();
            Assert.IsTrue(m.Locked);
            Assert.IsFalse(m.TrySlide(0, V(2, 2)));
            m.Unlock();
            Assert.IsTrue(m.TrySlide(0, V(2, 2)));
        }

        static StencilShapeAsset TwoHoleSheet()
        {
            var s = ScriptableObject.CreateInstance<StencilShapeAsset>();
            s.footprintW = 6; s.footprintH = 6;
            s.frontOpen = new List<Vector2Int> { V(1, 1), V(3, 1) };
            return s;
        }

        // Target (3,3); offsets (2,2) and (0,2) both light it. Optimal = (2,2).
        static StencilLevelDefinition TwoWayLevel()
        {
            var def = OnePlateLevel();
            def.plates[0].shape = TwoHoleSheet();
            def.plates[0].allowedOffsets = new List<Vector2Int> { V(0, 0), V(2, 2), V(0, 2) };
            def.plates[0].allowedRotations = new List<Rot90> { Rot90.None, Rot90.CW90, Rot90.CW180, Rot90.CW270 };
            def.plates[0].optimalOffset = V(2, 2);
            def.movePar = 1;
            return def;
        }

        [Test]
        public void Rating_PerfectRegister_WhenTransformMatchesOptimal()
        {
            var m = new LevelManipulation(TwoWayLevel().BuildRuntime());
            Assert.IsTrue(m.TrySlide(0, V(2, 2)));
            Assert.IsTrue(m.Project().Success);
            Assert.AreEqual(Rating.PerfectRegister, m.RateSuccess());
        }

        [Test]
        public void Rating_Precise_WithinPar_ButNotOptimalConfig()
        {
            var m = new LevelManipulation(TwoWayLevel().BuildRuntime());
            Assert.IsTrue(m.TrySlide(0, V(0, 2))); // solves, 1 move == par, not the optimal offset
            Assert.IsTrue(m.Project().Success);
            Assert.AreEqual(Rating.Precise, m.RateSuccess());
        }

        [Test]
        public void Rating_Aligned_WhenSolvedOverParAndNotOptimal()
        {
            var m = new LevelManipulation(TwoWayLevel().BuildRuntime());
            Assert.IsTrue(m.TrySlide(0, V(2, 2)));  // move 1 (would be optimal)
            Assert.IsTrue(m.TrySlide(0, V(0, 2)));  // move 2 - still solves, offset now non-optimal
            Assert.AreEqual(2, m.MoveCount);        // par is 1
            Assert.IsTrue(m.Project().Success);
            Assert.AreEqual(Rating.Aligned, m.RateSuccess());
        }

        [Test]
        public void Rating_Aligned_ForTutorial_Always()
        {
            var def = TwoWayLevel();
            def.levelId = 0;
            def.isTutorial = true;
            def.tutorialSteps = new List<TutorialStep> { new TutorialStep { instruction = "x" } };
            var m = new LevelManipulation(def.BuildRuntime());
            m.TrySlide(0, V(2, 2));
            m.Project();
            Assert.AreEqual(Rating.Aligned, m.RateSuccess());
        }
    }
}
