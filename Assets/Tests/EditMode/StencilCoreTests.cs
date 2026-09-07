using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using StencilRelay.Core;

namespace StencilRelay.Tests
{
    public class StencilCoreTests
    {
        static Vector2Int V(int x, int y) => new Vector2Int(x, y);

        // ---- CellGrid ----

        [Test]
        public void CellGrid_AndNot_ClearsOverlap()
        {
            var a = CellGrid.FromCells(3, 3, new[] { V(0, 0), V(1, 1), V(2, 2) });
            var b = CellGrid.FromCells(3, 3, new[] { V(1, 1) });
            a.AndNot(b);
            Assert.IsTrue(a.Get(0, 0));
            Assert.IsFalse(a.Get(1, 1));
            Assert.IsTrue(a.Get(2, 2));
            Assert.AreEqual(2, a.Count());
        }

        [Test]
        public void CellGrid_And_KeepsOnlyCommon()
        {
            var a = CellGrid.FromCells(2, 2, new[] { V(0, 0), V(1, 0) });
            var b = CellGrid.FromCells(2, 2, new[] { V(1, 0), V(1, 1) });
            a.And(b);
            Assert.IsTrue(a.ContentEquals(CellGrid.FromCells(2, 2, new[] { V(1, 0) })));
        }

        [Test]
        public void CellGrid_SizeMismatch_Throws()
        {
            var a = new CellGrid(2, 2);
            var b = new CellGrid(3, 2);
            Assert.Throws<System.ArgumentException>(() => a.And(b));
        }

        // ---- Rotation ----

        [Test]
        public void RotateCell_KnownMappings_ForAsymmetricFootprint()
        {
            // Footprint 3x2, open corner at (0,0).
            Assert.AreEqual(V(0, 0), CellGrid.RotateCell(V(0, 0), Rot90.None, 3, 2));
            Assert.AreEqual(V(0, 2), CellGrid.RotateCell(V(0, 0), Rot90.CW90, 3, 2));
            Assert.AreEqual(V(2, 1), CellGrid.RotateCell(V(0, 0), Rot90.CW180, 3, 2));
            Assert.AreEqual(V(1, 0), CellGrid.RotateCell(V(0, 0), Rot90.CW270, 3, 2));
        }

        [Test]
        public void RotatedFootprint_SwapsAxesForQuarterTurns()
        {
            Assert.AreEqual(V(3, 2), CellGrid.RotatedFootprint(Rot90.None, 3, 2));
            Assert.AreEqual(V(2, 3), CellGrid.RotatedFootprint(Rot90.CW90, 3, 2));
            Assert.AreEqual(V(3, 2), CellGrid.RotatedFootprint(Rot90.CW180, 3, 2));
            Assert.AreEqual(V(2, 3), CellGrid.RotatedFootprint(Rot90.CW270, 3, 2));
        }

        [Test]
        public void UnrotateCell_InvertsRotateCell_ForAllRotations()
        {
            foreach (var rot in new[] { Rot90.None, Rot90.CW90, Rot90.CW180, Rot90.CW270 })
            for (int x = 0; x < 4; x++)
            for (int y = 0; y < 3; y++)
            {
                var local = V(x, y);
                var rotated = CellGrid.RotateCell(local, rot, 4, 3);
                Assert.AreEqual(local, CellGrid.UnrotateCell(rotated, rot, 4, 3), $"rot {rot} cell {local}");
            }
        }

        [Test]
        public void FourRotations_ReturnToStart()
        {
            var start = V(2, 1);
            var c = start;
            // Applying CW90 four times about the same footprint returns to origin,
            // alternating footprint dims each step.
            c = CellGrid.RotateCell(c, Rot90.CW90, 4, 3);
            c = CellGrid.RotateCell(c, Rot90.CW90, 3, 4);
            c = CellGrid.RotateCell(c, Rot90.CW90, 4, 3);
            c = CellGrid.RotateCell(c, Rot90.CW90, 3, 4);
            Assert.AreEqual(start, c);
        }

        // ---- StencilShape flip ----

        [Test]
        public void Shape_Flip_MirrorsFrontWhenNoDistinctBack()
        {
            var shape = new StencilShape(3, 1, new[] { V(0, 0) });
            CollectionAssert.AreEquivalent(new[] { V(0, 0) }, shape.OpenCells(false));
            CollectionAssert.AreEquivalent(new[] { V(2, 0) }, shape.OpenCells(true));
        }

