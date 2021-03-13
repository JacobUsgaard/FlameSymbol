using System.Collections;
using Characters;
using Items.Weapons.Assistable.HealingStaffs;
using NUnit.Framework;
using UI;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.UI
{
    public class TradeDetailPanelTests : GameManagerTest
    {
        [UnityTest]
        public IEnumerator OnInformationTest()
        {
            GameManager.TradeDetailPanel.OnInformation();
            LogAssert.Expect(LogType.Log, "TradeDetailPanel.OnInformation is not implemented");
            yield return null;
        }

        /// <summary>
        /// Given a character has 3 items
        /// When the character trades the 3rd item
        /// Then the cursor should be on the 2nd item
        /// </summary>
        [UnityTest]
        public IEnumerator TradeTest1()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            character.Items.Clear();
            character.Items.Add(Heal.Create());
            character.Items.Add(Heal.Create());
            character.Items.Add(Heal.Create());

            Character targetCharacter = GameManager.CurrentLevel.GetCharacter(2, 1);

            GameManager.TradeDetailPanel.Show(character, targetCharacter);

            yield return null;

            Assert.True(GameManager.TradeDetailPanel.IsInFocus());

            yield return DownArrow();
            yield return DownArrow();

            Assert.AreEqual(2, GameManager.TradeDetailPanel.SourceItemsIndex);

            yield return Submit();

            Assert.AreEqual(1, GameManager.TradeDetailPanel.SourceItemsIndex);
        }

        /// <summary>
        /// Given a character has 3 items
        /// When the character trades the 2nd item
        /// Then the cursor should be on the 2nd item
        /// </summary>
        [UnityTest]
        public IEnumerator TradeTest2()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            character.Items.Clear();
            character.Items.Add(Heal.Create());
            character.Items.Add(Heal.Create());
            character.Items.Add(Heal.Create());

            Character targetCharacter = GameManager.CurrentLevel.GetCharacter(2, 1);

            GameManager.TradeDetailPanel.Show(character, targetCharacter);

            yield return null;

            Assert.True(GameManager.TradeDetailPanel.IsInFocus());

            yield return DownArrow();

            Assert.AreEqual(1, GameManager.TradeDetailPanel.SourceItemsIndex);

            yield return Submit();

            Assert.AreEqual(1, GameManager.TradeDetailPanel.SourceItemsIndex);
        }

        [UnityTest]
        public IEnumerator TradeTest3()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(2, 2);
            character.Items.Clear();
            character.Items.Add(Heal.Create());
            character.Items.Add(Heal.Create());
            character.Items.Add(Heal.Create());

            Character targetCharacter = GameManager.CurrentLevel.GetCharacter(2, 1);
            targetCharacter.Items.Clear();
            targetCharacter.Items.Add(Heal.Create());

            GameManager.TradeDetailPanel.Show(character, targetCharacter);

            Assert.True(GameManager.TradeDetailPanel.IsInFocus());

            yield return RightArrow();

            Assert.AreEqual(TradeDetailPanel.Side.DESTINATION, GameManager.TradeDetailPanel.CurrentSide);

            yield return Submit();

            Assert.AreEqual(4, character.Items.Count);
            Assert.AreEqual(TradeDetailPanel.Side.SOURCE, GameManager.TradeDetailPanel.CurrentSide);
        }
    }
}
