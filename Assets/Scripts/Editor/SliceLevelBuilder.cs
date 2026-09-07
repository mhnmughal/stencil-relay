using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using StencilRelay.Core;
using StencilRelay.Levels;

namespace StencilRelay.Editor
{
    /// <summary>
    /// Reproducibly (re)generates every level asset (0-12) + the shape assets + the
    /// LevelDatabase. Shapes and allowed placements are authored here; the SOLVER then
    /// picks each level's optimal configuration and the move par is derived from it, so
    /// levels can't silently become unsolvable. Assets stay hand-editable afterwards.
    ///
    /// Menu: Stencil Relay/Rebuild All Levels
    /// </summary>
    public static class SliceLevelBuilder
    {
        const string ContentRoot = "Assets/Content";
        const string ShapesDir = ContentRoot + "/Shapes";
        const string LevelsDir = ContentRoot + "/Levels";
        const string DbPath = ContentRoot + "/Resources/LevelDatabase.asset";

        static Vector2Int V(int x, int y) => new Vector2Int(x, y);
        static List<Vector2Int> Offs(params (int x, int y)[] o) => o.Select(t => V(t.x, t.y)).ToList();
        static List<Rot90> Rots(params Rot90[] r) => r.ToList();

        [MenuItem("Stencil Relay/Rebuild All Levels")]
        public static void Rebuild()
        {
            EnsureDir(ContentRoot); EnsureDir(ShapesDir); EnsureDir(LevelsDir);
            EnsureDir(ContentRoot + "/Resources");

            var levels = new List<StencilLevelDefinition>
            {
                BuildLevel0(), BuildLevel1(), BuildLevel2(), BuildLevel3(), BuildLevel4(),
                BuildLevel5(), BuildLevel6(), BuildLevel7(), BuildLevel8(),
                BuildLevel9(), BuildLevel10(), BuildLevel11(), BuildLevel12(),
            };

            // Solve each non-tutorial level, write its optimal + move par.
            foreach (var def in levels)
            {
                var report = LevelSolver.Solve(def.BuildRuntime());
                if (report.Status == SolveStatus.Solved && report.Solution != null)
                {
                    for (int i = 0; i < def.plates.Count && i < report.Solution.Count; i++)
                    {
                        var t = report.Solution[i];
                        def.plates[i].optimalOffset = t.Offset;
                        def.plates[i].optimalRotation = t.Rotation;
                        def.plates[i].optimalFlipped = t.Flipped;
                    }
                    if (!def.isTutorial)
                        def.movePar = def.MinMovesToOptimal() + ParSlack(def.levelId);
                    EditorUtility.SetDirty(def);
                    Debug.Log($"[LevelBuilder] L{def.levelId} '{def.displayTitle}': solved " +
                              $"({report.SolutionsFound}{(report.SolutionsFound >= LevelSolver.MaxSolutionsCounted ? "+" : "")} sol, " +
                              $"space {report.SearchSpace}), par {def.movePar} (min {def.MinMovesToOptimal()}).");
                }
                else
                {
                    Debug.LogError($"[LevelBuilder] L{def.levelId} '{def.displayTitle}': {report.Status} - {report.Message}");
                }
            }

            var db = AssetDatabase.LoadAssetAtPath<LevelDatabase>(DbPath);
            if (db == null) { db = ScriptableObject.CreateInstance<LevelDatabase>(); AssetDatabase.CreateAsset(db, DbPath); }
            db.levels = levels.OrderBy(l => l.levelId).ToList();
            EditorUtility.SetDirty(db);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var v = LevelValidation.Validate(db);
            Debug.Log($"[LevelBuilder] Validation: {v.Headline}");
            foreach (var issue in v.Issues.OrderByDescending(i => i.IsError).ThenBy(i => i.LevelId))
                (issue.IsError ? (System.Action<string>)Debug.LogError : Debug.LogWarning)(issue.ToString());
        }

        static int ParSlack(int levelId) => levelId <= 4 ? 0 : levelId <= 8 ? 1 : 2;

        // ============================================================ SET A

