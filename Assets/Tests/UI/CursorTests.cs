﻿using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;


namespace Tests.UI
{
    public class CursorTests : UITest
    {

        [UnityTest]
        public IEnumerator OnArrowTest1()
        {
            yield return null;
            Cursor cursor = GameManager.Cursor;

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

            Assert.AreEqual(4, character.AttackableSpaces.Count);
            Assert.AreEqual(6, character.MovableSpaces.Count);
        }

        /// <summary>
        /// Testing a character with no usable weapons
        /// </summary>
        /// <returns></returns>
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
            Assert.AreEqual(13, character.MovableSpaces.Count);
        }

        [UnityTest]
        public IEnumerator OnSubmitTest1()
        {
            yield return MoveCursor(2, 2);

            GameManager.Cursor.OnSubmit();

            Assert.AreEqual(Cursor.State.ChoosingMove, GameManager.Cursor.CurrentState);
        }

        /// <summary>
        /// Test OnInformation on a character
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnInformationTest1()
        {
            // finish setting up game manager
            yield return null;

            // cursor starts on character, need to finish loading that (e.g. movable/attackable spaces
            yield return null;

            GameManager.Cursor.OnInformation();

            yield return null;

            Scene scene = SceneManager.GetSceneByName("CharacterInformation");
            Assert.IsTrue(scene.isLoaded);
        }

        /// <summary>
        /// Test OnInformation on a non-character
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnInformationTest2()
        {
            // finish setting up game manager
            yield return null;

            // cursor starts on character, need to finish loading that (e.g. movable/attackable spaces
            yield return null;

            yield return MoveCursor(1, 0);
            GameManager.Cursor.OnInformation();

            Assert.AreNotEqual("CharacterInformation", SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Test OnInformation when Cursor.State is not Free
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnInformationTest3()
        {
            // finish setting up game manager
            yield return null;

            // cursor starts on character, need to finish loading that (e.g. movable/attackable spaces
            yield return null;

            yield return MoveCursor(2, 2);

            yield return Enter();

            GameManager.Cursor.OnInformation();

            Assert.AreNotEqual("CharacterInformation", SceneManager.GetActiveScene().name);
        }
    }
}
