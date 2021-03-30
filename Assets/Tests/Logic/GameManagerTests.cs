using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UI;
using UnityEngine;
using UnityEngine.TestTools;
using Logic;
using Cursor = UI.Cursor;
using Characters;

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

        /// <summary>
        /// End turn from PlayerActionMenu
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator EndTurnTest1()
        {
            Player previousPlayer = GameManager.CurrentPlayer;
            GameManager.Players[1].IsHuman = true;

            Assert.NotNull(previousPlayer);
            yield return MoveCursor(3, 0);

            // Player action menu
            yield return Submit();
            Assert.True(GameManager.PlayerActionMenu.IsInFocus());

            // End Turn
            yield return UpArrow();
            yield return Submit();

            Player currentPlayer = GameManager.CurrentPlayer;
            Assert.NotNull(currentPlayer);
            Assert.AreNotSame(previousPlayer, currentPlayer);
        }

        /// <summary>
        /// End turn after all characters have moved
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator EndTurnTest2()
        {
            Player previousPlayer = GameManager.CurrentPlayer;
            GameManager.Players[1].IsHuman = true;
            Assert.NotNull(previousPlayer);

            foreach (Character character in previousPlayer.Characters)
            {
                character.HasMoved = true;
            }

            yield return null;
            LogAssert.Expect(LogType.Log, "All characters have moved");

            Player currentPlayer = GameManager.CurrentPlayer;
            Assert.NotNull(currentPlayer);
            Assert.AreNotSame(previousPlayer, currentPlayer);
        }

        /// <summary>
        /// Call end turn from wrong player
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator EndTurnTest3()
        {
            GameManager.EndTurn(GameManager.Players[1]);
            LogAssert.Expect(LogType.Error, "Player that is not current player is ending their turn. Current player:  (Logic.Player), calling player:  (Logic.AIPlayer)");
            yield return null;
        }

        [UnityTest]
        public IEnumerator CalculateNextPlayerTest()
        {
            Player player = ScriptableObject.CreateInstance<Player>();
            _ = player.Characters.Add(GameManager.CurrentLevel.GetCharacter(2, 2));
            player.Name = "TestPlayer";
            GameManager.CurrentPlayer = player;
            Player nextPlayer = GameManager.CalculateNextPlayer();
            LogAssert.Expect(LogType.Error, "Unable to find player: TestPlayer");
            Assert.Null(nextPlayer);
            yield return null;
        }

        [UnityTest]
        public IEnumerable PlayerActionMenuOnCancelTest()
        {
            yield return MoveCursor(3, 0);
            yield return Submit();

            Assert.True(GameManager.PlayerActionMenu.IsInFocus());

            yield return Cancel();
            LogAssert.Expect(LogType.Log, "PlayerActionMenuOnCancel");
            Assert.True(GameManager.Cursor.IsInFocus());
            Assert.AreEqual(Cursor.State.Free, GameManager.Cursor.CurrentState);
        }
    }
}
