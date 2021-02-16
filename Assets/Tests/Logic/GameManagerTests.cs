using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UI;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Logic
{
    public class GameManagerTests : GameManagerTest
    {
        /// <summary>
        /// Test whether the GameManager instantiates everything correctly.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator GameManagerTest1()
        {
            Assert.IsNotNull(GameManager);

            Cursor cursor = GameManager.Cursor;
            Assert.IsNotNull(cursor);

            Path path = GameManager.Cursor.Path;
            Assert.IsNotNull(path);

            yield return null;
        }

        [UnityTest]
        public IEnumerator DestroyAllTest()
        {
            GameObject gameObject = new GameObject();
            List<Transform> list = new List<Transform>
            {
                gameObject.transform
            };

            GameManager.DestroyAll(list);

            Assert.AreEqual(0, list.Count);
            Assert.IsEmpty(gameObject.transform);

            yield return null;
        }
    }
}
