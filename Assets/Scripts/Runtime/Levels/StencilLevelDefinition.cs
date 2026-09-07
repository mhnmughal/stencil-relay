using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using StencilRelay.Core;

namespace StencilRelay.Levels
{
    public enum LevelSet { Calibration, Alignment, Occlusion, Projection }

    [Serializable]
    public sealed class ProjectorEntry
    {
        public string id = "P";
        public bool fullBoardAperture = true;
        public List<Vector2Int> apertureCells = new List<Vector2Int>();

        public LightProjector ToRuntime(int boardW, int boardH)
        {
            if (fullBoardAperture) return LightProjector.Full(id, boardW, boardH);
            return new LightProjector(id, CellGrid.FromCells(boardW, boardH, apertureCells));
        }
    }

    [Serializable]
    public sealed class SensorEntry
    {
        public string label = "T";
        public ConditionKind kind = ConditionKind.RequiredTarget;
        public string projectorId = "P";
        public List<Vector2Int> cells = new List<Vector2Int>();
        [Tooltip("Required filter category for a target (0 = any). Level 10.")]
        public int requiredFilter;

        public SensorSpec ToRuntime() => new SensorSpec
        {
            Label = label,
            Kind = kind,
            ProjectorId = projectorId,
            Cells = new List<Vector2Int>(cells),
            RequiredFilter = requiredFilter
        };
    }

    [Serializable]
    public sealed class PlateEntry
    {
        public string id = "A";
        public StencilShapeAsset shape;

        [Header("Initial placement")]
        public Vector2Int initialOffset;
        public Rot90 initialRotation = Rot90.None;
        public bool initialFlipped;

        [Header("Permissions")]
        public List<Vector2Int> allowedOffsets = new List<Vector2Int>();
        public List<Rot90> allowedRotations = new List<Rot90>();
        public bool flipAllowed;

        [Header("Authored optimal")]
        public Vector2Int optimalOffset;
        public Rot90 optimalRotation = Rot90.None;
        public bool optimalFlipped;

        [Header("Visual")]
        [Tooltip("Depth plane index for Level 9 parallax. 0 = table plane.")]
        public int depthPlane;

        public PlateTransform Initial => new PlateTransform(initialOffset, initialRotation, initialFlipped);
        public PlateTransform Optimal => new PlateTransform(optimalOffset, optimalRotation, optimalFlipped);

        public PlatePermissions ToPermissions() => new PlatePermissions
        {
            AllowedOffsets = new List<Vector2Int>(allowedOffsets),
            AllowedRotations = new List<Rot90>(allowedRotations),
            FlipAllowed = flipAllowed
        };
    }

    [Serializable]
    public sealed class TutorialStep
    {
        [Tooltip("Hint anchor id the ProjectionTable scene knows: PlateA/PlateB/..., RotateDial, FlipToggle, ProjectSwitch, ResetButton, Board.")]
        public string highlightAnchor;
        public string instruction;
        [TextArea] public string note;
    }

    /// <summary>Plain runtime bundle produced from a <see cref="StencilLevelDefinition"/>.</summary>
    public sealed class LevelRuntime
    {
        public int LevelId;
        public string Title;
        public LevelSet Set;
        public int BoardW, BoardH;
        public List<LightProjector> Projectors = new List<LightProjector>();
        public List<SensorSpec> Sensors = new List<SensorSpec>();
        public List<StencilPlate> Plates = new List<StencilPlate>();
        public List<PlatePermissions> Permissions = new List<PlatePermissions>();
        public List<PlateTransform> Optimal = new List<PlateTransform>();
        public List<int> DepthPlanes = new List<int>();
        public float ParallaxPerDepth;
        public int MovePar;
        public bool IsTutorial;
        public List<string> TutorialInstructions = new List<string>();

        public void ApplyTransforms(IReadOnlyList<PlateTransform> transforms)
        {
            for (int i = 0; i < Plates.Count && i < transforms.Count; i++)
            {
                Plates[i].Transform = transforms[i];
                Plates[i].ExtraShift = ParallaxShiftFor(i);
            }
        }

