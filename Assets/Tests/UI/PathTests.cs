using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UI;
using System.Collections.Generic;

namespace Tests.UI
{
    public class PathTests : UITest
    {

        [Test]
        public void GetDistanceTest1()
        {
            Path path = ScriptableObject.CreateInstance<Path>();
            Vector2 start = new Vector2(1, 1);
            Vector2 end = new Vector2(1, 2);

            Assert.AreEqual(1f, path.GetDistance(start, end));
        }

        [Test]
        public void GetDistanceTest2()
        {
            Path path = ScriptableObject.CreateInstance<Path>();
            Vector2 start = new Vector2(1, 1);
            Vector2 end = new Vector2(1, 3);

            Assert.Greater(path.GetDistance(start, end), 1f);
        }

        [UnityTest]
        public IEnumerator RecalculateTest1()
        {
            yield return null;

            Path path = GameManager.Cursor.Path;
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            path.StartPath(character);

            Assert.AreEqual(character.Moves, path.CalculateRemainingMoves());
            Assert.AreEqual(1, path.Positions.Count);
        }

        [UnityTest]
        public IEnumerator RecalculateTest2()
        {
            yield return null;

            Path path = GameManager.Cursor.Path;
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            path.StartPath(character);
            path.Add(new Vector2(2, 1));

            Assert.AreEqual(character.Moves - 1, path.CalculateRemainingMoves());
            Assert.AreEqual(2, path.Positions.Count);
        }

        [UnityTest]
        public IEnumerator RecalculateTest3()
        {
            yield return null;

            Path path = GameManager.Cursor.Path;
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            path.StartPath(character);
            path.Add(new Vector2(2, 1));
            path.Add(new Vector2(2, 0));


            Assert.AreEqual(character.Moves - 2, path.CalculateRemainingMoves());
            Assert.AreEqual(3, path.Positions.Count);
        }

        [UnityTest]
        public IEnumerator RecalculateTest4()
        {
            yield return null;

            Path path = GameManager.Cursor.Path;
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            path.StartPath(character);
            path.Add(new Vector2(2, 1));
            path.Add(new Vector2(2, 0));
            path.Add(new Vector2(2, 1));

            Assert.AreEqual(character.Moves - 1, path.CalculateRemainingMoves());
            Assert.AreEqual(2, path.Positions.Count);
        }

        [UnityTest]
        public IEnumerator RecalculateTest5()
        {
            yield return null;

            Path path = GameManager.Cursor.Path;
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            path.StartPath(character);
            path.Add(new Vector2(2, 1));
            path.Add(new Vector2(2, 0));
            path.Add(new Vector2(2, 1));
            path.Add(new Vector2(2, 0));

            Assert.AreEqual(character.Moves - 2, path.CalculateRemainingMoves());
            Assert.AreEqual(3, path.Positions.Count);
        }

        [UnityTest]
        public IEnumerator RecalculateTest6()
        {
            yield return null;

            Path path = GameManager.Cursor.Path;
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            path.StartPath(character);
            path.Add(new Vector2(2, 1));
            path.Add(new Vector2(2, 0));
            path.Add(new Vector2(2, 1));
            path.Add(new Vector2(2, 2));

            Assert.AreEqual(character.Moves, path.CalculateRemainingMoves());
            Assert.AreEqual(1, path.Positions.Count);
        }

        [UnityTest]
        public IEnumerator RecalculateTest7()
        {
            yield return null;

            Path path = GameManager.Cursor.Path;
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            character.Moves = 10;
            path.StartPath(character);
            path.Add(new Vector2(2, 1));
            path.Add(new Vector2(2, 0));
            path.Add(new Vector2(3, 0));
            path.Add(new Vector2(3, 1));
            path.Add(new Vector2(2, 1));

            Assert.AreEqual(character.Moves - 1, path.CalculateRemainingMoves());
            Assert.AreEqual(2, path.Positions.Count);
            Assert.AreEqual(path.Positions.Count, path.Transforms.Count + 1);
        }

        [UnityTest]
        public IEnumerator RecalculateTest8()
        {
            yield return null;
            Cursor cursor = GameManager.Cursor;
            Path path = cursor.Path;

            // Move cursor to character
            yield return MoveCursor(2, 2);

            // Press 'Enter'
            cursor.OnSubmit();
            yield return null;

            // Press 'Down Arrow'
            cursor.OnArrow(0, -1f);
            yield return null;

            Assert.AreEqual(2, path.Positions.Count);
        }

        [UnityTest]
        public IEnumerator CalculatePathTest1()
        {
            yield return null;

            yield return MoveCursor(2, 2);
            GameManager.Cursor.OnSubmit();

            Path path = GameManager.Cursor.Path;

            Debug.LogFormat("Attackable positions: {0}", string.Join(",", GameManager.CurrentLevel.GetCharacter().AttackableSpaces.ConvertAll(s => s.position)));

            List<Vector2> positions = path.CalculatePath(new Vector2(2, 2), new Vector2(1, 1), 3);
            Debug.LogFormat("Positions: {0}", string.Join(",", positions));
            Assert.AreEqual(3, positions.Count);
        }
    }
}