        static StencilLevelDefinition BuildLevel0()
        {
            var sheet = Shape("Shape_L0_SheetHoleRight", 6, 6, Cells((4, 1)));
            var def = NewLevel("Level_00_Calibration", 0, "Calibration", LevelSet.Calibration, V(6, 6));
            def.isTutorial = true;
            def.movePar = 0;
            def.sensors = new List<SensorEntry> { Target("T", V(3, 3)) };
            def.plates = new List<PlateEntry>
            {
                Plate("A", sheet, V(0, 0), Rot90.None,
                    offsets: Offs((0,0),(2,2),(1,2),(2,1)),
                    rots: Rots(Rot90.None, Rot90.CW90)),
            };
            def.tutorialSteps = new List<TutorialStep>
            {
                new TutorialStep { highlightAnchor = "PlateA", instruction = "Select the stencil." },
                new TutorialStep { highlightAnchor = "Board", instruction = "Slide it onto the registration mark." },
                new TutorialStep { highlightAnchor = "RotateDial", instruction = "Rotate the plate." },
                new TutorialStep { highlightAnchor = "ProjectSwitch", instruction = "Project the pattern." },
            };
            return Save(def, "Level_00_Calibration");
        }

        static StencilLevelDefinition BuildLevel1()
        {
            var sheet = Shape("Shape_L1_SheetDot", 6, 6, Cells((1, 1)));
            var def = NewLevel("Level_01_FirstAlignment", 1, "First Alignment", LevelSet.Alignment, V(6, 6));
            def.sensors = new List<SensorEntry> { Target("T", V(3, 3)) };
            def.plates = new List<PlateEntry>
            {
                Plate("A", sheet, V(0, 0), Rot90.None, offsets: Offs((0,0),(2,0),(0,2),(2,2),(1,1))),
            };
            return Save(def, "Level_01_FirstAlignment");
        }

        static StencilLevelDefinition BuildLevel2()
        {
            var h = Shape("Shape_L2_HSlot", 6, 6, Cells((1, 1), (2, 1), (3, 1)));
            var v = Shape("Shape_L2_VSlot", 6, 6, Cells((1, 1), (1, 2), (1, 3)));
            var def = NewLevel("Level_02_DoubleLayer", 2, "Double Layer", LevelSet.Alignment, V(6, 6));
            def.sensors = new List<SensorEntry> { Target("T", V(3, 3)) };
            def.plates = new List<PlateEntry>
            {
                Plate("A", h, V(0, 0), Rot90.None, offsets: Offs((0,0),(2,2),(0,2),(2,0))),
                Plate("B", v, V(0, 0), Rot90.None, offsets: Offs((0,0),(2,2),(2,0),(2,1))),
            };
            return Save(def, "Level_02_DoubleLayer");
        }

        static StencilLevelDefinition BuildLevel3()
        {
            var dom = Shape("Shape_L3_VDomino", 6, 6, Cells((2, 2), (2, 3)));
            var def = NewLevel("Level_03_QuarterTurn", 3, "Quarter Turn", LevelSet.Alignment, V(6, 6));
            def.sensors = new List<SensorEntry> { Target("T", V(3, 3), V(4, 3)) };
            def.plates = new List<PlateEntry>
            {
                Plate("A", dom, V(0, 0), Rot90.None,
                    offsets: Offs((0,0),(1,0),(0,1),(1,1),(2,2)),
                    rots: Rots(Rot90.None, Rot90.CW90)),
            };
            return Save(def, "Level_03_QuarterTurn");
        }

        static StencilLevelDefinition BuildLevel4()
        {
            var a = Shape("Shape_L4_DotLeft", 6, 6, Cells((1, 1)));
            var b = Shape("Shape_L4_DotRight", 6, 6, Cells((4, 1)));
            var def = NewLevel("Level_04_Registration", 4, "Registration", LevelSet.Alignment, V(6, 6));
            def.sensors = new List<SensorEntry> { Target("T", V(3, 3)) };
            def.plates = new List<PlateEntry>
            {
                Plate("A", a, V(0, 0), Rot90.None, offsets: Offs((0,0),(2,2),(1,1),(0,2))),
                Plate("B", b, V(0, 0), Rot90.None, offsets: Offs((0,0),(2,2),(1,1),(2,0)),
                    rots: Rots(Rot90.None, Rot90.CW90)),
            };
            return Save(def, "Level_04_Registration");
        }

        // ============================================================ SET B

