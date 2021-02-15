using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.UI
{
    public class MenuTests : GameManagerTest
    {
        [UnityTest]
        public IEnumerator SelectMenuItemTest1()
        {
            GameManager.CharacterActionMenu.CurrentMenuItemIndex = -1;
            GameManager.CharacterActionMenu.SelectMenuItem();
            LogAssert.Expect(LogType.Error, "Invalid menu index: -1");
            yield return null;
        }

        [UnityTest]
        public IEnumerator OnArrowTest()
        {
            int index = GameManager.CharacterActionMenu.CurrentMenuItemIndex;
            GameManager.CharacterActionMenu.OnArrow(0, 0);
            Assert.AreEqual(index, GameManager.CharacterActionMenu.CurrentMenuItemIndex);
            yield return null;
        }

        [UnityTest]
        public IEnumerator OnInformationTest()
        {
            yield return Information(GameManager.CharacterActionMenu);
            LogAssert.Expect(LogType.Log, "Menu.OnArrow not implemented");
        }
    }
}