        [Test]
        public void Shape_Flip_UsesDistinctBackVerbatim()
        {
            var shape = new StencilShape(3, 1, new[] { V(0, 0) }, backOpen: new[] { V(1, 0) });
            CollectionAssert.AreEquivalent(new[] { V(1, 0) }, shape.OpenCells(true));
        }

        // ---- StencilPlate resolve ----

        [Test]
        public void Plate_ResolveOpaque_IsFootprintMinusHoles_Translated()
        {
            var shape = new StencilShape(2, 2, new[] { V(0, 0) });
            var plate = new StencilPlate("A", shape, new PlateTransform(V(1, 1)));

            var opaque = plate.ResolveOpaque(4, 4);
            Assert.IsTrue(opaque.ContentEquals(
                CellGrid.FromCells(4, 4, new[] { V(2, 1), V(1, 2), V(2, 2) })));

            var open = plate.ResolveOpenOnBoard(4, 4);
            Assert.IsTrue(open.ContentEquals(CellGrid.FromCells(4, 4, new[] { V(1, 1) })));
        }

        [Test]
        public void Plate_PartlyOffBoard_ClipsCleanly()
        {
            var shape = new StencilShape(2, 2, new Vector2Int[0]); // solid
            var plate = new StencilPlate("A", shape, new PlateTransform(V(-1, -1)));
            var opaque = plate.ResolveOpaque(4, 4);
            Assert.IsTrue(opaque.ContentEquals(CellGrid.FromCells(4, 4, new[] { V(0, 0) })));
        }

        // ---- Composition ----

        static (int w, int h, LightProjector proj) FullBoard(int w, int h)
        {
            return (w, h, LightProjector.Full("P", w, h));
        }

        [Test]
        public void Composite_TwoPlates_IsIntersectionOfHoles()
        {
            var p1 = new StencilPlate("A", new StencilShape(2, 2, new[] { V(0, 0) }), new PlateTransform(V(1, 1)));
            var p2 = new StencilPlate("B", new StencilShape(4, 4, new[] { V(1, 1) }), new PlateTransform(V(0, 0)));
            var (w, h, proj) = FullBoard(4, 4);

            var light = StencilLayerStack.Composite(proj, new[] { p1, p2 }, w, h);
            Assert.IsTrue(light.ContentEquals(CellGrid.FromCells(4, 4, new[] { V(1, 1) })));
        }

        [Test]
        public void Composite_IsOrderIndependent()
        {
            var p1 = new StencilPlate("A", new StencilShape(4, 4, new[] { V(1, 1), V(2, 2) }), new PlateTransform(V(0, 0)));
            var p2 = new StencilPlate("B", new StencilShape(4, 4, new[] { V(1, 1), V(3, 3) }), new PlateTransform(V(0, 0)));
            var (w, h, proj) = FullBoard(4, 4);

            var ab = StencilLayerStack.Composite(proj, new[] { p1, p2 }, w, h);
            var ba = StencilLayerStack.Composite(proj, new[] { p2, p1 }, w, h);
            Assert.IsTrue(ab.ContentEquals(ba));
            Assert.IsTrue(ab.ContentEquals(CellGrid.FromCells(4, 4, new[] { V(1, 1) })));
        }

        // ---- Evaluation ----

        static List<SensorSpec> Sensors(params SensorSpec[] s) => new List<SensorSpec>(s);

        static SensorSpec Target(string label, Vector2Int cell, string proj = "P")
            => new SensorSpec { Label = label, Kind = ConditionKind.RequiredTarget, ProjectorId = proj, Cells = { cell } };

        static SensorSpec Forbidden(string label, Vector2Int cell, string proj = "P")
            => new SensorSpec { Label = label, Kind = ConditionKind.ForbiddenSensor, ProjectorId = proj, Cells = { cell } };

        [Test]
        public void Evaluate_RequiredTargetLit_Succeeds()
        {
            var plate = new StencilPlate("A", new StencilShape(2, 2, new[] { V(0, 0) }), new PlateTransform(V(1, 1)));
            var res = ProjectionEvaluator.Evaluate(4, 4,
                new[] { LightProjector.Full("P", 4, 4) },
                Sensors(Target("T", V(1, 1))),
                new[] { plate });

            Assert.IsTrue(res.Success);
            Assert.IsEmpty(res.FailureReasons);
        }

