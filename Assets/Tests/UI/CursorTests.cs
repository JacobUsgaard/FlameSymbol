using System.Collections;
using NUnit.Framework;
using UI;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.UI
{
    public class CursorTests : UITest
    {

        [UnityTest]
        public IEnumerator OnArrowTest1()
        {
            yield return null;
            Cursor cursor = GameManager.Cursor;
            Path path = cursor.Path;

            // Move cursor
            cursor.OnArrow(1f, 0f);
            yield return null;

            Assert.AreEqual(new Vector3(1, 0, 0), cursor.transform.position);
        }

        [UnityTest]
        public IEnumerator OnArrowTest2()
        {
            yield return null;
            Cursor cursor = GameManager.Cursor;
            Path path = cursor.Path;

            // Move cursor
            cursor.OnArrow(1f, 0f);
            cursor.OnArrow(1f, 0f);
            yield return null;

            Assert.AreEqual(new Vector3(2, 0, 0), cursor.transform.position);
        }

        [UnityTest]
        public IEnumerator OnArrowTest3()
        {
            yield return null;
            Cursor cursor = GameManager.Cursor;
            Path path = cursor.Path;

            // Move cursor
            cursor.OnArrow(0f, 1f);
            yield return null;

            Assert.AreEqual(new Vector3(0, 1, 0), cursor.transform.position);
        }

        [UnityTest]
        public IEnumerator OnArrowTest4()
        {
            yield return null;
            Cursor cursor = GameManager.Cursor;
            Path path = cursor.Path;

            // Move cursor
            cursor.OnArrow(0f, 1f);
            cursor.OnArrow(0f, 1f);
            yield return null;

            Assert.AreEqual(new Vector3(0, 2, 0), cursor.transform.position);
        }

        [UnityTest]
        public IEnumerator OnArrowTest5()
        {
            yield return null;
            Cursor cursor = GameManager.Cursor;
            Path path = cursor.Path;

            // Move cursor
            cursor.OnArrow(0f, 1f);
            cursor.OnArrow(1f, 0f);
            yield return null;

            Assert.AreEqual(new Vector3(1, 1, 0), cursor.transform.position);
        }

        [UnityTest]
        public IEnumerator OnArrowTest6()
        {
            yield return null;
            Cursor cursor = GameManager.Cursor;
            Path path = cursor.Path;

            // Move cursor
            cursor.OnArrow(0f, 1f);
            cursor.OnArrow(0f, -1f);
            yield return null;

            Assert.AreEqual(new Vector3(0, 0, 0), cursor.transform.position);
        }

        [UnityTest]
        public IEnumerator OnArrowTest7()
        {
            yield return null;
            Cursor cursor = GameManager.Cursor;
            Path path = cursor.Path;

            // Move cursor
            cursor.OnArrow(1f, 0f);
            cursor.OnArrow(-1f, 0f);
            yield return null;

            Assert.AreEqual(new Vector3(0, 0, 0), cursor.transform.position);
        }

        [UnityTest]
        public IEnumerator MoveTest1()
        {
            yield return null;
            Cursor cursor = GameManager.Cursor;
            Path path = cursor.Path;

            // Move cursor
            cursor.Move(0, 1);
            yield return null;

            Assert.AreEqual(new Vector3(0, 1, 0), cursor.transform.position);
        }

        [UnityTest]
        public IEnumerator MoveTest2()
        {
            yield return null;
            Cursor cursor = GameManager.Cursor;
            Path path = cursor.Path;

            // Move cursor
            cursor.Move(1, 0);
            yield return null;

            Assert.AreEqual(new Vector3(1, 0, 0), cursor.transform.position);
        }

        [UnityTest]
        public IEnumerator MoveTest3()
        {
            yield return null;
            Cursor cursor = GameManager.Cursor;
            Path path = cursor.Path;

            // Move cursor
            cursor.Move(2, 2);
            yield return null;

            Assert.AreEqual(new Vector3(2, 2, 0), cursor.transform.position);
        }

        [UnityTest]
        public IEnumerator AttackablePositionsTest1()
        {
            yield return null;

            Cursor cursor = GameManager.Cursor;
            Character character = GameManager.CurrentLevel.GetCharacter(cursor.transform.position);

            Assert.AreEqual(3, character.AttackableSpaces.Count);
            Assert.AreEqual(3, character.MovableSpaces.Count);
        }

        [UnityTest]
        public IEnumerator AttackablePositionsTest2()
        {
            yield return null;

            Cursor cursor = GameManager.Cursor;
            cursor.OnArrow(1, 0);
            cursor.OnArrow(0, 1);
            cursor.OnArrow(0, 1);

            yield return null;

            Character character = GameManager.CurrentLevel.GetCharacter(cursor.transform.position);

            Assert.AreEqual(0, character.AttackableSpaces.Count);
            Assert.AreEqual(9, character.MovableSpaces.Count);
        }

        [UnityTest]
        public IEnumerator OnSubmitTest1()
        {

            yield return null;
        }
    }
}
