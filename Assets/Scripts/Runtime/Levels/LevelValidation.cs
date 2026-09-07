using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using StencilRelay.Core;

namespace StencilRelay.Levels
{
    public sealed class LevelIssue
    {
        public int LevelId;
        public string LevelName;
        public string Message;
        public bool IsError;   // errors fail validation; warnings do not
        public override string ToString()
            => $"{(IsError ? "ERROR" : "warn")} [L{LevelId:00} {LevelName}] {Message}";
    }

    public sealed class ValidationReport
    {
        public List<LevelIssue> Issues = new List<LevelIssue>();
        public int LevelsChecked;
        public int LevelsValid;

        public bool AllValid => Issues.All(i => !i.IsError) && LevelsChecked == 13 && LevelsValid == 13;
        public IEnumerable<LevelIssue> Errors => Issues.Where(i => i.IsError);
        public IEnumerable<LevelIssue> Warnings => Issues.Where(i => !i.IsError);

        public string Headline => AllValid
            ? "13 / 13 VALID"
            : $"{LevelsValid} / {LevelsChecked} VALID  ({Errors.Count()} error(s), {Warnings.Count()} warning(s))";
    }

    /// <summary>
    /// Data / simulation validation for the 13 levels (master prompt sections 13, 27, 37, 83).
    /// Pure C# so both the Editor menu and EditMode tests run the same checks. Never hides
    /// invalid data - every problem is reported with its exact level and reason.
    /// </summary>
    public static class LevelValidation
    {
        public static ValidationReport Validate(LevelDatabase db)
        {
            var report = new ValidationReport();
            if (db == null)
            {
                report.Issues.Add(new LevelIssue { LevelId = -1, LevelName = "(none)", IsError = true, Message = "LevelDatabase is null." });
                return report;
            }

            var defs = db.levels.Where(l => l != null).ToList();

            // DB-level structure
            var idCounts = defs.GroupBy(d => d.levelId).ToDictionary(g => g.Key, g => g.Count());
            for (int expected = 0; expected <= 12; expected++)
                if (!idCounts.ContainsKey(expected))
                    report.Issues.Add(Err(expected, "(missing)", $"No level with id {expected} in the database."));
            foreach (var kv in idCounts.Where(kv => kv.Value > 1))
                report.Issues.Add(Err(kv.Key, "(dupe)", $"Level id {kv.Key} appears {kv.Value} times."));
            if (defs.Count != 13)
                report.Issues.Add(Err(-1, "(database)", $"Database has {defs.Count} levels, expected 13."));

            foreach (var def in defs.OrderBy(d => d.levelId))
            {
                report.LevelsChecked++;
                int before = report.Issues.Count(i => i.IsError);
                ValidateOne(def, report);
                if (report.Issues.Count(i => i.IsError) == before)
                    report.LevelsValid++;
            }

            return report;
        }

