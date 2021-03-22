using Characters;
using UnityEngine;

namespace Terrain
{
    public class Terrain : MonoBehaviour
    {
        /// <summary>
        /// The default defense boost a character gets from occupying this terrain
        /// </summary>
        public int DefenseBoost { get; set; } = 1;

        /// <summary>
        /// The default movement cost for a character to move to this terrain
        /// </summary>
        public int MovementCost { get; set; } = 1;

        /// <summary>
        /// The display name of this terrain
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The default hit percentage boost a character gets for occupying this
        /// terrain
        /// </summary>
        public int HitPercentageBoost { get; set; } = 0;

        public virtual bool IsPassable(Character character, float x, float y)
        {
            return true;
        }
    }
}