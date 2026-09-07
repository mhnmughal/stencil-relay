using System.Collections.Generic;
using System.Linq;
using StencilRelay.Core;

namespace StencilRelay.Levels
{
    public enum SolveStatus { Solved, Unsolvable, Inconclusive }

    public sealed class SolveReport
    {
        public SolveStatus Status;
        public List<PlateTransform> Solution;   // when Solved
        public int SolutionsFound;              // capped count
        public long SearchSpace;                // product of per-plate placement counts
        public string Message;
    }

    /// <summary>
    /// Brute-force solvability check for the Editor validator and tests. Enumerates every
    /// permitted placement combination and evaluates it. Bounded by <see cref="MaxSearchSpace"/>
    /// so it never hangs the Editor; larger spaces return Inconclusive (the authored optimal
    /// is still checked directly by the validator).
    /// ponytail: naive cartesian product; fine because levels list only a handful of allowed
    /// offsets per plate. Add pruning only if a real level exceeds the cap.
    /// </summary>
    public static class LevelSolver
    {
        public const long MaxSearchSpace = 3_000_000;
        public const int MaxSolutionsCounted = 5;

        public static SolveReport Solve(LevelRuntime rt)
        {
            var perPlate = new List<List<PlateTransform>>();
            long space = 1;
            for (int i = 0; i < rt.Plates.Count; i++)
            {
                var placements = rt.Permissions[i]
                    .EnumeratePlacements(rt.Plates[i].Transform)
                    .Distinct()
                    .ToList();
                if (placements.Count == 0)
                    placements.Add(rt.Plates[i].Transform);
                perPlate.Add(placements);
                space *= placements.Count;
            }

            var report = new SolveReport { SearchSpace = space };

            if (rt.Plates.Count == 0)
            {
                report.Status = SolveStatus.Unsolvable;
                report.Message = "Level has no plates.";
                return report;
            }

            if (space > MaxSearchSpace)
            {
                report.Status = SolveStatus.Inconclusive;
                report.Message = $"Search space {space:N0} exceeds cap {MaxSearchSpace:N0}; " +
                                 "exhaustive check skipped (authored optimal still verified separately).";
                return report;
            }

            var current = new PlateTransform[rt.Plates.Count];
            var firstSolution = (List<PlateTransform>)null;
            int solutions = 0;

            void Recurse(int plateIndex)
            {
                if (solutions >= MaxSolutionsCounted) return;
                if (plateIndex == perPlate.Count)
                {
                    for (int i = 0; i < current.Length; i++)
                    {
                        rt.Plates[i].Transform = current[i];
                        rt.Plates[i].ExtraShift = rt.ParallaxShiftFor(i);
                    }
                    if (rt.Evaluate().Success)
                    {
                        solutions++;
                        if (firstSolution == null) firstSolution = current.ToList();
                    }
                    return;
                }
                foreach (var t in perPlate[plateIndex])
                {
                    current[plateIndex] = t;
                    Recurse(plateIndex + 1);
                    if (solutions >= MaxSolutionsCounted) return;
                }
            }

            Recurse(0);

            report.SolutionsFound = solutions;
            if (solutions > 0)
            {
                report.Status = SolveStatus.Solved;
                report.Solution = firstSolution;
                report.Message = solutions >= MaxSolutionsCounted
                    ? $"Solvable ({MaxSolutionsCounted}+ solutions)."
                    : $"Solvable ({solutions} solution{(solutions == 1 ? "" : "s")}).";
            }
            else
            {
                report.Status = SolveStatus.Unsolvable;
                report.Message = "No permitted placement combination satisfies all conditions.";
            }

            return report;
        }
    }
}
