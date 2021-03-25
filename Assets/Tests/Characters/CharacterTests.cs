using System.Collections;
using System.Collections.Generic;
using Characters;
using Items;
using Items.Weapons;
using Items.Weapons.Assistable.HealingStaffs;
using Items.Weapons.Attackable;
using Items.Weapons.Attackable.Magic.FireMagic;
using Items.Weapons.Attackable.Strength.Lance;
using Items.Weapons.Attackable.Strength.Sword;
using Logic;
using NUnit.Framework;
using UI;
using UnityEngine;
using UnityEngine.TestTools;
using Cursor = UI.Cursor;

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
            Character character = GameManager.CurrentLevel.GetCharacter(1, 2);

            yield return MoveCursor(2, 2);

            // Select Character
            yield return Submit();

            // Select Move
            yield return Submit();

            // Attack is in the list of actions
            Assert.True(GameManager.CharacterActionMenu.MenuItems.Exists((Menu.MenuItem<Item> obj) => { return obj.DisplayText.text.Contains("Attack"); })); ; ;

            // Select Attack
            yield return Submit();

            // Character has one option for weapons
            Assert.AreEqual(1, GameManager.ItemSelectionMenu.MenuItems.Count);

            // Select Weapon
            yield return Submit();

            // Character has four spaces with Characters
            Assert.AreEqual(4, GameManager.Cursor.AttackableSpacesWithCharacters.Count);

            // Select Character to attack
            yield return Submit();

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

            Character defendingCharacter = GameManager.CurrentLevel.GetCharacter(1, 2);
            defendingCharacter.CurrentHp = 1;

            yield return MoveCursor(2, 2);

            yield return Submit();
            yield return Submit();

            yield return Submit();

            yield return Submit();

            yield return Submit();

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

            yield return Submit();

            yield return Submit();

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
            yield return Submit();

            // select move
            yield return Submit();

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
            Heal heal = Heal.Create();
            _ = heal.Ranges.Add(3);
            character.Items.Add(heal);

            yield return MoveCursor(2, 2);

            Assert.AreEqual(8, character.AttackableTransforms.Count);
            Assert.AreEqual(1, character.AssistableTransforms.Count);
        }

        /// <summary>
        /// Initial test to make sure everything shows up correctly
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TradingTest1()
        {
            Character sourceCharacter = GameManager.CurrentLevel.GetCharacter(2, 2);
            Character targetCharacter = GameManager.CurrentLevel.GetCharacter(2, 1);
            sourceCharacter.Items.Clear();

            // Move cursor
            yield return MoveCursor(2, 2);

            // select character
            yield return Submit();

            // select move
            yield return Submit();
            Assert.True(GameManager.CharacterActionMenu.IsInFocus());

            // select trade
            GameManager.CharacterActionMenu.OnSubmit();
            Assert.AreEqual(Cursor.State.ChoosingTradeTarget, GameManager.Cursor.CurrentState);

            // select trading character
            yield return Submit();
            Assert.True(GameManager.TradeDetailPanel.IsInFocus());

            Assert.AreEqual(sourceCharacter.CharacterName, GameManager.TradeDetailPanel.SourceText.text);
            Assert.AreEqual(targetCharacter.CharacterName, GameManager.TradeDetailPanel.DestinationText.text);

            sourceCharacter.Items.ForEach(
                sourceCharacterItem =>
                {
                    Debug.LogFormat("Item: {0}", sourceCharacterItem.Text.text);
                    Assert.True(
                        GameManager.TradeDetailPanel.TradeSourceMenuItems.Exists(sourceTradeItem => sourceCharacterItem.name.Equals(sourceTradeItem.Text.text)));
                }
            );
        }

        /// <summary>
        /// Test trading items
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TradingTest2()
        {
            Character sourceCharacter = GameManager.CurrentLevel.GetCharacter(2, 2);
            sourceCharacter.Items.Clear();
            sourceCharacter.Items.Add(IronSword.Create());
            sourceCharacter.Items.Add(Fire.Create());

            Character targetCharacter = GameManager.CurrentLevel.GetCharacter(2, 1);
            targetCharacter.Items.Clear();
            targetCharacter.Items.Add(Fire.Create());

            // Move cursor
            yield return MoveCursor(2, 2);

            // select character
            yield return Submit();

            // select move
            yield return Submit();

            // select trade
            yield return DownArrow();
            yield return Submit();

            // select trading character
            yield return Submit();

            yield return Submit();
            Assert.AreEqual(1, sourceCharacter.Items.Count);
            Assert.AreEqual(2, targetCharacter.Items.Count);

            yield return DownArrow();
            Assert.AreEqual(TradeDetailPanel.Side.SOURCE, GameManager.TradeDetailPanel.CurrentSide);

            // Press right
            yield return RightArrow();
            Assert.AreEqual(TradeDetailPanel.Side.DESTINATION, GameManager.TradeDetailPanel.CurrentSide);

            yield return DownArrow();
            Assert.AreEqual(TradeDetailPanel.Side.DESTINATION, GameManager.TradeDetailPanel.CurrentSide);

            yield return DownArrow();
            Assert.AreEqual(TradeDetailPanel.Side.DESTINATION, GameManager.TradeDetailPanel.CurrentSide);

            yield return LeftArrow();
            Assert.AreEqual(TradeDetailPanel.Side.SOURCE, GameManager.TradeDetailPanel.CurrentSide);

            yield return LeftArrow();
            Assert.AreEqual(TradeDetailPanel.Side.SOURCE, GameManager.TradeDetailPanel.CurrentSide);

            yield return RightArrow();
            Assert.AreEqual(TradeDetailPanel.Side.DESTINATION, GameManager.TradeDetailPanel.CurrentSide);

            yield return Submit();
            Assert.AreEqual(2, sourceCharacter.Items.Count);
            Assert.AreEqual(1, targetCharacter.Items.Count);
        }

        /// <summary>
        /// Test canceling the trade
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TradingTest3()
        {
            Character sourceCharacter = GameManager.CurrentLevel.GetCharacter(2, 2);
            sourceCharacter.Items.Clear();
            sourceCharacter.Items.Add(IronSword.Create());
            sourceCharacter.Items.Add(Fire.Create());

            Character targetCharacter = GameManager.CurrentLevel.GetCharacter(2, 1);
            targetCharacter.Items.Clear();
            targetCharacter.Items.Add(Fire.Create());

            // Move cursor
            yield return MoveCursor(2, 2);

            // select character
            yield return Submit();

            // select move
            yield return Submit();

            // select trade
            yield return DownArrow();
            yield return Submit();

            // select trading character
            yield return Submit();

            yield return Cancel();
            Assert.False(GameManager.TradeDetailPanel.IsInFocus());
            Assert.True(GameManager.Cursor.IsInFocus());
            Assert.AreEqual(Cursor.State.ChoosingTradeTarget, GameManager.Cursor.CurrentState);
        }

        /// <summary>
        /// Test calling trade when no items are tradable
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TradingTest4()
        {
            Character sourceCharacter = GameManager.CurrentLevel.GetCharacter(2, 2);
            sourceCharacter.Items.Clear();

            Character targetCharacter = GameManager.CurrentLevel.GetCharacter(2, 1);
            targetCharacter.Items.Clear();

            GameManager.TradeDetailPanel.Show(sourceCharacter, targetCharacter);

            LogAssert.Expect(LogType.Error, "Neither character has items to trade.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator CalculateAssistablePositionsTest1()
        {
            HashSet<int> ranges = new HashSet<int>();
            ranges.Add(1);
            ranges.Add(2);

            Character character = GameManager.CurrentLevel.GetCharacter(0, 0);

            HashSet<Vector2> assistablePositions = character.CalculateAssistablePositions(0, 0, ranges);
            Assert.AreEqual(6, assistablePositions.Count);

            yield return null;
        }

        [UnityTest]
        public IEnumerator AddExperienceTest1()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(0, 0);
            int currentLevel = character.Level;
            character.AddExperience(100);
            Assert.AreEqual(currentLevel + 1, character.Level);

            yield return null;
        }

        [UnityTest]
        public IEnumerator AddExperienceTest2()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(0, 0);
            character.AddExperience(101);

            LogAssert.Expect(LogType.Error, "Experience cannot be greater than 100: 101");

            yield return null;
        }

        /// <summary>
        /// Test to make sure characters don't move on top of each other
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator MoveTest1()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(0, 0);

            character.Move(new Vector2(0, 1));
            LogAssert.Expect(LogType.Error, "Position is already taken: (0.0, 1.0)");
            yield return null;
        }

        /// <summary>
        /// Test to make sure character move actually works
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator MoveTest2()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(0, 0);

            character.Move(new Vector2(1, 0));

            Assert.AreEqual(character, GameManager.CurrentLevel.GetCharacter(1, 0));
            yield return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator CalculateMovementCostTest1()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(0, 0);

            Assert.AreEqual(int.MaxValue, character.CalculateMovementCost(new Vector2(-1, 0)));
            yield return null;
        }

        /// <summary>
        /// Equiping an item that the character does not have in their inventory
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator EquipTest1()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(0, 0);
            Item item = IronSword.Create();

            character.Equip(item);
            LogAssert.Expect(LogType.Error, "Iron Sword does not exist in inventory.");

            yield return null;
        }

        /// <summary>
        /// Enemy attacks back
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator AttackTest2()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);

            GameManager.CurrentLevel.SetCharacter(character, 1, 0);

            character.CompleteAttack(GameManager.CurrentLevel.GetCharacter(0, 0));

            yield return null;
        }

        /// <summary>
        /// Defending character kills attacking character
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator AttackTest3()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            character.CurrentHp = 1;

            GameManager.CurrentLevel.SetCharacter(character, 1, 0);

            character.CompleteAttack(GameManager.CurrentLevel.GetCharacter(0, 0));

            yield return null;
        }

        /// <summary>
        /// Calculate damage with unknown weapon type
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator CalculateDamageTest1()
        {
            Character attackCharacter = GameManager.CurrentLevel.GetCharacter(2, 2);
            Character defenseCharacter = GameManager.CurrentLevel.GetCharacter(2, 1);

            Weapon weapon = ScriptableObject.CreateInstance<Items.Weapons.WeaponTests.FakeWeapon>();

            _ = attackCharacter.CalculateDamage(weapon, defenseCharacter, null);
            LogAssert.Expect(LogType.Error, "Unknown weapon type: FakeWeapon");
            yield return null;
        }

        /// <summary>
        /// Testing when Character does not have a usable item
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator GetUsableItemTest1()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            character.Proficiencies.Clear();

            Assert.IsNull(character.GetUsableItem<Attackable>());
            yield return null;
        }

        /// <summary>
        /// Checking to make sure when character has an item's proficiency but is not proficient enough
        /// then IsProficient returns false.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator IsProficientTest1()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            character.Proficiencies.Clear();
            character.AddProficiency(new Proficiency(typeof(Sword), Proficiency.Rank.A));

            character.Items.Clear();
            IronLance ironLance = IronLance.Create();
            ironLance.RequiredProficiencyRank = Proficiency.Rank.S;
            character.Items.Add(ironLance);

            Assert.False(character.IsProficient(ironLance));
            yield return null;
        }

        [UnityTest]
        public IEnumerator CalculateNumberOfAttacksTest1()
        {
            Character attackCharacter = GameManager.CurrentLevel.GetCharacter(2, 2);
            attackCharacter.Speed = 30;

            Character defenseCharacter = GameManager.CurrentLevel.GetCharacter(1, 2);

            int numberOfAttacks = attackCharacter.CalculateNumberOfAttacks(attackCharacter.GetUsableItem<Weapon>(), defenseCharacter, null);
            Assert.AreEqual(2, numberOfAttacks);

            yield return null;
        }

        /// <summary>
        /// When a character waits, they shouldn't be able to move again
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator EndActionTest1()
        {
            yield return MoveCursor(2, 2);
            yield return Submit();
            yield return DownArrow();
            yield return RightArrow();
            yield return Submit();
            yield return UpArrow();
            yield return Submit();

            Assert.AreEqual(Cursor.State.Free, GameManager.Cursor.CurrentState);
            Assert.True(GameManager.CurrentLevel.GetCharacter(3, 1).HasMoved);
        }

        /// <summary>
        /// When a character attacks, they shouldn't be able to move again
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator EndActionTest2()
        {
            yield return MoveCursor(2, 2);
            yield return Submit();
            yield return Submit();
            yield return Submit();
            yield return Submit();
            yield return Submit();

            Assert.AreEqual(Cursor.State.Free, GameManager.Cursor.CurrentState);
            Assert.True(GameManager.CurrentLevel.GetCharacter(2, 2).HasMoved);
        }

        /// <summary>
        /// When a character assists, they shouldn't be able to move again
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator EndActionTest3()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            character.Items.Clear();
            character.Items.Add(Heal.Create());
            character.AddProficiency(new Proficiency(typeof(HealingStaff), Proficiency.Rank.A));

            yield return MoveCursor(2, 2);
            yield return Submit();
            yield return Submit();
            yield return Submit();
            yield return Submit();
            yield return Submit();

            Assert.AreEqual(Cursor.State.Free, GameManager.Cursor.CurrentState);
            Assert.True(GameManager.CurrentLevel.GetCharacter(2, 2).HasMoved);
        }

        /// <summary>
        /// When a character trades, they shouldn't be able to move again
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator EndActionTest4()
        {
            Character sourceCharacter = GameManager.CurrentLevel.GetCharacter(2, 2);
            sourceCharacter.Items.Clear();
            sourceCharacter.Items.Add(IronSword.Create());
            sourceCharacter.Items.Add(Fire.Create());

            Character targetCharacter = GameManager.CurrentLevel.GetCharacter(2, 1);
            targetCharacter.Items.Clear();
            targetCharacter.Items.Add(Fire.Create());

            // Move cursor
            yield return MoveCursor(2, 2);

            // select character
            yield return Submit();

            // select move
            yield return Submit();

            // select trade
            yield return DownArrow();
            yield return Submit();

            // select trading character
            yield return Submit();

            // trade item
            yield return Submit();

            Assert.AreEqual(1, sourceCharacter.Items.Count);
            Assert.AreEqual(2, targetCharacter.Items.Count);

            //Assert.AreEqual(Cursor.State.Free, GameManager.Cursor.CurrentState);
            Assert.True(GameManager.CurrentLevel.GetCharacter(2, 2).HasMoved);
            Assert.True(GameManager.CurrentLevel.GetCharacter(2, 2).HasTraded);
            Assert.False(GameManager.CurrentLevel.GetCharacter(2, 1).HasMoved);
            Assert.False(GameManager.CurrentLevel.GetCharacter(2, 1).HasTraded);
        }

        /// <summary>
        /// Given a character has moved
        /// And Cursor.CurrentState is Free
        /// When Submit is pressed on character
        /// Then the player action menu should appear
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator EndActionTest5()
        {
            // Move cursor
            yield return MoveCursor(2, 2);

            // select character
            yield return Submit();

            // select move
            yield return Submit();

            // select Wait
            yield return UpArrow();
            yield return Submit();

            Assert.True(GameManager.CurrentLevel.GetCharacter(2, 2).HasMoved);

            yield return Submit();

            Assert.True(GameManager.PlayerActionMenu.IsInFocus());
        }

        /// <summary>
        /// Given a character has traded
        /// And the character action menu is in focus
        /// When the player presses Cancel 
        /// Then the character should not be able to move again
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator EndActionTest6()
        {
            Character sourceCharacter = GameManager.CurrentLevel.GetCharacter(2, 2);
            sourceCharacter.Items.Clear();
            sourceCharacter.Items.Add(IronSword.Create());
            sourceCharacter.Items.Add(Fire.Create());

            Character targetCharacter = GameManager.CurrentLevel.GetCharacter(2, 1);
            targetCharacter.Items.Clear();
            targetCharacter.Items.Add(Fire.Create());

            // Move cursor
            yield return MoveCursor(2, 2);

            // select character
            yield return Submit();

            // select move
            yield return Submit();

            // select trade
            yield return DownArrow();
            yield return Submit();

            // select trading character
            yield return Submit();

            // trade item
            yield return Submit();

            Assert.AreEqual(1, sourceCharacter.Items.Count);
            Assert.AreEqual(2, targetCharacter.Items.Count);

            yield return Cancel();
            yield return Cancel();
        }

        /// <summary>
        /// Given a character has selected a move
        /// When the player presses Cancel 
        /// Then the character should be able to move again
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator EndActionTest7()
        {
            yield return MoveCursor(2, 2);
            yield return Submit();

            yield return DownArrow();
            yield return RightArrow();
            yield return Submit();

            yield return Cancel();
            yield return Cancel();

            Assert.AreEqual(Cursor.State.Free, GameManager.Cursor.CurrentState);
            Assert.False(GameManager.CurrentLevel.GetCharacter(2, 2).HasMoved);
        }

        [UnityTest]
        public IEnumerator DoubleAttackTest1()
        {
            Character attackCharacter = GameManager.CurrentLevel.GetCharacter(2, 2);
            attackCharacter.Speed = 30;

            Character defenseCharacter = GameManager.CurrentLevel.GetCharacter(1, 2);

            attackCharacter.CompleteAttack(defenseCharacter);

            Assert.AreEqual(0, defenseCharacter.CurrentHp);

            yield return null;
        }

        [UnityTest]
        public IEnumerator CriticalAttackTest1()
        {
            Character attackCharacter = GameManager.CurrentLevel.GetCharacter(2, 2);
            attackCharacter.Skill = 10000;

            Character defenseCharacter = GameManager.CurrentLevel.GetCharacter(1, 2);

            attackCharacter.CompleteAttack(defenseCharacter);

            LogAssert.Expect(LogType.Log, "Critical attack");
            yield return null;
        }

        /// <summary>
        /// Given a character has moved
        /// And Cursor.CurrentState is Free
        /// When Cursor is over character
        /// Then movable and attackable transforms should not show
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator CharacterHasMovedTest()
        {
            // Move cursor
            yield return MoveCursor(2, 2);

            // select character
            yield return Submit();

            // select move
            yield return Submit();

            // select Wait
            yield return UpArrow();
            yield return Submit();

            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);

            Assert.True(character.HasMoved);

            Assert.IsEmpty(character.AttackableTransforms);
            Assert.IsEmpty(character.MovableTransforms);
        }

        [UnityTest]
        public IEnumerator TerrainMovementTest1()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(0, 0);
            character.Moves = 2;


            yield return MoveCursor(0, 0);

            Assert.True(character.MovableTransforms.Exists(c => c.position.x == 2 && c.position.y == 0));

            GameManager.CurrentLevel.SetTerrain(GameManager.ForrestTerrain, 2, 0);

            yield return MoveCursor(1, 0);
            yield return MoveCursor(0, 0);

            Assert.False(character.MovableTransforms.Exists(c => c.position.x == 2 && c.position.y == 0));
        }

        [UnityTest]
        public IEnumerator TerrainDefenseTest1()
        {
            Character attacker = GameManager.CurrentLevel.GetCharacter(2, 2);
            Character defender = GameManager.CurrentLevel.GetCharacter(1, 2);

            int before = attacker.CalculateDamage(attacker.GetUsableItem<Weapon>(), defender, null);

            GameManager.CurrentLevel.SetTerrain(GameManager.ForrestTerrain, 1, 2);
            yield return null;

            int after = attacker.CalculateDamage(attacker.GetUsableItem<Weapon>(), defender, null);

            Assert.Greater(before, after);
        }

        [UnityTest]
        public IEnumerator TerrainHitTest1()
        {
            Character attacker = GameManager.CurrentLevel.GetCharacter(2, 2);
            Character defender = GameManager.CurrentLevel.GetCharacter(1, 2);

            int before = attacker.CalculateHitPercentage(attacker.GetUsableItem<Weapon>(), defender, null);

            GameManager.CurrentLevel.SetTerrain(GameManager.ForrestTerrain, 1, 2);
            yield return null;

            int after = attacker.CalculateHitPercentage(attacker.GetUsableItem<Weapon>(), defender, null);

            Assert.Greater(before, after);
        }
    }
}
