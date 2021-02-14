using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Tests.UI
{
    public class CursorTests : GameManagerTest
    {

        [UnityTest]
        public IEnumerator OnArrowTest1()
        {
            Cursor cursor = GameManager.Cursor;

            // Move cursor
            cursor.OnArrow(1f, 0f);
            yield return null;

            Assert.AreEqual(new Vector3(1, 0, 0), cursor.transform.position);
        }

        [UnityTest]
        public IEnumerator OnArrowTest2()
        {
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

            Assert.AreEqual(4, character.AttackableTransforms.Count);
            Assert.AreEqual(6, character.MovableTransforms.Count);
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

            yield return MoveCursor(1, 2);

            yield return null;

            Character character = GameManager.CurrentLevel.GetCharacter(cursor.transform.position);

            Assert.AreEqual(0, character.AttackableTransforms.Count);
            Assert.AreEqual(11, character.MovableTransforms.Count);
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
            //yield return null;

            // cursor starts on character, need to finish loading that (e.g. movable/attackable spaces
            //yield return null;

            yield return Information();

            Scene scene = SceneManager.GetSceneByName("CharacterInformation");
            Assert.IsTrue(scene.isLoaded);

            //_ = Assert.Throws<System.NotImplementedException>(() => DownArrow());

            yield return Cancel();
            Assert.IsFalse(scene.isLoaded);
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

            yield return Information(GameManager.Cursor);

            Assert.AreNotEqual("CharacterInformation", SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// When the cursor is in free state, the terrain information panel
        /// should be visible
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TerrainInformationPanelTest1()
        {
            yield return MoveCursor(2, 2);

            Assert.True(GameManager.TerrainInformationPanel.gameObject.activeSelf);

            yield return MoveCursor(0, 0);

            Assert.True(GameManager.TerrainInformationPanel.gameObject.activeSelf);
        }

        /// <summary>
        /// When the cursor is in choosing move state, the terrain information
        /// panel should be visible.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TerrainInformationPanelTest2()
        {
            yield return MoveCursor(2, 2);

            yield return Submit(GameManager.Cursor);

            Assert.True(GameManager.TerrainInformationPanel.gameObject.activeSelf);
        }

        /// <summary>
        /// When the the character action menu is open, the terrain information
        /// panel should not be visible.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TerrainInformationPanelTest3()
        {
            yield return MoveCursor(2, 2);

            yield return Submit(GameManager.Cursor);

            yield return Submit(GameManager.Cursor);

            Assert.False(GameManager.TerrainInformationPanel.gameObject.activeSelf);
        }

        /// <summary>
        /// When the cursor is in free state and is over a position with a
        /// character, the character information panel should be visible
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator CharacterInformationPanelTest1()
        {
            yield return MoveCursor(2, 2);

            Assert.True(GameManager.CharacterInformationPanel.gameObject.activeSelf);

            yield return MoveCursor(0, 0);

            Assert.True(GameManager.CharacterInformationPanel.gameObject.activeSelf);

            yield return MoveCursor(1, 0);

            Assert.False(GameManager.CharacterInformationPanel.gameObject.activeSelf);
        }

        /// <summary>
        /// When a character is choosing a move and the cursor is over a
        /// character, the character information panel should be visible
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator CharacterInformationPanelTest2()
        {
            yield return MoveCursor(2, 2);

            yield return Submit(GameManager.Cursor);

            Assert.True(GameManager.CharacterInformationPanel.gameObject.activeSelf);

            yield return MoveCursor(2, 0);

            Assert.False(GameManager.CharacterInformationPanel.gameObject.activeSelf);

            yield return MoveCursor(0, 1);

            Assert.True(GameManager.CharacterInformationPanel.gameObject.activeSelf);
        }

        /// <summary>
        /// When the the character action menu is open, the terrain information
        /// panel should not be visible.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator CharacterInformationPanelTest3()
        {
            yield return MoveCursor(2, 2);

            yield return Submit(GameManager.Cursor);

            yield return Submit(GameManager.Cursor);

            Assert.False(GameManager.CharacterInformationPanel.gameObject.activeSelf);
        }

        /// <summary>
        /// When submit is pressed outside of movable/attackable spaces, nothing should happen
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnSubmitOutsideMovableAndAttackableSpaces()
        {
            yield return MoveCursor(2, 2);

            yield return Submit(GameManager.Cursor);

            Character character = GameManager.Cursor.SelectedCharacter;
            List<Transform> movableSpaces = character.MovableTransforms;
            List<Transform> attackableSpaces = character.AttackableTransforms;

            yield return MoveCursor(0, 3);

            Assert.AreEqual(character, GameManager.Cursor.SelectedCharacter);
            Assert.AreEqual(movableSpaces, character.MovableTransforms);
            Assert.AreEqual(attackableSpaces, character.AttackableTransforms);
        }

        [UnityTest]
        public IEnumerator AttackableRangeTest1()
        {
            yield return MoveCursor(0, 0);

            yield return Submit(GameManager.Cursor);
            Assert.AreEqual(10, GameManager.Cursor.AttackableRange.AttackableTransforms.Count);

            yield return Submit(GameManager.Cursor);
            Assert.AreEqual(10, GameManager.Cursor.AttackableRange.AttackableTransforms.Count);

            yield return MoveCursor(0, 1);
            yield return Submit(GameManager.Cursor);
            Assert.AreEqual(12, GameManager.Cursor.AttackableRange.AttackableTransforms.Count);

            yield return Cancel(GameManager.Cursor);
            Assert.AreEqual(10, GameManager.Cursor.AttackableRange.AttackableTransforms.Count);

            yield return Cancel(GameManager.Cursor);
            Assert.AreEqual(0, GameManager.Cursor.AttackableRange.AttackableTransforms.Count);
        }

        [UnityTest]
        public IEnumerator CharacterActionMenuOnCancelTest()
        {
            yield return MoveCursor(2, 2);

            yield return Submit(GameManager.Cursor);

            yield return Submit(GameManager.Cursor);

            Assert.True(GameManager.CharacterActionMenu.IsInFocus());

            yield return Cancel();

            Assert.True(GameManager.Cursor.IsInFocus());

            Assert.AreEqual(Cursor.State.ChoosingMove, GameManager.Cursor.CurrentState);
        }

        [UnityTest]
        public IEnumerator CharacterActionMenuWaitTest()
        {
            yield return MoveCursor(2, 2);

            yield return Submit(GameManager.Cursor);

            yield return Submit(GameManager.Cursor);

            Assert.True(GameManager.CharacterActionMenu.IsInFocus());

            yield return DownArrow(GameManager.CharacterActionMenu);
            yield return DownArrow(GameManager.CharacterActionMenu);
            yield return DownArrow(GameManager.CharacterActionMenu);

            yield return Submit(GameManager.CharacterActionMenu);

            Assert.True(GameManager.Cursor.IsInFocus());
            Assert.False(GameManager.CharacterActionMenu.gameObject.activeSelf);

        }

        [UnityTest]
        public IEnumerator ItemDetailMenuOnCancelTest()
        {
            yield return MoveCursor(2, 2);

            yield return Submit(GameManager.Cursor);

            yield return Submit(GameManager.Cursor);

            Assert.True(GameManager.CharacterActionMenu.gameObject.activeSelf);
            Assert.True(GameManager.CharacterActionMenu.IsInFocus());

            yield return DownArrow(GameManager.CharacterActionMenu);
            yield return DownArrow(GameManager.CharacterActionMenu);

            yield return Submit(GameManager.CharacterActionMenu);
            Assert.True(GameManager.ItemDetailMenu.gameObject.activeSelf);
            Assert.False(GameManager.CharacterActionMenu.gameObject.activeSelf);


            yield return Cancel(GameManager.ItemDetailMenu);
            Assert.False(GameManager.ItemDetailMenu.gameObject.activeSelf);
            Assert.True(GameManager.CharacterActionMenu.gameObject.activeSelf);
        }

        [UnityTest]
        public IEnumerator ItemSelectionMenuOnCancelTest()
        {
            yield return MoveCursor(2, 2);

            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.CharacterActionMenu);

            Assert.True(GameManager.ItemSelectionMenu.gameObject.activeSelf);

            yield return Cancel(GameManager.ItemSelectionMenu);

            Assert.False(GameManager.ItemSelectionMenu.gameObject.activeSelf);
            Assert.True(GameManager.CharacterActionMenu.gameObject.activeSelf);
        }

        [UnityTest]
        public IEnumerator ItemDetailMenuOnSelectTest()
        {
            yield return MoveCursor(2, 2);

            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.Cursor);

            yield return DownArrow(GameManager.CharacterActionMenu);
            yield return DownArrow(GameManager.CharacterActionMenu);
            yield return Submit(GameManager.CharacterActionMenu);

            Assert.False(GameManager.CharacterActionMenu.gameObject.activeSelf);
            Assert.True(GameManager.ItemDetailMenu.gameObject.activeSelf);

            yield return Submit(GameManager.ItemDetailMenu);

            Assert.False(GameManager.ItemDetailMenu.gameObject.activeSelf);
            Assert.True(GameManager.ItemActionMenu.gameObject.activeSelf);
        }

        [UnityTest]
        public IEnumerator ItemActionMenuOnCancelTest()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);

            yield return MoveCursor(2, 2);

            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.Cursor);

            yield return DownArrow(GameManager.CharacterActionMenu);
            yield return DownArrow(GameManager.CharacterActionMenu);
            yield return Submit(GameManager.CharacterActionMenu);

            Assert.False(GameManager.CharacterActionMenu.gameObject.activeSelf);
            Assert.True(GameManager.ItemDetailMenu.gameObject.activeSelf);

            yield return Submit(GameManager.ItemDetailMenu);

            Assert.False(GameManager.ItemDetailMenu.gameObject.activeSelf);
            Assert.True(GameManager.ItemActionMenu.gameObject.activeSelf);

            yield return Cancel(GameManager.ItemActionMenu);
            Assert.False(GameManager.ItemActionMenu.gameObject.activeSelf);
            Assert.True(GameManager.ItemDetailMenu.gameObject.activeSelf);

            Assert.AreEqual(character.Items.Count, GameManager.ItemDetailMenu.MenuItems.Count);
        }

        [UnityTest]
        public IEnumerator ItemActionMenuEquipTest()
        {
            yield return MoveCursor(2, 2);

            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.Cursor);

            yield return DownArrow(GameManager.CharacterActionMenu);
            yield return DownArrow(GameManager.CharacterActionMenu);
            yield return Submit(GameManager.CharacterActionMenu);

            Assert.False(GameManager.CharacterActionMenu.gameObject.activeSelf);
            Assert.True(GameManager.ItemDetailMenu.gameObject.activeSelf);

            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            Item equippedItem = character.Items[0];
            Item nonEquippedItem = character.Items[1];

            yield return DownArrow(GameManager.ItemDetailMenu);

            yield return Submit(GameManager.ItemDetailMenu);

            Assert.False(GameManager.ItemDetailMenu.gameObject.activeSelf);
            Assert.True(GameManager.ItemActionMenu.gameObject.activeSelf);

            yield return Submit(GameManager.ItemActionMenu);

            Assert.False(GameManager.ItemActionMenu.gameObject.activeSelf);
            Assert.True(GameManager.ItemDetailMenu.gameObject.activeSelf);

            Assert.AreEqual(nonEquippedItem, character.Items[0]);
            Assert.AreEqual(equippedItem, character.Items[1]);
        }

        [UnityTest]
        public IEnumerator ItemActionMenuDropTest1()
        {
            yield return MoveCursor(2, 2);

            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.Cursor);

            yield return DownArrow(GameManager.CharacterActionMenu);
            yield return DownArrow(GameManager.CharacterActionMenu);
            yield return Submit(GameManager.CharacterActionMenu);

            Assert.False(GameManager.CharacterActionMenu.gameObject.activeSelf);
            Assert.True(GameManager.ItemDetailMenu.gameObject.activeSelf);

            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            int count = character.Items.Count;

            Assert.True(count > 0);

            Item equippedItem = character.Items[0];

            yield return Submit(GameManager.ItemDetailMenu);

            Assert.False(GameManager.ItemDetailMenu.gameObject.activeSelf);
            Assert.True(GameManager.ItemActionMenu.gameObject.activeSelf);

            yield return DownArrow(GameManager.ItemActionMenu);
            yield return DownArrow(GameManager.ItemActionMenu);
            yield return Submit(GameManager.ItemActionMenu);

            Assert.False(GameManager.ItemActionMenu.gameObject.activeSelf);
            Assert.True(GameManager.ItemDetailMenu.gameObject.activeSelf);

            Assert.AreEqual(count - 1, character.Items.Count);
            Assert.False(character.Items.Exists(item => item.Equals(equippedItem)));
        }

        /// <summary>
        /// Test dropping the character's only item should bring them back to
        /// the character action menu.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator ItemActionMenuDropTest2()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            character.Items.Clear();
            character.Items.Add(Fire.Create());

            yield return MoveCursor(2, 2);

            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.Cursor);

            yield return DownArrow(GameManager.CharacterActionMenu);
            yield return DownArrow(GameManager.CharacterActionMenu);
            yield return Submit(GameManager.CharacterActionMenu);

            Assert.False(GameManager.CharacterActionMenu.gameObject.activeSelf);
            Assert.True(GameManager.ItemDetailMenu.gameObject.activeSelf);

            Assert.True(character.Items.Count == 1);

            yield return Submit(GameManager.ItemDetailMenu);

            Assert.False(GameManager.ItemDetailMenu.gameObject.activeSelf);
            Assert.True(GameManager.ItemActionMenu.gameObject.activeSelf);

            yield return DownArrow(GameManager.ItemActionMenu);
            yield return DownArrow(GameManager.ItemActionMenu);
            yield return Submit(GameManager.ItemActionMenu);

            Assert.False(GameManager.ItemActionMenu.gameObject.activeSelf);
            Assert.True(GameManager.CharacterActionMenu.gameObject.activeSelf);

            Assert.AreEqual(0, character.Items.Count);
        }

        [UnityTest]
        public IEnumerator ItemActionMenuUseTest()
        {
            yield return MoveCursor(2, 2);

            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.Cursor);

            yield return DownArrow(GameManager.CharacterActionMenu);
            yield return DownArrow(GameManager.CharacterActionMenu);
            yield return Submit(GameManager.CharacterActionMenu);

            yield return Submit(GameManager.ItemDetailMenu);
            yield return DownArrow(GameManager.ItemActionMenu);

            _ = Assert.Throws<System.NotImplementedException>(() =>
              {
                  GameManager.ItemActionMenu.OnSubmit();
              });
        }

        /// <summary>
        /// Move test with hiding the information panels
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator MoveTest4()
        {
            GameManager.Cursor.Move(new Vector2(2, 2), false);
            yield return null;

            Assert.AreEqual(new Vector3(2, 2, 0), GameManager.Cursor.transform.position);
            Assert.False(GameManager.CharacterInformationPanel.gameObject.activeSelf);
            Assert.False(GameManager.TerrainInformationPanel.gameObject.activeSelf);
        }

        /// <summary>
        /// Given: Cursor.CurrentState is ChoosingAttackTarget
        ///     And: Two characters are within range
        /// When: An arrow is pressed
        /// Then: Cursor switches between those two characters
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnArrowChoosingAttackTargetTest1()
        {
            yield return MoveCursor(2, 2);

            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.CharacterActionMenu);
            yield return Submit(GameManager.ItemSelectionMenu);

            Assert.AreEqual(Cursor.State.ChoosingAttackTarget, GameManager.Cursor.CurrentState);

            Character originalTarget = GameManager.CurrentLevel.GetCharacter(GameManager.Cursor.transform.position);

            yield return DownArrow(GameManager.Cursor);

            Assert.AreNotSame(originalTarget, GameManager.CurrentLevel.GetCharacter(GameManager.Cursor.transform.position));
        }

        /// <summary>
        /// Given Cursor.CurrentState is ChoosingAttackTarget
        /// And one character is within range
        /// When arrow is pressed
        /// Then cursor should remain on that one character
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnArrowChoosingAttackTargetTest2()
        {
            GameManager.CurrentLevel.SetCharacter(GameManager.CurrentLevel.GetCharacter(2, 2), 2, 0);

            yield return MoveCursor(2, 0);
            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.CharacterActionMenu);
            yield return Submit(GameManager.ItemSelectionMenu);

            Assert.AreEqual(Cursor.State.ChoosingAttackTarget, GameManager.Cursor.CurrentState);

            Character originalTarget = GameManager.CurrentLevel.GetCharacter(GameManager.Cursor.transform.position);

            yield return DownArrow(GameManager.Cursor);

            Assert.AreSame(originalTarget, GameManager.CurrentLevel.GetCharacter(GameManager.Cursor.transform.position));
        }

        /// <summary>
        /// Given Cursor.CurrentState is ChoosingAssistTarget
        /// And two characters are within range
        /// When arrow is pressed
        /// Then cursor switches between those two characters
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnArrowChoosingAssistTargetTest1()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            character.Items.Clear();
            character.AddProficiency(new Proficiency(typeof(HealingStaff), Proficiency.Rank.A));
            character.Items.Add(Heal.Create());
            character.Move(new Vector2(3, 1));

            // Adding second character
            Character target = GameManager.CurrentLevel.Create<Character>(GameManager.WizardPrefab);
            target.Player = GameManager.CurrentLevel.HumanPlayer;
            GameManager.CurrentLevel.DrawCharacter(target, 3, 0);

            yield return MoveCursor(3, 1);

            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.CharacterActionMenu);
            yield return Submit(GameManager.ItemSelectionMenu);

            Assert.AreEqual(Cursor.State.ChoosingAssistTarget, GameManager.Cursor.CurrentState);

            Character originalAssistTarget = GameManager.CurrentLevel.GetCharacter(GameManager.Cursor.transform.position);
            yield return DownArrow(GameManager.Cursor);

            Assert.AreNotSame(originalAssistTarget, GameManager.CurrentLevel.GetCharacter(GameManager.Cursor.transform.position));
        }

        /// <summary>
        /// Given Cursor.CurrentState is ChoosingAssistTarget
        /// And one character is within range
        /// When arrow is pressed
        /// Then cursor should remain on that one character
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnArrowChoosingAssistTargetTest2()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            character.Items.Clear();
            character.AddProficiency(new Proficiency(typeof(HealingStaff), Proficiency.Rank.A));
            character.Items.Add(Heal.Create());


            yield return MoveCursor(2, 2);

            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.CharacterActionMenu);
            yield return Submit(GameManager.ItemSelectionMenu);

            Assert.AreEqual(Cursor.State.ChoosingAssistTarget, GameManager.Cursor.CurrentState);

            Character originalAssistTarget = GameManager.CurrentLevel.GetCharacter(GameManager.Cursor.transform.position);
            yield return DownArrow(GameManager.Cursor);

            Assert.AreSame(originalAssistTarget, GameManager.CurrentLevel.GetCharacter(GameManager.Cursor.transform.position));
        }

        /// <summary>
        /// Given Cursor.CurrentState is ChoosingTradeTarget
        /// And two characters are within range
        /// When arrow is pressed
        /// Then cursor switches between those two characters
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnArrowChoosingTradeTargetTest1()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            character.Items.Clear();
            character.AddProficiency(new Proficiency(typeof(HealingStaff), Proficiency.Rank.A));
            character.Items.Add(Heal.Create());
            character.Move(new Vector2(3, 1));

            // Adding second character
            Character target = GameManager.CurrentLevel.Create<Character>(GameManager.WizardPrefab);
            target.Player = GameManager.CurrentLevel.HumanPlayer;
            GameManager.CurrentLevel.DrawCharacter(target, 3, 0);

            yield return MoveCursor(3, 1);

            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.Cursor);
            Debug.LogFormat("Menu item: {0}", GameManager.CharacterActionMenu.MenuItems[GameManager.CharacterActionMenu.CurrentMenuItemIndex].DisplayText.text);
            yield return DownArrow(GameManager.CharacterActionMenu);
            Debug.LogFormat("Menu item: {0}", GameManager.CharacterActionMenu.MenuItems[GameManager.CharacterActionMenu.CurrentMenuItemIndex].DisplayText.text);

            yield return Submit(GameManager.CharacterActionMenu);


            Assert.AreEqual(Cursor.State.ChoosingTradeTarget, GameManager.Cursor.CurrentState);

            Character originalAssistTarget = GameManager.CurrentLevel.GetCharacter(GameManager.Cursor.transform.position);
            yield return DownArrow(GameManager.Cursor);

            Assert.AreNotSame(originalAssistTarget, GameManager.CurrentLevel.GetCharacter(GameManager.Cursor.transform.position));
        }

        /// <summary>
        /// Given Cursor.CurrentState is ChoosingTradeTarget
        /// And one character is within range
        /// When arrow is pressed
        /// Then cursor should remain on that one character
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnArrowChoosingTradeTargetTest2()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            character.Items.Clear();
            character.AddProficiency(new Proficiency(typeof(HealingStaff), Proficiency.Rank.A));
            character.Items.Add(Heal.Create());

            yield return MoveCursor(2, 2);

            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.Cursor);
            yield return DownArrow(GameManager.CharacterActionMenu);
            yield return Submit(GameManager.CharacterActionMenu);

            Assert.AreEqual(Cursor.State.ChoosingTradeTarget, GameManager.Cursor.CurrentState);

            Character originalAssistTarget = GameManager.CurrentLevel.GetCharacter(GameManager.Cursor.transform.position);
            yield return DownArrow(GameManager.Cursor);

            Assert.AreSame(originalAssistTarget, GameManager.CurrentLevel.GetCharacter(GameManager.Cursor.transform.position));

        }

        /// <summary>
        /// Given Cursor is in unknown state
        /// And cursor is in focus
        /// When an arrow is pressed
        /// Then an error is logged
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnArrowUnknownStateTest()
        {
            GameManager.Cursor.CurrentState = Cursor.State.Error;
            yield return DownArrow(GameManager.Cursor);

            LogAssert.Expect(LogType.Error, "Invalid Cursor.State in OnArrow: Error");
        }
        /// <summary>
        /// Given Cursor is in unknown state
        /// And cursor is in focus
        /// When submit is pressed
        /// Then an error is logged
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnSubmitUnknownStateTest()
        {
            GameManager.Cursor.CurrentState = Cursor.State.Error;
            yield return Submit(GameManager.Cursor);

            LogAssert.Expect(LogType.Error, "Invalid Cursor.State in OnSubmit: Error");
        }

        /// <summary>
        /// When the cursor is at the edge of the board and an arrow key is
        /// pressed, attempting to move it off the board. The cursor should
        /// stay where it is.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnArrowFreeTest()
        {
            Vector3 originalPosition = GameManager.Cursor.transform.position;
            yield return DownArrow(GameManager.Cursor);
            Assert.AreEqual(originalPosition, GameManager.Cursor.transform.position);
        }

        /// <summary>
        /// When a character is choosing a move and the cursor is at the edge of
        /// the board and an arrow key is pressed, attempting to move it off the
        /// board. The cursor should stay where it is.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnArrowChoosingMoveTest()
        {
            yield return MoveCursor(2, 2);
            yield return Submit(GameManager.Cursor);
            yield return MoveCursor(0, 0);
            Assert.AreEqual(Cursor.State.ChoosingMove, GameManager.Cursor.CurrentState);

            Vector3 originalPosition = GameManager.Cursor.transform.position;
            yield return DownArrow(GameManager.Cursor);
            Assert.AreEqual(originalPosition, GameManager.Cursor.transform.position);
        }

        /// <summary>
        /// Given: Cursor.CurrentState is ChoosingMove
        ///     And: Cursor is over a position with a character of same player
        /// When: Enter is pressed
        /// Then: The character should stay where they are
        ///     And: Cursor stays in ChoosingMove
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnSubmitChoosingMoveTest1()
        {
            yield return MoveCursor(2, 2);
            yield return Submit(GameManager.Cursor);
            yield return MoveCursor(2, 1);

            Assert.AreEqual(Cursor.State.ChoosingMove, GameManager.Cursor.CurrentState);
            Assert.NotNull(GameManager.CurrentLevel.GetCharacter(1, 2));

            Character currentCharacter = GameManager.CurrentLevel.GetCharacter(2, 2);
            Character targetCharacter = GameManager.CurrentLevel.GetCharacter(2, 1);
            Vector3 originalPosition = GameManager.Cursor.transform.position;
            yield return Submit(GameManager.Cursor);

            Assert.AreEqual(Cursor.State.ChoosingMove, GameManager.Cursor.CurrentState);
            Assert.AreEqual(originalPosition, GameManager.Cursor.transform.position);
            Assert.AreSame(currentCharacter, GameManager.CurrentLevel.GetCharacter(2, 2));
            Assert.AreSame(targetCharacter, GameManager.CurrentLevel.GetCharacter(2, 1));
        }

        /// <summary>
        /// Given: Cursor.CurrentState is ChoosingMove
        ///     And: Cursor is over a position not in movable positions
        /// When: Enter is pressed
        /// Then: The character should stay where they are
        ///     And: Cursor.CurrentState stays in ChoosingMove
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnSubmitChoosingMoveTest2()
        {
            yield return MoveCursor(2, 2);
            yield return Submit(GameManager.Cursor);
            yield return MoveCursor(1, 2);

            Assert.AreEqual(Cursor.State.ChoosingMove, GameManager.Cursor.CurrentState);
            Assert.NotNull(GameManager.CurrentLevel.GetCharacter(1, 2));

            Character currentCharacter = GameManager.CurrentLevel.GetCharacter(2, 2);
            Character targetCharacter = GameManager.CurrentLevel.GetCharacter(1, 2);
            Vector3 originalPosition = GameManager.Cursor.transform.position;
            yield return Submit(GameManager.Cursor);

            Assert.AreEqual(Cursor.State.ChoosingMove, GameManager.Cursor.CurrentState);
            Assert.AreEqual(originalPosition, GameManager.Cursor.transform.position);
            Assert.AreSame(currentCharacter, GameManager.CurrentLevel.GetCharacter(2, 2));
            Assert.AreSame(targetCharacter, GameManager.CurrentLevel.GetCharacter(1, 2));
        }

        /// <summary>
        /// Given: Cursor.CurrentState is Free
        ///     And: Cursor is in focus
        /// When: Enter is pressed
        /// Then: The player action menu should be active
        ///     And: The player action menu is in focus
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnSubmitFreeTest()
        {
            yield return MoveCursor(1, 0);
            yield return Submit(GameManager.Cursor);

            Assert.True(GameManager.PlayerActionMenu.gameObject.activeSelf);
            Assert.True(GameManager.PlayerActionMenu.IsInFocus());
        }

        /// <summary>
        /// Given: Cursor.CurrentState is ChoosingMove
        /// When: Cancel is pressed
        /// Then: Cursor.CurrentState is Free
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnCancelChoosingMoveTest()
        {
            yield return MoveCursor(2, 2);
            yield return Submit(GameManager.Cursor);
            yield return Cancel();

            Assert.AreEqual(0, GameManager.Cursor.Path.Positions.Count);
            Assert.AreEqual(Cursor.State.Free, GameManager.Cursor.CurrentState);
        }

        /// <summary>
        /// Given: Cursor.CurrentState is ChoosingAssistTarget
        /// When: Cancel is pressed
        /// Then: ItemSelectionMenu is active
        ///     And: ItemSelectionMenu is in focus
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnCancelChoosingAssistTargetTest()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            character.Items.Clear();
            character.AddProficiency(new Proficiency(typeof(HealingStaff), Proficiency.Rank.A));
            character.Items.Add(Heal.Create());

            yield return MoveCursor(2, 2);
            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.CharacterActionMenu);
            yield return Submit(GameManager.ItemSelectionMenu);

            Assert.AreEqual(Cursor.State.ChoosingAssistTarget, GameManager.Cursor.CurrentState);
            Assert.True(GameManager.Cursor.IsInFocus());

            yield return Cancel();

            Assert.True(GameManager.ItemSelectionMenu.gameObject.activeSelf);
            Assert.True(GameManager.ItemSelectionMenu.IsInFocus());
        }

        /// <summary>
        /// Given: Cursor.CurrentState is ChoosingAttackTarget
        /// When: Cancel is pressed
        /// Then: ItemSelectionMenu is active
        ///     And: ItemSelectionMenu is in focus
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnCancelChoosingAttackTarget()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            character.Items.Clear();
            character.AddProficiency(new Proficiency(typeof(FireMagic), Proficiency.Rank.A));
            character.Items.Add(Fire.Create());

            yield return MoveCursor(2, 2);
            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.CharacterActionMenu);
            yield return Submit(GameManager.ItemSelectionMenu);

            Assert.AreEqual(Cursor.State.ChoosingAttackTarget, GameManager.Cursor.CurrentState);
            Assert.True(GameManager.Cursor.IsInFocus());

            yield return Cancel();

            Assert.True(GameManager.ItemSelectionMenu.gameObject.activeSelf);
            Assert.True(GameManager.ItemSelectionMenu.IsInFocus());
        }

        /// <summary>
        /// Given: Cursor.CurrentState is ChoosingTradeTarget
        /// When: Cancel is pressed
        /// Then: CharacterActionMenu is active
        ///     And: CharacterActionMenu is in focus
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnCancelChoosingTradeTarget()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            character.Proficiencies.Clear();
            character.Items.Clear();
            character.Items.Add(Fire.Create());

            yield return MoveCursor(2, 2);
            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.Cursor);
            yield return Submit(GameManager.CharacterActionMenu);

            Assert.AreEqual(Cursor.State.ChoosingTradeTarget, GameManager.Cursor.CurrentState);
            Assert.True(GameManager.Cursor.IsInFocus());

            yield return Cancel();

            Assert.True(GameManager.CharacterActionMenu.gameObject.activeSelf);
            Assert.True(GameManager.CharacterActionMenu.IsInFocus());
        }

        [UnityTest]
        public IEnumerator OnCancelUnknownState()
        {
            GameManager.Cursor.CurrentState = Cursor.State.Error;
            yield return Cancel(GameManager.Cursor);

            LogAssert.Expect(LogType.Error, "Invalid Cursor.State in OnCancel: Error");
        }
    }
}
