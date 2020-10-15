using System.Collections;
using NUnit.Framework;
using Tests.UI;
using UI;
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
    }
}