        [Test]
        public void Evaluate_RequiredTargetBlocked_FailsWithReason()
        {
            var plate = new StencilPlate("A", new StencilShape(2, 2, new[] { V(0, 0) }), new PlateTransform(V(1, 1)));
            var res = ProjectionEvaluator.Evaluate(4, 4,
                new[] { LightProjector.Full("P", 4, 4) },
                Sensors(Target("T", V(2, 2))), // blocked cell
                new[] { plate });

            Assert.IsFalse(res.Success);
            CollectionAssert.Contains(new List<string>(res.FailureReasons), "Target Unlit (T)");
        }

        [Test]
        public void Evaluate_ForbiddenSensorLit_Fails()
        {
            var plate = new StencilPlate("A", new StencilShape(2, 2, new[] { V(0, 0) }), new PlateTransform(V(1, 1)));
            var res = ProjectionEvaluator.Evaluate(4, 4,
                new[] { LightProjector.Full("P", 4, 4) },
                Sensors(Target("T", V(1, 1)), Forbidden("X", V(0, 0))),
                new[] { plate });

            Assert.IsFalse(res.Success);
            CollectionAssert.Contains(new List<string>(res.FailureReasons), "Forbidden Sensor Lit (X)");
        }

        [Test]
        public void Evaluate_IsDeterministic_RepeatedCalls()
        {
            StencilPlate Plate() => new StencilPlate("A",
                new StencilShape(3, 3, new[] { V(0, 0), V(1, 1) }), new PlateTransform(V(1, 1), Rot90.CW90));

            ProjectionResult Run() => ProjectionEvaluator.Evaluate(6, 6,
                new[] { LightProjector.Full("P", 6, 6) },
                Sensors(Target("T", V(2, 2)), Forbidden("X", V(5, 5))),
                new[] { Plate() });

            var a = Run();
            var b = Run();
            Assert.AreEqual(a.Success, b.Success);
            Assert.IsTrue(a.LightByProjector["P"].ContentEquals(b.LightByProjector["P"]));
        }

        [Test]
        public void Evaluate_Filter_RequiresLightToCarryCategory()
        {
            // Plate A: 4x4, one open cell at (1,1) tagged filter category 2.
            var frontFilter = new Dictionary<Vector2Int, int> { { V(1, 1), 2 } };
            var shape = new StencilShape(4, 4, new[] { V(1, 1) }, null, frontFilter);
            var plate = new StencilPlate("A", shape, new PlateTransform(V(1, 1))); // hole -> board (2,2)

            var projectors = new[] { LightProjector.Full("P", 4, 4) };

            // Target at (2,2) requiring category 2 -> pass.
            var okTarget = new SensorSpec { Label = "T", Kind = ConditionKind.RequiredTarget, ProjectorId = "P", Cells = { V(2, 2) }, RequiredFilter = 2 };
            Assert.IsTrue(ProjectionEvaluator.Evaluate(4, 4, projectors, Sensors(okTarget), new[] { plate }).Success);

            // Same target requiring category 3 -> Filter Mismatch.
            var badTarget = new SensorSpec { Label = "T", Kind = ConditionKind.RequiredTarget, ProjectorId = "P", Cells = { V(2, 2) }, RequiredFilter = 3 };
            var res = ProjectionEvaluator.Evaluate(4, 4, projectors, Sensors(badTarget), new[] { plate });
            Assert.IsFalse(res.Success);
            CollectionAssert.Contains(new List<string>(res.FailureReasons), "Filter Mismatch (T)");
        }

        [Test]
        public void Evaluate_DualProjector_LabelsFailingProjector()
        {
            // Two projectors; plate blocks P2's target only.
            var plate = new StencilPlate("A", new StencilShape(4, 4, new[] { V(0, 0) }), new PlateTransform(V(0, 0)));
            var res = ProjectionEvaluator.Evaluate(4, 4,
                new[] { LightProjector.Full("P1", 4, 4), LightProjector.Full("P2", 4, 4) },
                Sensors(Target("T1", V(0, 0), "P1"), Target("T2", V(3, 3), "P2")),
                new[] { plate });

            Assert.IsFalse(res.Success);
            CollectionAssert.Contains(new List<string>(res.FailureReasons), "Target Unlit (T2, P2)");
        }

        // ---- PlatePermissions ----

        [Test]
        public void Permissions_EnumeratePlacements_CoversOffsetsRotationsFlips()
        {
            var perms = new PlatePermissions
            {
                AllowedOffsets = { V(0, 0), V(1, 0) },
                AllowedRotations = { Rot90.None, Rot90.CW90 },
                FlipAllowed = true
            };
            var all = new List<PlateTransform>(perms.EnumeratePlacements(new PlateTransform(V(0, 0))));
            Assert.AreEqual(2 * 2 * 2, all.Count);
        }
    }
}
