using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class AssertHelper : Assert
    {
        public static void Contains(Vector3 position, ICollection<Transform> transforms)
        {
            foreach (Transform transform in transforms)
            {
                if (transform.position.Equals(position))
                {
                    return;
                }
            }

            Fail("Failed to find {0} in {1}", position, transforms);
        }
    }
}
