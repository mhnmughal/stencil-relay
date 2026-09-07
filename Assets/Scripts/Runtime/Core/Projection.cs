using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StencilRelay.Core
{
    /// <summary>A light source. Emits into <see cref="Aperture"/> board cells; light then
    /// passes the plate stack. Levels have one projector; Level 11 has two.</summary>
    public sealed class LightProjector
    {
        public readonly string Id;
        public readonly CellGrid Aperture;

        public LightProjector(string id, CellGrid aperture)
        {
            Id = id;
            Aperture = aperture;
        }

        /// <summary>LightProjector emitting across the whole board.</summary>
        public static LightProjector Full(string id, int boardW, int boardH)
        {
            var ap = new CellGrid(boardW, boardH);
            ap.SetAll(true);
            return new LightProjector(id, ap);
        }
    }

    public enum ConditionKind { RequiredTarget, ForbiddenSensor }

    /// <summary>A required target (all cells must be lit) or forbidden sensor (no cell may be lit).</summary>
    public sealed class SensorSpec
    {
        public string Label;
        public ConditionKind Kind;
        public string ProjectorId;
        public List<Vector2Int> Cells = new List<Vector2Int>();

        /// <summary>Required filter category for a target (0 = any). Level 10.</summary>
        public int RequiredFilter;
    }

    public sealed class ConditionResult
    {
        public string Label;
        public ConditionKind Kind;
        public string ProjectorId;
        public bool Passed;
        public string Reason;
    }

    public sealed class ProjectionResult
    {
        public bool Success;
        public List<ConditionResult> Conditions = new List<ConditionResult>();
        /// <summary>Composited light per projector id, for rendering. Never drives logic re-eval.</summary>
        public Dictionary<string, CellGrid> LightByProjector = new Dictionary<string, CellGrid>();

        public IEnumerable<ConditionResult> Failures => Conditions.Where(c => !c.Passed);

        /// <summary>Human-readable failure reasons, deduplicated (for the MISALIGNED screen).</summary>
        public IEnumerable<string> FailureReasons => Failures.Select(f => f.Reason).Distinct();
    }

    /// <summary>Composites projector light through the plate stack. Order-independent (all AND/AND-NOT).</summary>
    public static class StencilLayerStack
    {
        public static CellGrid Composite(LightProjector projector, IEnumerable<StencilPlate> plates,
            int boardW, int boardH)
        {
            var light = projector.Aperture.Clone();
            foreach (var plate in plates)
                light.AndNot(plate.ResolveOpaque(boardW, boardH));
            return light;
        }
    }

    /// <summary>
    /// Deterministic puzzle evaluation. Pure function of (board size, projectors, sensors,
    /// plate list with their current transforms). Same input -> same result, always.
    /// </summary>
    public static class ProjectionEvaluator
    {
        public static ProjectionResult Evaluate(
            int boardW, int boardH,
            IReadOnlyList<LightProjector> projectors,
            IReadOnlyList<SensorSpec> sensors,
            IReadOnlyList<StencilPlate> plates)
        {
            var result = new ProjectionResult { Success = true };

            foreach (var projector in projectors)
            {
                var light = StencilLayerStack.Composite(projector, plates, boardW, boardH);
                result.LightByProjector[projector.Id] = light;

                foreach (var sensor in sensors.Where(s => s.ProjectorId == projector.Id))
                {
                    var cr = new ConditionResult
                    {
                        Label = sensor.Label,
                        Kind = sensor.Kind,
                        ProjectorId = projector.Id
                    };

                    int lit = sensor.Cells.Count(c => light.Get(c.x, c.y));

                    if (sensor.Kind == ConditionKind.RequiredTarget)
                    {
                        bool allLit = lit == sensor.Cells.Count && sensor.Cells.Count > 0;
                        bool filterOk = sensor.RequiredFilter == 0 || sensor.Cells.All(c =>
                            plates.Any(p => p.OpenFilterAtBoardCell(c, boardW, boardH) == sensor.RequiredFilter));
                        cr.Passed = allLit && filterOk;
                        string suffix = projectors.Count > 1 ? $", {projector.Id}" : "";
                        cr.Reason = cr.Passed ? null
                            : !allLit ? $"Target Unlit ({sensor.Label}{suffix})"
                            : $"Filter Mismatch ({sensor.Label}{suffix})";
                    }
                    else // ForbiddenSensor
                    {
                        cr.Passed = lit == 0;
                        cr.Reason = cr.Passed ? null
                            : projectors.Count > 1
                                ? $"Forbidden Sensor Lit ({sensor.Label}, {projector.Id})"
                                : $"Forbidden Sensor Lit ({sensor.Label})";
                    }

                    if (!cr.Passed) result.Success = false;
                    result.Conditions.Add(cr);
                }
            }

            return result;
        }
    }
}
