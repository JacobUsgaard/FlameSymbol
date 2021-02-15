using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Logic
{
    public class ManagedMonoBehaviorTests
    {

        [UnityTest]
        public IEnumerator DestroyAllTest()
        {
            GameObject gameObject = new GameObject();
            List<Transform> list = new List<Transform>
            {
                gameObject.transform
            };

            ManagedMonoBehavior.DestroyAll(list);

            Assert.AreEqual(0, list.Count);
            Assert.IsEmpty(gameObject.transform);

            yield return null;
        }
    }
}
