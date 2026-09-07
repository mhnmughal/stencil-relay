using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using StencilRelay.Core;

namespace StencilRelay.Levels
{
    public enum Rating { None, Aligned, Precise, PerfectRegister }

    /// <summary>
    /// Player-side manipulation state for one level attempt: which plate is selected,
    /// the committed transforms, and the move count. Pure logic (no scene deps) so it is
    /// EditMode-tested and the same instance drives both the views and the evaluator.
    ///
    /// Move accounting: each committed slide (to a different offset), each rotate step,
    /// and each flip is one move, attributed to that plate. Resetting a plate refunds
    /// its moves. This keeps PRECISE reachable after a fumble+reset.
    /// </summary>
    public sealed class LevelManipulation
    {
        public LevelRuntime Runtime { get; }
        public int SelectedIndex { get; private set; } = -1;
        public bool Locked { get; private set; }

        readonly int[] _movesPerPlate;
        readonly PlateTransform[] _initial;   // runtime is built at initial transforms
        public int MoveCount => _movesPerPlate.Sum();

        public event Action Changed;

        public LevelManipulation(LevelRuntime runtime)
        {
            Runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
            _movesPerPlate = new int[runtime.Plates.Count];
            _initial = runtime.Plates.Select(p => p.Transform).ToArray();
        }

        public StencilPlate Plate(int i) => Runtime.Plates[i];
        public PlatePermissions Permissions(int i) => Runtime.Permissions[i];
        public PlateTransform Initial(int i) => _initial[i];
        PlateTransform InitialCache(int i) => _initial[i];

        public void Select(int plateIndex)
        {
            if (Locked) return;
            SelectedIndex = (plateIndex >= 0 && plateIndex < Runtime.Plates.Count) ? plateIndex : -1;
            Changed?.Invoke();
        }

        public bool TrySlide(int i, Vector2Int newOffset)
        {
            if (Locked || !ValidIndex(i)) return false;
            var perms = Runtime.Permissions[i];
            var candidate = Runtime.Plates[i].Transform;
            candidate.Offset = newOffset;
            if (!perms.CanPlace(candidate, InitialCache(i))) return false;
            if (Runtime.Plates[i].Transform.Offset == newOffset) return false; // no-op

            Runtime.Plates[i].Transform = candidate;
            ApplyParallax(i);
            _movesPerPlate[i]++;
            Changed?.Invoke();
            return true;
        }

        /// <summary>Snap an arbitrary grid coordinate to the nearest allowed offset, then slide.</summary>
        public bool TrySlideToNearest(int i, Vector2 gridCoord)
        {
            if (Locked || !ValidIndex(i)) return false;
            var allowed = AllowedOffsets(i).ToList();
            if (allowed.Count == 0) return false;
            var best = allowed.OrderBy(o => ((Vector2)o - gridCoord).sqrMagnitude).First();
            return TrySlide(i, best);
        }

        public IEnumerable<Vector2Int> AllowedOffsets(int i)
        {
            var perms = Runtime.Permissions[i];
            if (perms.AllowedOffsets != null && perms.AllowedOffsets.Count > 0)
                return perms.AllowedOffsets;
            return new[] { InitialCache(i).Offset };
        }

        public bool RotateStep(int i, int dir = 1)
        {
            if (Locked || !ValidIndex(i)) return false;
            var perms = Runtime.Permissions[i];
            var cycle = (perms.AllowedRotations != null && perms.AllowedRotations.Count > 0)
                ? perms.AllowedRotations
                : new List<Rot90> { InitialCache(i).Rotation };
            if (cycle.Count < 2) return false;

            int cur = cycle.IndexOf(Runtime.Plates[i].Transform.Rotation);
            if (cur < 0) cur = 0;
            int next = ((cur + Math.Sign(dir)) % cycle.Count + cycle.Count) % cycle.Count;

            var t = Runtime.Plates[i].Transform;
            t.Rotation = cycle[next];
            Runtime.Plates[i].Transform = t;
            _movesPerPlate[i]++;
            Changed?.Invoke();
            return true;
        }

        public bool Flip(int i)
        {
            if (Locked || !ValidIndex(i)) return false;
            if (!Runtime.Permissions[i].FlipAllowed) return false;
            var t = Runtime.Plates[i].Transform;
            t.Flipped = !t.Flipped;
            Runtime.Plates[i].Transform = t;
            _movesPerPlate[i]++;
            Changed?.Invoke();
            return true;
        }

        public void ResetPlate(int i)
        {
            if (Locked || !ValidIndex(i)) return;
            Runtime.Plates[i].Transform = InitialCache(i);
            ApplyParallax(i);
            _movesPerPlate[i] = 0;
            Changed?.Invoke();
        }

        public void ResetAll()
        {
            if (Locked) return;
            for (int i = 0; i < Runtime.Plates.Count; i++)
            {
                Runtime.Plates[i].Transform = InitialCache(i);
                ApplyParallax(i);
                _movesPerPlate[i] = 0;
            }
            SelectedIndex = -1;
            Changed?.Invoke();
        }

        public ProjectionResult Project()
        {
            Locked = true;
            var result = Runtime.Evaluate();
            Changed?.Invoke();
            return result;
        }

        public void Unlock()
        {
            Locked = false;
            Changed?.Invoke();
        }

        /// <summary>Rating for a successful projection.</summary>
        public Rating RateSuccess()
        {
            if (Runtime.IsTutorial) return Rating.Aligned;
            bool atOptimal = true;
            for (int i = 0; i < Runtime.Plates.Count; i++)
                if (!Runtime.Plates[i].Transform.Equals(Runtime.Optimal[i])) { atOptimal = false; break; }
            if (atOptimal) return Rating.PerfectRegister;
            if (MoveCount <= Runtime.MovePar) return Rating.Precise;
            return Rating.Aligned;
        }

        void ApplyParallax(int i) => Runtime.Plates[i].ExtraShift = Runtime.ParallaxShiftFor(i);
        bool ValidIndex(int i) => i >= 0 && i < Runtime.Plates.Count;
    }
}
