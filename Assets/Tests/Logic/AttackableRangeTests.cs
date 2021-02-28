using System.Collections;
using Characters;
using Logic;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests.Logic
{
    public class AttackableRangeTests : GameManagerTest
    {
        [UnityTest]
        public IEnumerator AttackableRangeTest1()
        {
            AttackableRange attackableRange = GameManager.Cursor.AttackableRange;

            yield return MoveCursor(1, 2);
            yield return Submit();

            Assert.AreEqual(1, attackableRange.Characters.Count);
            Assert.AreEqual(0, attackableRange.AttackableTransforms.Count);

            yield return MoveCursor(2, 3);
            yield return Submit();
            Assert.AreEqual(2, attackableRange.Characters.Count);
            Assert.AreEqual(9, attackableRange.AttackableTransforms.Count);

            yield return MoveCursor(3, 3);
            yield return Submit();
            Assert.AreEqual(3, attackableRange.Characters.Count);
            Assert.AreEqual(9, attackableRange.AttackableTransforms.Count);

            yield return Cancel();
            Assert.AreEqual(2, attackableRange.Characters.Count);
            Assert.AreEqual(9, attackableRange.AttackableTransforms.Count);

            yield return MoveCursor(2, 3);
            yield return Cancel();
            Assert.AreEqual(1, attackableRange.Characters.Count);
            Assert.AreEqual(0, attackableRange.AttackableTransforms.Count);
        }

        [UnityTest]
        public IEnumerator AttackableRangeTest2()
        {
            AttackableRange attackableRange = GameManager.Cursor.AttackableRange;

            yield return MoveCursor(2, 3);
            yield return Submit();
            Assert.AreEqual(1, attackableRange.Characters.Count);
            Assert.AreEqual(9, attackableRange.AttackableTransforms.Count);

            yield return Cancel();
            Assert.AreEqual(0, attackableRange.Characters.Count);
            Assert.AreEqual(0, attackableRange.AttackableTransforms.Count);

            yield return MoveCursor(1, 2);
            yield return Submit();
            Assert.AreEqual(1, attackableRange.Characters.Count);
            Assert.AreEqual(0, attackableRange.AttackableTransforms.Count);
        }

        [UnityTest]
        public IEnumerator AttackableRangeTest3()
        {
            AttackableRange attackableRange = GameManager.Cursor.AttackableRange;
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);

            attackableRange.RemoveCharacter(character);
            LogAssert.Expect(UnityEngine.LogType.Error, "Cannot remove character Wizard from AttackableRange at (2.0, 2.0, 0.0)");
            yield return null;
        }
    }
}
