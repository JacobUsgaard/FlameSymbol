using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Characters
{
    public class CharacterTests : GameManagerTest
    {

        /// <summary>
        /// Checking that everything occurs correctly when attacking.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator AttackTest1()
        {
            yield return MoveCursor(2, 2);

            yield return Enter();

            yield return Enter();

            Assert.True(GameManager.CharacterActionMenu.MenuItems.Exists((Menu.MenuItem<Item> obj) => { return obj.DisplayText.text.Contains("Attack"); })); ; ;

            Assert.AreEqual(4, GameManager.Cursor.AttackableSpacesWithCharacters.Count);

            GameManager.CharacterActionMenu.OnSubmit();
            yield return null;

            Assert.AreEqual(1, GameManager.ItemSelectionMenu.MenuItems.Count);

            GameManager.ItemSelectionMenu.OnSubmit();
            yield return null;

            yield return Enter();

            Character character = GameManager.CurrentLevel.GetCharacter(1, 2);
            Assert.AreNotEqual(character.MaxHp, character.CurrentHp);
        }

        /// <summary>
        /// Testing to make sure everything is cleaned up after character death
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator DieTest1()
        {
            ICollection<Character> beforeCharacters = GameManager.CurrentLevel.GetCharacters();

            yield return MoveCursor(2, 2);

            yield return Enter();

            yield return Enter();

            GameManager.CharacterActionMenu.OnSubmit();
            yield return null;

            GameManager.ItemSelectionMenu.OnSubmit();
            yield return null;

            yield return Enter();

            yield return MoveCursor(2, 2);

            yield return Enter();
            yield return Enter();

            GameManager.CharacterActionMenu.OnSubmit();
            yield return null;

            GameManager.ItemSelectionMenu.OnSubmit();
            yield return null;

            yield return Enter();

            Assert.IsNull(GameManager.CurrentLevel.GetCharacter(1, 2));

            foreach (Character character in GameManager.CurrentLevel.GetCharacters())
            {
                Assert.IsEmpty(character.MovableTransforms);
                Assert.IsEmpty(character.MovableTransforms);
            }

            ICollection<Character> afterCharacters = GameManager.CurrentLevel.GetCharacters();

            Assert.AreEqual(afterCharacters.Count, beforeCharacters.Count - 1);
        }

        /// <summary>
        /// When the cursor is over a player-owned character
        /// and the character has a staff
        /// and the cursor is in stage choosing move
        /// Then the character should have staffable transforms
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator AssistableTransformsTest1()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);

            character.Items.Clear();
            character.Items.Add(Heal.Create());
            character.AddProficiency(new Proficiency(typeof(HealingStaff), rank: Proficiency.Rank.E));

            yield return MoveCursor(2, 2);

            Assert.IsNotEmpty(character.AssistableTransforms);
            Assert.AreEqual(5, character.AssistableTransforms.Count);

            AssertHelper.Contains(new Vector3(0, 0), character.AssistableTransforms);
            AssertHelper.Contains(new Vector3(0, 1), character.AssistableTransforms);
            AssertHelper.Contains(new Vector3(1, 2), character.AssistableTransforms);
            AssertHelper.Contains(new Vector3(2, 3), character.AssistableTransforms);
            AssertHelper.Contains(new Vector3(3, 2), character.AssistableTransforms);
        }

        /// <summary>
        /// When the character has a staff
        /// and there is a staffable character within range
        /// then the 'Assist' option should show up in the character action menu
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator HealingTest1()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);

            character.Items.Clear();
            character.Items.Add(Heal.Create());
            character.AddProficiency(new Proficiency(typeof(HealingStaff), rank: Proficiency.Rank.E));

            yield return MoveCursor(2, 2);

            yield return Enter();

            yield return Enter();

            Assert.IsNotEmpty(GameManager.CharacterActionMenu.MenuItems);

            Assert.True(GameManager.CharacterActionMenu.MenuItems.Exists((Menu.MenuItem<Item> obj) => { return obj.DisplayText.text.Contains("Assist"); })); ; ;
        }

        /// <summary>
        /// Checking to make sure heal works
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator HealingTest2()
        {
            Character hurtCharacter = GameManager.CurrentLevel.GetCharacter(2, 1);
            hurtCharacter.CurrentHp -= 2;

            Character healingCharacter = GameManager.CurrentLevel.GetCharacter(2, 2);
            healingCharacter.Items.Clear();
            healingCharacter.Items.Add(Heal.Create());
            healingCharacter.AddProficiency(new Proficiency(typeof(HealingStaff), rank: Proficiency.Rank.E));

            yield return MoveCursor(2, 2);

            // select character
            yield return Enter();

            // select move
            yield return Enter();

            // select Assist
            GameManager.CharacterActionMenu.OnSubmit();

            // select staff
            GameManager.ItemSelectionMenu.OnSubmit();

            //select other character
            GameManager.Cursor.OnSubmit();

            Assert.False(GameManager.ItemSelectionMenu.IsInFocus());
            Assert.IsEmpty(healingCharacter.AssistableTransforms);
            Assert.IsEmpty(healingCharacter.MovableTransforms);
            Assert.IsEmpty(healingCharacter.AttackableTransforms);
            Assert.AreEqual(hurtCharacter.CurrentHp, hurtCharacter.MaxHp);
        }

        /// <summary>
        /// Making sure both attackable and assistable positions show up.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator HealingTest3()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);

            yield return MoveCursor(2, 2);

            Assert.AreEqual(8, character.AttackableTransforms.Count);
            Assert.AreEqual(1, character.AssistableTransforms.Count);
        }
    }
}