        public Vector2Int ParallaxShiftFor(int plateIndex)
        {
            if (ParallaxPerDepth == 0f || plateIndex >= DepthPlanes.Count) return Vector2Int.zero;
            // Deterministic authored horizontal shift per depth plane (Level 9). Integer cells.
            int shift = Mathf.RoundToInt(DepthPlanes[plateIndex] * ParallaxPerDepth);
            return new Vector2Int(shift, 0);
        }

        public ProjectionResult Evaluate()
            => ProjectionEvaluator.Evaluate(BoardW, BoardH, Projectors, Sensors, Plates);
    }

    [CreateAssetMenu(menuName = "Stencil Relay/Level Definition", fileName = "Level")]
    public sealed class StencilLevelDefinition : ScriptableObject
    {
        [Range(0, 12)] public int levelId;
        public string displayTitle = "Pattern";
        public LevelSet set = LevelSet.Alignment;
        public Vector2Int boardSize = new Vector2Int(6, 6);

        public List<ProjectorEntry> projectors = new List<ProjectorEntry> { new ProjectorEntry() };
        public List<PlateEntry> plates = new List<PlateEntry>();
        public List<SensorEntry> sensors = new List<SensorEntry>();

        [Min(0)] public int movePar = 3;

        [Header("Tutorial (Level 0 only)")]
        public bool isTutorial;
        public List<TutorialStep> tutorialSteps = new List<TutorialStep>();

        [Header("Visual modifiers")]
        [Tooltip("Cells of horizontal shift per depth-plane step (Level 9 parallax). 0 = off.")]
        public float parallaxPerDepth;

        LevelRuntime BuildBase()
        {
            int bw = Mathf.Max(1, boardSize.x);
            int bh = Mathf.Max(1, boardSize.y);
            var rt = new LevelRuntime
            {
                LevelId = levelId,
                Title = displayTitle,
                Set = set,
                BoardW = bw,
                BoardH = bh,
                MovePar = movePar,
                IsTutorial = isTutorial,
                TutorialInstructions = tutorialSteps
                    .Where(s => s != null && !string.IsNullOrWhiteSpace(s.instruction))
                    .Select(s => s.instruction).ToList(),
                ParallaxPerDepth = parallaxPerDepth,
                Projectors = projectors.Select(p => p.ToRuntime(bw, bh)).ToList(),
                Sensors = sensors.Select(s => s.ToRuntime()).ToList(),
            };
            foreach (var pe in plates)
            {
                var shape = pe.shape != null ? pe.shape.ToRuntime() : new StencilShape(1, 1, new Vector2Int[0]);
                rt.Plates.Add(new StencilPlate(pe.id, shape, pe.Initial));
                rt.Permissions.Add(pe.ToPermissions());
                rt.Optimal.Add(pe.Optimal);
                rt.DepthPlanes.Add(pe.depthPlane);
            }
            for (int i = 0; i < rt.Plates.Count; i++)
                rt.Plates[i].ExtraShift = rt.ParallaxShiftFor(i);
            return rt;
        }

        /// <summary>Runtime bundle with plates at their initial transforms.</summary>
        public LevelRuntime BuildRuntime() => BuildBase();

        /// <summary>Runtime bundle with plates at the authored optimal transforms.</summary>
        public LevelRuntime BuildAtOptimal()
        {
            var rt = BuildBase();
            rt.ApplyTransforms(rt.Optimal);
            return rt;
        }

        /// <summary>Minimum player moves from initial to optimal (slide=1, each quarter turn=1, flip=1).</summary>
        public int MinMovesToOptimal()
        {
            int total = 0;
            foreach (var pe in plates)
            {
                if (pe.initialOffset != pe.optimalOffset) total += 1;
                total += QuarterTurns(pe.initialRotation, pe.optimalRotation);
                if (pe.initialFlipped != pe.optimalFlipped) total += 1;
            }
            return total;
        }

        static int QuarterTurns(Rot90 a, Rot90 b)
        {
            int d = (((int)b - (int)a) % 4 + 4) % 4;
            return Mathf.Min(d, 4 - d);
        }
    }
}