        static StencilLevelDefinition BuildLevel5()
        {
            var slot = Shape("Shape_L5_VColumn", 6, 6, Cells((3, 1), (3, 2), (3, 3), (3, 4)));
            var block = Shape("Shape_L5_Blocker", 3, 2, new List<Vector2Int>());
            var def = NewLevel("Level_05_ShadowBlock", 5, "Shadow Block", LevelSet.Occlusion, V(6, 6));
            def.sensors = new List<SensorEntry> { Target("T", V(3, 1)), Forbidden("X", V(3, 4)) };
            def.plates = new List<PlateEntry>
            {
                Plate("A", slot, V(1, 0), Rot90.None, offsets: Offs((0,0),(1,0),(2,0))),
                Plate("B", block, V(0, 0), Rot90.None, offsets: Offs((0,0),(2,3),(0,3),(2,0))),
            };
            return Save(def, "Level_05_ShadowBlock");
        }

        static StencilLevelDefinition BuildLevel6()
        {
            var row = Shape("Shape_L6_HRow", 6, 6, Cells((1, 2), (2, 2), (3, 2), (4, 2), (5, 2)));
            var gate = Shape("Shape_L6_Gate", 6, 6, Cells((1, 3), (5, 3)));
            var def = NewLevel("Level_06_ForbiddenSensor", 6, "Forbidden Sensor", LevelSet.Occlusion, V(6, 6));
            def.sensors = new List<SensorEntry>
            {
                Target("A", V(1, 3)), Target("B", V(5, 3)), Forbidden("C", V(3, 3)),
            };
            def.plates = new List<PlateEntry>
            {
                Plate("A", row, V(0, 0), Rot90.None, offsets: Offs((0,0),(0,1),(0,2))),
                Plate("B", gate, V(2, 0), Rot90.None, offsets: Offs((0,0),(2,0),(0,1),(0,2))),
            };
            return Save(def, "Level_06_ForbiddenSensor");
        }

        static StencilLevelDefinition BuildLevel7()
        {
            var s = Shape("Shape_L7_Reverse", 6, 6,
                front: Cells((1, 0), (1, 1), (1, 2)),
                back: Cells((4, 1)));
            var def = NewLevel("Level_07_ReverseFace", 7, "Reverse Face", LevelSet.Occlusion, V(6, 6));
            def.sensors = new List<SensorEntry>
            {
                Target("T", V(4, 1)), Forbidden("X1", V(4, 0)), Forbidden("X2", V(4, 2)),
            };
            def.plates = new List<PlateEntry>
            {
                Plate("A", s, V(0, 0), Rot90.None, offsets: Offs((0,0),(3,0),(2,1),(0,2)), flip: true),
            };
            return Save(def, "Level_07_ReverseFace");
        }

        static StencilLevelDefinition BuildLevel8()
        {
            var a = Shape("Shape_L8_VDomino", 6, 6, Cells((2, 0), (2, 1)));
            var b = Shape("Shape_L8_FlipDomino", 6, 6,
                front: Cells((2, 0), (2, 1)),
                back: Cells((0, 3), (1, 3)));
            var def = NewLevel("Level_08_PreciseRegister", 8, "Precise Register", LevelSet.Occlusion, V(6, 6));
            def.sensors = new List<SensorEntry>
            {
                Target("A", V(2, 3), V(3, 3)), Forbidden("X", V(4, 3)),
            };
            def.plates = new List<PlateEntry>
            {
                Plate("A", a, V(0, 0), Rot90.None, offsets: Offs((0,0),(2,0),(1,0),(0,1)),
                    rots: Rots(Rot90.None, Rot90.CW90)),
                Plate("B", b, V(0, 0), Rot90.None, offsets: Offs((0,0),(2,0),(1,0),(0,1)), flip: true),
            };
            return Save(def, "Level_08_PreciseRegister");
        }

        // ============================================================ SET C

        static StencilLevelDefinition BuildLevel9()
        {
            var s = Shape("Shape_L9_Dot", 7, 6, Cells((1, 2)));
            var def = NewLevel("Level_09_Parallax", 9, "Parallax", LevelSet.Projection, V(7, 6));
            def.parallaxPerDepth = 1f;
            def.sensors = new List<SensorEntry> { Target("T", V(4, 2)), Forbidden("X", V(5, 2)) };
            def.plates = new List<PlateEntry>
            {
                PlateDepth("A", s, V(0, 0), 0, offsets: Offs((0,0),(3,0),(2,0),(1,0))),
                PlateDepth("B", s, V(0, 0), 2, offsets: Offs((0,0),(1,0),(2,0),(3,0))),
            };
            return Save(def, "Level_09_Parallax");
        }