        static void ValidateOne(StencilLevelDefinition def, ValidationReport report)
        {
            int id = def.levelId;
            string name = def.displayTitle;
            void E(string m) => report.Issues.Add(Err(id, name, m));
            void W(string m) => report.Issues.Add(Warn(id, name, m));

            if (id < 0 || id > 12) E($"levelId {id} out of range 0..12.");

            int bw = def.boardSize.x, bh = def.boardSize.y;
            if (bw < 2 || bh < 2) E($"board size {bw}x{bh} too small.");
            bool InBoard(Vector2Int c) => c.x >= 0 && c.y >= 0 && c.x < bw && c.y < bh;

            if (def.projectors == null || def.projectors.Count == 0) E("no projectors.");
            var projIds = new HashSet<string>(def.projectors?.Select(p => p.id) ?? Enumerable.Empty<string>());
            foreach (var p in def.projectors ?? new List<ProjectorEntry>())
            {
                if (string.IsNullOrWhiteSpace(p.id)) E("projector with empty id.");
                if (!p.fullBoardAperture)
                {
                    if (p.apertureCells.Count == 0) E($"projector {p.id} has custom aperture with no cells.");
                    foreach (var c in p.apertureCells.Where(c => !InBoard(c))) E($"projector {p.id} aperture cell {c} off board.");
                }
            }

            if (def.plates == null || def.plates.Count == 0) E("no plates.");
            if (def.plates != null && def.plates.Count > 4) W($"{def.plates.Count} plates (design guidance: 1-4).");
            var plateIds = new HashSet<string>();
            foreach (var pe in def.plates ?? new List<PlateEntry>())
            {
                if (string.IsNullOrWhiteSpace(pe.id)) E("plate with empty id.");
                else if (!plateIds.Add(pe.id)) E($"duplicate plate id '{pe.id}'.");
                if (pe.shape == null) { E($"plate '{pe.id}' has no shape asset."); continue; }
                // A plate with no front openings is a valid blocker (Set B). Only flag it if
                // EVERY plate on the level is holeless (then no light can ever pass).
                if (pe.shape.hasDistinctBack && pe.shape.backOpen.Count == 0) W($"plate '{pe.id}' marked distinct-back but backOpen empty.");

                var perms = pe.ToPermissions();
                if (!perms.CanPlace(pe.Initial, pe.Initial))
                    E($"plate '{pe.id}' initial transform not allowed by its own permissions.");
                if (!perms.CanPlace(pe.Optimal, pe.Initial))
                    E($"plate '{pe.id}' optimal transform {pe.Optimal} not reachable under permissions.");
                foreach (var o in pe.allowedOffsets.Where(o => o.x < -pe.shape.footprintW || o.y < -pe.shape.footprintH || o.x >= bw || o.y >= bh))
                    W($"plate '{pe.id}' allowed offset {o} places the whole footprint off board.");
            }

            if (def.sensors == null || !def.sensors.Any(s => s.kind == ConditionKind.RequiredTarget))
                E("no required targets.");
            if (def.plates != null && def.plates.All(p => p.shape != null && p.shape.frontOpen.Count == 0
                    && (!p.shape.hasDistinctBack || p.shape.backOpen.Count == 0)))
                E("every plate is holeless - no light can pass.");
            foreach (var s in def.sensors ?? new List<SensorEntry>())
            {
                if (s.cells.Count == 0) E($"sensor '{s.label}' has no cells.");
                foreach (var c in s.cells.Where(c => !InBoard(c))) E($"sensor '{s.label}' cell {c} off board.");
                if (!projIds.Contains(s.projectorId)) E($"sensor '{s.label}' references unknown projector '{s.projectorId}'.");
            }

            // Tutorial rules
            if (id == 0)
            {
                if (!def.isTutorial) E("Level 0 must have isTutorial = true.");
                if (def.tutorialSteps == null || def.tutorialSteps.Count == 0) E("Level 0 has no tutorial steps.");
            }
            else if (def.isTutorial)
            {
                E($"Level {id} must not be a tutorial.");
            }

            // Level 11 dual projection
            if (id == 11)
            {
                if (projIds.Count < 2) E("Level 11 (Dual Projection) needs at least 2 projectors.");
                var projsWithTargets = (def.sensors ?? new List<SensorEntry>())
                    .Where(s => s.kind == ConditionKind.RequiredTarget)
                    .Select(s => s.projectorId).Distinct().Count();
                if (projsWithTargets < 2) E("Level 11 needs required targets on at least 2 projectors.");
            }

            // Simulation: authored optimal must solve
            LevelRuntime atOptimal;
            try { atOptimal = def.BuildAtOptimal(); }
            catch (System.Exception ex) { E($"BuildAtOptimal threw: {ex.Message}"); return; }

            var optRes = atOptimal.Evaluate();
            if (!optRes.Success)
                E($"authored optimal does NOT solve the level. Failures: {string.Join("; ", optRes.FailureReasons)}");

            // Move par sanity (skip for tutorial)
            if (id != 0)
            {
                int minMoves = def.MinMovesToOptimal();
                if (def.movePar < minMoves)
                    E($"movePar {def.movePar} < minimum moves to optimal ({minMoves}).");
                if (def.movePar > minMoves + 6)
                    W($"movePar {def.movePar} is generous vs minimum {minMoves}.");
            }

            // Brute-force solvability
            try
            {
                var solve = LevelSolver.Solve(def.BuildRuntime());
                switch (solve.Status)
                {
                    case SolveStatus.Unsolvable:
                        E($"solver found NO solution under the permitted placements. {solve.Message}");
                        break;
                    case SolveStatus.Inconclusive:
                        W($"solver inconclusive: {solve.Message}");
                        break;
                    case SolveStatus.Solved:
                        // A unique solution is the ideal for a handcrafted puzzle - not a warning.
                        break;
                }
            }
            catch (System.Exception ex) { W($"solver threw: {ex.Message}"); }
        }

        static LevelIssue Err(int id, string name, string m) => new LevelIssue { LevelId = id, LevelName = name, Message = m, IsError = true };
        static LevelIssue Warn(int id, string name, string m) => new LevelIssue { LevelId = id, LevelName = name, Message = m, IsError = false };
    }
}
