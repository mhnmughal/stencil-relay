using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StencilRelay.Levels
{
    /// <summary>Ordered list of the 13 level definitions (index 0 = Calibration).</summary>
    [CreateAssetMenu(menuName = "Stencil Relay/Level Database", fileName = "LevelDatabase")]
    public sealed class LevelDatabase : ScriptableObject
    {
        public List<StencilLevelDefinition> levels = new List<StencilLevelDefinition>();

        public int Count => levels.Count;

        public StencilLevelDefinition ByIndex(int index)
            => (index >= 0 && index < levels.Count) ? levels[index] : null;

        public StencilLevelDefinition ById(int levelId)
            => levels.FirstOrDefault(l => l != null && l.levelId == levelId);

        /// <summary>Ordered definitions, sorted by levelId (defensive against list mis-ordering).</summary>
        public IEnumerable<StencilLevelDefinition> Ordered
            => levels.Where(l => l != null).OrderBy(l => l.levelId);
    }
}