        static StencilLevelDefinition BuildLevel10()
        {
            var s = Shape("Shape_L10_TwoFilter", 7, 7,
                front: Cells((1, 1), (3, 1)),
                front_filters: new List<int> { 1, 2 });
            var def = NewLevel("Level_10_FilterPass", 10, "Filter Pass", LevelSet.Projection, V(7, 7));
            def.sensors = new List<SensorEntry>
            {
                TargetF("Square", 1, V(3, 3)),
                TargetF("Triangle", 2, V(5, 3)),
            };
            def.plates = new List<PlateEntry>
            {
                Plate("A", s, V(0, 0), Rot90.None, offsets: Offs((0,0),(2,2),(1,1),(2,0))),
                Plate("B", s, V(1, 1), Rot90.None, offsets: Offs((0,0),(2,2),(1,1),(0,2),(4,2))),
            };
            return Save(def, "Level_10_FilterPass");
        }

        static StencilLevelDefinition BuildLevel11()
        {
            var s = Shape("Shape_L11_TwoDot", 6, 6, Cells((1, 3), (4, 3)));
            var def = NewLevel("Level_11_DualProjection", 11, "Dual Projection", LevelSet.Projection, V(6, 6));
            def.projectors = new List<ProjectorEntry>
            {
                ApertureProj("P1", V(6, 6), c => c.x <= 2),
                ApertureProj("P2", V(6, 6), c => c.x >= 3),
            };
            def.sensors = new List<SensorEntry>
            {
                Target("L", V(1, 3), "P1"), Forbidden("XL", V(1, 1), "P1"),
                Target("R", V(4, 3), "P2"), Forbidden("XR", V(4, 5), "P2"),
            };
            def.plates = new List<PlateEntry>
            {
                Plate("A", s, V(0, 0), Rot90.None, offsets: Offs((0,0),(1,0),(0,1),(1,1))),
                Plate("B", s, V(1, 0), Rot90.None, offsets: Offs((0,0),(1,0),(0,1),(1,1))),
            };
            return Save(def, "Level_11_DualProjection");
        }

        static StencilLevelDefinition BuildLevel12()
        {
            // Final figure (board space) = L: Bar {(2,3),(3,3)} on P1, Post {(4,2),(4,3)} on P2.
            // All three plates are full 7x7 sheets that always cover the whole board, so the
            // projected light is exactly the intersection of their hole figures.
            //  A: figure appears only after a CW90 turn (holes authored pre-rotated).
            //  B: figure is on the back face - needs a flip.
            //  C: a wider patch that must be slid home so it stops trimming the figure.
            // Culmination: rotate + flip + slide + dual projection, one coherent target.
            var figure = Cells((2, 3), (3, 3), (4, 3), (4, 2));
            var aPre = Cells((3, 2), (3, 3), (3, 4), (4, 4));  // CW90 of `figure` in 7x7
            var a = Shape("Shape_L12_RotSheet", 7, 7, aPre);
            var b = Shape("Shape_L12_FlipSheet", 7, 7, front: aPre, back: figure);
            var c = Shape("Shape_L12_SlideSheet", 7, 7,
                Cells((2, 3), (3, 3), (4, 3), (4, 2), (5, 3), (2, 2)));

            var def = NewLevel("Level_12_MasterPlate", 12, "Master Plate", LevelSet.Projection, V(7, 7));
            def.projectors = new List<ProjectorEntry> { ProjEntry("P1", V(7, 7)), ProjEntry("P2", V(7, 7)) };
            def.sensors = new List<SensorEntry>
            {
                Target("Bar", V(2, 3), "P1"), Target("Bar2", V(3, 3), "P1"), Forbidden("XP1", V(3, 2), "P1"),
                Target("Post", V(4, 2), "P2"), Target("Post2", V(4, 3), "P2"), Forbidden("XP2", V(4, 4), "P2"),
            };
            def.plates = new List<PlateEntry>
            {
                Plate("A", a, V(0, 0), Rot90.None,
                    rots: Rots(Rot90.None, Rot90.CW90, Rot90.CW180, Rot90.CW270)),
                Plate("B", b, V(0, 0), Rot90.None, flip: true),
                Plate("C", c, V(2, 0), Rot90.None, offsets: Offs((0,0),(2,0),(1,0),(0,1))),
            };
            return Save(def, "Level_12_MasterPlate");
        }

        // ============================================================ helpers

        static List<Vector2Int> Cells(params (int x, int y)[] c) => c.Select(t => V(t.x, t.y)).ToList();

        static SensorEntry Target(string label, params Vector2Int[] cells)
            => new SensorEntry { label = label, kind = ConditionKind.RequiredTarget, projectorId = "P", cells = cells.ToList() };

