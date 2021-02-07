using System.Collections;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace Tests.UI
{
    public class AttackDetailPanelTests : GameManagerTest
    {
        /// <summary>
        /// Validate that the AssistDetailPanel shows up on the left when the
        /// characters are on the right side of the screen
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator ValidateLeftPlacement()
        {
            Character sourceCharacter = GameManager.CurrentLevel.GetCharacter(2, 2);
            sourceCharacter.Items.Clear();
            sourceCharacter.Items.Add(IronSword.Create());
            sourceCharacter.AddProficiency(new Proficiency(typeof(Sword), Proficiency.Rank.A));

            Character targetCharacter = GameManager.CurrentLevel.GetCharacter(2, 1);

            GameManager.AttackDetailPanel.Show(sourceCharacter, targetCharacter);

            Assert.True(GameManager.AttackDetailPanel.gameObject.activeSelf);
            yield return null;
        }

        /// <summary>
        /// Validate that the AssistDetailPanel shows up on the right when the
        /// characters are on the left side of the screen
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator ValidateRightPlacement()
        {
            Character sourceCharacter = GameManager.CurrentLevel.GetCharacter(0, 0);
            sourceCharacter.Items.Clear();
            sourceCharacter.Items.Add(IronSword.Create());
            sourceCharacter.AddProficiency(new Proficiency(typeof(Sword), Proficiency.Rank.A));

            Character targetCharacter = GameManager.CurrentLevel.GetCharacter(0, 1);

            GameManager.AttackDetailPanel.Show(sourceCharacter, targetCharacter);

            Assert.True(GameManager.AttackDetailPanel.gameObject.activeSelf);
            yield return null;
        }
    }
}
