using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UI;
using System.Collections.Generic;

namespace Tests.UI
{
    public class PathTests : GameManagerTest
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

        /// <summary>
        /// Given: Character is choosing move
        ///     And: Path is full
        /// When: Cursor is moved to a position in movable positions but not already in Path
        /// Then: Path is recalculated
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator RecalculateTest9()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);

            yield return MoveCursor(2, 2);
            yield return Submit();
            yield return DownArrow();
            yield return RightArrow();
            yield return DownArrow();

            Assert.AreEqual(Cursor.State.ChoosingMove, GameManager.Cursor.CurrentState);
            Assert.AreEqual(character.Moves + 1, GameManager.Cursor.Path.Positions.Count);

            yield return LeftArrow();

            Assert.AreNotEqual(character.Moves + 1, GameManager.Cursor.Path.Positions.Count);
        }

        [UnityTest]
        public IEnumerator CalculatePathTest1()
        {
            yield return null;

            yield return MoveCursor(2, 2);
            yield return Submit();

            Path path = GameManager.Cursor.Path;

            Debug.LogFormat("Attackable positions: {0}", string.Join(",", GameManager.CurrentLevel.GetCharacter().AttackableTransforms.ConvertAll(s => s.position)));

            List<Vector2> positions = path.CalculatePath(new Vector2(2, 2), new Vector2(1, 1), 3);
            Debug.LogFormat("Positions: {0}", string.Join(",", positions));
            Assert.AreEqual(3, positions.Count);
        }

        [UnityTest]
        public IEnumerator CalculatePathTest2()
        {
            yield return MoveCursor(2, 1);
            yield return Submit();
            yield return UpArrow();

            Assert.AreEqual(2, GameManager.Cursor.Path.Positions.Count);
        }

        [UnityTest]
        public IEnumerator CalculatePathTest3()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 1);
            yield return MoveCursor(2, 1);
            yield return Submit();
            yield return RightArrow();
            yield return DownArrow();
            yield return LeftArrow();

            Assert.AreEqual(character.Moves + 1, GameManager.Cursor.Path.Positions.Count);

            yield return LeftArrow();

            Assert.AreNotEqual(character.Moves + 1, GameManager.Cursor.Path.Positions.Count);
        }

        [UnityTest]
        public IEnumerator CalculatePathTest4()
        {
            yield return MoveCursor(2, 1);
            yield return Submit();

            List<Vector2> positions = GameManager.Cursor.Path.CalculatePath(new Vector2(2, 1), new Vector2(3, 1), 3);
            Assert.AreEqual(2, positions.Count);
        }

        [UnityTest]
        public IEnumerator CalculatePathTest5()
        {
            yield return MoveCursor(2, 1);
            yield return Submit();

            List<Vector2> positions = GameManager.Cursor.Path.CalculatePath(new Vector2(2, 1), new Vector2(2, 2), 3);
            Assert.AreEqual(2, positions.Count);
        }

        [UnityTest]
        public IEnumerator CalculatePathTest6()
        {
            yield return MoveCursor(2, 1);
            yield return Submit();

            GameManager.Cursor.Path.Positions.Clear();
            GameManager.Cursor.Path.Redraw();

            LogAssert.Expect(LogType.Error, "Invalid number of path positions: 0");
        }

        [UnityTest]
        public IEnumerator CalculatePathTest7()
        {
            GameManager.CurrentLevel.SetCharacter(GameManager.CurrentLevel.GetCharacter(2, 1), 3, 0);

            yield return MoveCursor(2, 1);
            yield return Submit();

            GameManager.Cursor.Path.Positions.Clear();
            GameManager.Cursor.Path.Redraw();

            LogAssert.Expect(LogType.Error, "Invalid number of path positions: 0");
        }

        [UnityTest]
        public IEnumerator RedrawTest1()
        {
            GameManager.CurrentLevel.SetCharacter(GameManager.CurrentLevel.GetCharacter(2, 1), 3, 0);

            yield return MoveCursor(3, 0);
            yield return Submit();

            yield return LeftArrow();
            yield return LeftArrow();
            yield return UpArrow();
            yield return RightArrow();
            yield return RightArrow();
            yield return UpArrow();
        }

        [UnityTest]
        public IEnumerator RedrawTest2()
        {
            GameManager.CurrentLevel.SetCharacter(GameManager.CurrentLevel.GetCharacter(2, 1), 2, 0);

            yield return MoveCursor(2, 0);
            yield return Submit();

            yield return UpArrow();
            yield return UpArrow();
            yield return DownArrow();
            yield return LeftArrow();
            yield return RightArrow();
            yield return RightArrow();
        }

        [UnityTest]
        public IEnumerator RedrawTest3()
        {
            yield return MoveCursor(2, 2);

            GameManager.Cursor.Path.Positions.Clear();
            GameManager.Cursor.Path.Positions.AddRange(new List<Vector2>() {
                new Vector2(0, 0), new Vector2(2, 2), new Vector2(2, 1)
            });

            GameManager.Cursor.Path.Redraw();
            LogAssert.Expect(LogType.Error, "Invalid movement previous: (0.0, 0.0), current: (2.0, 2.0), next: (2.0, 1.0)");
        }

        [Test]
        public void RotatePathEndTest1()
        {
            Path path = ScriptableObject.CreateInstance<Path>();
            path.RotatePathEnd(null, new Vector2(0, 0), new Vector2(2, 0));
            LogAssert.Expect(LogType.Error, "Distance too far. Start: (0.0, 0.0), End: (2.0, 0.0)");
        }
    }
}