        static SensorEntry Target(string label, Vector2Int cell, string proj)
            => new SensorEntry { label = label, kind = ConditionKind.RequiredTarget, projectorId = proj, cells = { cell } };

        static SensorEntry TargetF(string label, int filter, Vector2Int cell)
            => new SensorEntry { label = label, kind = ConditionKind.RequiredTarget, projectorId = "P", cells = { cell }, requiredFilter = filter };

        static SensorEntry Forbidden(string label, Vector2Int cell, string proj = "P")
            => new SensorEntry { label = label, kind = ConditionKind.ForbiddenSensor, projectorId = proj, cells = { cell } };

        static ProjectorEntry ProjEntry(string id, Vector2Int _) => new ProjectorEntry { id = id, fullBoardAperture = true };

        static ProjectorEntry ApertureProj(string id, Vector2Int board, System.Func<Vector2Int, bool> inAperture)
        {
            var cells = new List<Vector2Int>();
            for (int y = 0; y < board.y; y++) for (int x = 0; x < board.x; x++)
                if (inAperture(V(x, y))) cells.Add(V(x, y));
            return new ProjectorEntry { id = id, fullBoardAperture = false, apertureCells = cells };
        }

        static PlateEntry Plate(string id, StencilShapeAsset shape, Vector2Int initOffset, Rot90 initRot,
            List<Vector2Int> offsets = null, List<Rot90> rots = null, bool flip = false)
            => new PlateEntry
            {
                id = id, shape = shape,
                initialOffset = initOffset, initialRotation = initRot,
                allowedOffsets = offsets ?? new List<Vector2Int> { initOffset },
                allowedRotations = rots ?? new List<Rot90>(),
                flipAllowed = flip,
            };

        static PlateEntry PlateDepth(string id, StencilShapeAsset shape, Vector2Int initOffset, int depth,
            List<Vector2Int> offsets = null)
        {
            var p = Plate(id, shape, initOffset, Rot90.None, offsets);
            p.depthPlane = depth;
            return p;
        }

        static StencilLevelDefinition NewLevel(string _, int id, string title, LevelSet set, Vector2Int board)
        {
            var path = $"{LevelsDir}/{_}.asset";
            var def = AssetDatabase.LoadAssetAtPath<StencilLevelDefinition>(path)
                      ?? ScriptableObject.CreateInstance<StencilLevelDefinition>();
            def.levelId = id;
            def.displayTitle = title;
            def.set = set;
            def.boardSize = board;
            def.projectors = new List<ProjectorEntry> { new ProjectorEntry { id = "P", fullBoardAperture = true } };
            def.sensors = new List<SensorEntry>();
            def.plates = new List<PlateEntry>();
            def.tutorialSteps = new List<TutorialStep>();
            def.isTutorial = false;
            def.parallaxPerDepth = 0f;
            def.movePar = 3;
            return def;
        }

        static StencilLevelDefinition Save(StencilLevelDefinition def, string name)
        {
            var path = $"{LevelsDir}/{name}.asset";
            if (AssetDatabase.LoadAssetAtPath<StencilLevelDefinition>(path) == null)
                AssetDatabase.CreateAsset(def, path);
            else
                EditorUtility.SetDirty(def);
            return def;
        }

        static StencilShapeAsset Shape(string name, int w, int h,
            List<Vector2Int> front, List<Vector2Int> back = null,
            List<int> front_filters = null, List<int> back_filters = null)
        {
            var path = $"{ShapesDir}/{name}.asset";
            var s = AssetDatabase.LoadAssetAtPath<StencilShapeAsset>(path);
            bool created = s == null;
            if (created) s = ScriptableObject.CreateInstance<StencilShapeAsset>();
            s.footprintW = w; s.footprintH = h;
            s.frontOpen = front ?? new List<Vector2Int>();
            s.hasDistinctBack = back != null;
            s.backOpen = back ?? new List<Vector2Int>();
            s.frontFilters = front_filters ?? new List<int>();
            s.backFilters = back_filters ?? new List<int>();
            if (created) AssetDatabase.CreateAsset(s, path);
            else EditorUtility.SetDirty(s);
            return s;
        }

        static void EnsureDir(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parent = Path.GetDirectoryName(path).Replace('\\', '/');
            var leaf = Path.GetFileName(path);
            if (!AssetDatabase.IsValidFolder(parent)) EnsureDir(parent);
            AssetDatabase.CreateFolder(parent, leaf);
        }
    }
}
