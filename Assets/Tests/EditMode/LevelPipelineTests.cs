using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using StencilRelay.Core;
using StencilRelay.Levels;

namespace StencilRelay.Tests
{
    public class LevelPipelineTests
    {
        static Vector2Int V(int x, int y) => new Vector2Int(x, y);

        static StencilShapeAsset SheetWithHole(int w, int h, Vector2Int hole)
        {
            var s = ScriptableObject.CreateInstance<StencilShapeAsset>();
            s.footprintW = w; s.footprintH = h;
            s.frontOpen = new List<Vector2Int> { hole };
            return s;
        }

        /// <summary>A minimal solvable one-plate level: 6x6 sheet, hole slides onto the target.</summary>
        static StencilLevelDefinition SolvableLevel(int id = 1)
        {
            var def = ScriptableObject.CreateInstance<StencilLevelDefinition>();
            def.levelId = id;
            def.displayTitle = "Synthetic";
            def.set = LevelSet.Alignment;
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
                    shape = SheetWithHole(6, 6, V(1, 1)),
                    initialOffset = V(0, 0),
                    allowedOffsets = new List<Vector2Int> { V(0, 0), V(2, 2) },
                    optimalOffset = V(2, 2),
                }
            };
            return def;
        }

        [Test]
        public void BuildRuntime_And_BuildAtOptimal_Differ()
        {
            var def = SolvableLevel();
            Assert.IsFalse(def.BuildRuntime().Evaluate().Success, "initial should not solve");
            Assert.IsTrue(def.BuildAtOptimal().Evaluate().Success, "optimal should solve");
        }

        [Test]
        public void Solver_FindsSolution_ForSolvableLevel()
        {
            var report = LevelSolver.Solve(SolvableLevel().BuildRuntime());
            Assert.AreEqual(SolveStatus.Solved, report.Status);
            Assert.GreaterOrEqual(report.SolutionsFound, 1);
        }

        [Test]
        public void Solver_ReportsUnsolvable_WhenNoPlacementWorks()
        {
            var def = SolvableLevel();
            def.plates[0].allowedOffsets = new List<Vector2Int> { V(0, 0) }; // hole never reaches (3,3)
            def.plates[0].optimalOffset = V(0, 0);
            var report = LevelSolver.Solve(def.BuildRuntime());
            Assert.AreEqual(SolveStatus.Unsolvable, report.Status);
        }

        [Test]
        public void Solver_MinMovesToOptimal_CountsSlideRotateFlip()
        {
            var def = SolvableLevel();
            def.plates[0].initialRotation = Rot90.None;
            def.plates[0].optimalRotation = Rot90.CW180;   // 2 quarter turns
            def.plates[0].initialFlipped = false;
            def.plates[0].optimalFlipped = true;           // 1 flip
            def.plates[0].flipAllowed = true;
            def.plates[0].allowedRotations = new List<Rot90> { Rot90.None, Rot90.CW90, Rot90.CW180, Rot90.CW270 };
            // offset also changes (0,0)->(2,2) => 1
            Assert.AreEqual(1 + 2 + 1, def.MinMovesToOptimal());
        }

        [Test]
        public void Validation_CleanLevel_ProducesNoErrors_ForThatLevel()
        {
            var db = ScriptableObject.CreateInstance<LevelDatabase>();
            db.levels = new List<StencilLevelDefinition> { SolvableLevel(1) };
            var report = LevelValidation.Validate(db);
            // DB-structure errors (missing 0,2..12) are expected; the level itself must be clean.
            Assert.IsEmpty(report.Issues.Where(i => i.IsError && i.LevelId == 1),
                string.Join("\n", report.Issues.Where(i => i.LevelId == 1).Select(i => i.ToString())));
        }

        [Test]
        public void Validation_Flags_OptimalThatDoesNotSolve()
        {
            var def = SolvableLevel(1);
            def.plates[0].optimalOffset = V(0, 0); // does not light the target
            var db = ScriptableObject.CreateInstance<LevelDatabase>();
            db.levels = new List<StencilLevelDefinition> { def };
            var report = LevelValidation.Validate(db);
            Assert.IsTrue(report.Issues.Any(i => i.IsError && i.LevelId == 1 && i.Message.Contains("optimal")));
        }

        [Test]
        public void Validation_Flags_MoveParBelowMinimum()
        {
            var def = SolvableLevel(1);
            def.movePar = 0; // min is 1 (offset changes)
            var db = ScriptableObject.CreateInstance<LevelDatabase>();
            db.levels = new List<StencilLevelDefinition> { def };
            var report = LevelValidation.Validate(db);
            Assert.IsTrue(report.Issues.Any(i => i.IsError && i.LevelId == 1 && i.Message.Contains("movePar")));
        }

        [Test]
        public void Validation_Flags_TutorialFlagOnNonZeroLevel()
        {
            var def = SolvableLevel(3);
            def.isTutorial = true;
            var db = ScriptableObject.CreateInstance<LevelDatabase>();
            db.levels = new List<StencilLevelDefinition> { def };
            var report = LevelValidation.Validate(db);
            Assert.IsTrue(report.Issues.Any(i => i.IsError && i.LevelId == 3 && i.Message.Contains("tutorial")));
        }

        [Test]
        public void Validation_Flags_Level11_WithoutDualProjectors()
        {
            var def = SolvableLevel(11);
            var db = ScriptableObject.CreateInstance<LevelDatabase>();
            db.levels = new List<StencilLevelDefinition> { def };
            var report = LevelValidation.Validate(db);
            Assert.IsTrue(report.Issues.Any(i => i.IsError && i.LevelId == 11 && i.Message.Contains("projector")));
        }

        [Test]
        public void RealDatabase_Validates_13_of_13()
        {
            var db = Resources.Load<LevelDatabase>("LevelDatabase");
            Assert.IsNotNull(db, "LevelDatabase not found in Resources");
            var report = LevelValidation.Validate(db);
            Assert.IsTrue(report.AllValid,
                report.Headline + "\n" + string.Join("\n", report.Errors.Select(e => e.ToString())));
            Assert.AreEqual("13 / 13 VALID", report.Headline);
        }

        [Test]
        public void RealDatabase_EveryLevel_OptimalSolves_And_SolverConfirms()
        {
            var db = Resources.Load<LevelDatabase>("LevelDatabase");
            foreach (var def in db.Ordered)
            {
                var opt = def.BuildAtOptimal().Evaluate();
                Assert.IsTrue(opt.Success, $"L{def.levelId} '{def.displayTitle}' optimal fails: {string.Join(", ", opt.FailureReasons)}");

                var solve = LevelSolver.Solve(def.BuildRuntime());
                Assert.AreNotEqual(SolveStatus.Unsolvable, solve.Status, $"L{def.levelId} unsolvable");

                if (!def.isTutorial)
                    Assert.GreaterOrEqual(def.movePar, def.MinMovesToOptimal(), $"L{def.levelId} movePar below minimum");
            }
        }

        [Test]
        public void RealDatabase_InitialState_DoesNotSolve_ForNonTrivialLevels()
        {
            var db = Resources.Load<LevelDatabase>("LevelDatabase");
            foreach (var def in db.Ordered)
            {
                // every authored level should require at least one manipulation
                var initial = def.BuildRuntime().Evaluate();
                Assert.IsFalse(initial.Success, $"L{def.levelId} '{def.displayTitle}' is already solved at start");
            }
        }

        [Test]
        public void FullDatabase_Of13_Reports_AllValid_WhenEachLevelClean()
        {
            var db = ScriptableObject.CreateInstance<LevelDatabase>();
            db.levels = new List<StencilLevelDefinition>();
            for (int id = 0; id <= 12; id++)
            {
                var def = SolvableLevel(id);
                if (id == 0)
                {
                    def.isTutorial = true;
                    def.movePar = 0;
                    def.tutorialSteps = new List<TutorialStep> { new TutorialStep { instruction = "Select." } };
                }
                if (id == 11)
                {
                    // add a second projector + target so the dual-projection rule passes
                    def.projectors.Add(new ProjectorEntry { id = "P2", fullBoardAperture = true });
                    def.sensors.Add(new SensorEntry { label = "T2", kind = ConditionKind.RequiredTarget, projectorId = "P2", cells = { V(3, 3) } });
                }
                db.levels.Add(def);
            }
            var report = LevelValidation.Validate(db);
            Assert.IsTrue(report.AllValid, report.Headline + "\n" + string.Join("\n", report.Errors.Select(e => e.ToString())));
            Assert.AreEqual("13 / 13 VALID", report.Headline);
        }
    }
}
