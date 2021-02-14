using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests.Items
{
    public class ItemTests
    {
        /// <summary>
        /// Given: An item
        /// When: The is broken
        /// Then: Then IsBroken returns true
        /// </summary>
        [Test]
        public void IsBrokenTest()
        {
            FakeItem item = ScriptableObject.CreateInstance<FakeItem>();
            item.UsesTotal = 1;
            item.UsesRemaining = 1;
            Assert.False(item.IsBroken());
        }

        /// <summary>
        /// Given: An item
        ///     And: The item has less than 0 uses
        /// When: The item is used
        /// Then: An error is logged
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator UseTest1()
        {
            FakeItem item = ScriptableObject.CreateInstance<FakeItem>();
            item.UsesRemaining = -1;
            GameObject gameObject = new GameObject("TestObject");
            Text text = gameObject.AddComponent<Text>();
            text.text = "FakeItem";
            item.Text = text;

            item.Use();
            LogAssert.Expect(LogType.Error, "Item FakeItem cannot have negative uses remaining.");
            yield return null;
        }

        /// <summary>
        /// Given: An item
        ///     And: The item has 1 use remaining
        /// When: The item is used
        /// Then: The item breaks
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator UseTest2()
        {
            FakeItem item = ScriptableObject.CreateInstance<FakeItem>();
            item.UsesRemaining = 1;
            GameObject gameObject = new GameObject("TestObject");
            Text text = gameObject.AddComponent<Text>();
            text.text = "FakeItem";
            item.Text = text;

            item.Use();
            LogAssert.Expect(LogType.Log, "Breaking FakeItem");
            yield return null;
        }

        private class FakeItem : Item { }
    }
}
