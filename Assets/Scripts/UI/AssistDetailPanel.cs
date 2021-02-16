using Characters;
using Logic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AssistDetailPanel : ManagedMonoBehavior
    {

        public Text AttackNameText;
        public Text InfoText;
        public Text DefenseNameText;

        public Text AttackWeaponText;
        public Text WeaponText;
        public Text DefenseWeaponText;

        public Text AttackHpText;
        public Text HpText;
        public Text DefenseHpText;

        public Text AttackHitText;
        public Text HitText;
        public Text DefenseHitText;

        public Text AttackDamageText;
        public Text DamageText;
        public Text DefenseDamageText;

        public Text AttackCriticalText;
        public Text CriticalText;
        public Text DefenseCriticalText;

        public void Show(Character attackCharacter, Character defenseCharacter)
        {
            Debug.LogFormat("{0} assisting {1}", attackCharacter.CharacterName, defenseCharacter.CharacterName);

            AttackNameText.text = attackCharacter.CharacterName;
            DefenseNameText.text = defenseCharacter.CharacterName;

            Character.AssistInformation AssistInformation = attackCharacter.CalculateAssistInformation(defenseCharacter);

            AttackWeaponText.text = AssistInformation.Item.Text.text;
            AttackHpText.text = attackCharacter.CurrentHp.ToString();
            AttackHitText.text = AssistInformation.HitPercentage + "%";
            AttackDamageText.text = AssistInformation.Might.ToString();
            AttackCriticalText.text = "0%";

            DefenseWeaponText.text = "";
            DefenseHpText.text = defenseCharacter.CurrentHp.ToString();
            DefenseHitText.text = "--";
            DefenseDamageText.text = "--";
            DefenseCriticalText.text = "--";

            float x;
            if (defenseCharacter.transform.position.x >= GameManager.CurrentLevel.TerrainMap.GetLength(0) / 2)
            {
                x = defenseCharacter.transform.position.x - 3;
            }
            else
            {
                x = defenseCharacter.transform.position.x + 3;
            }


            transform.position = new Vector2(x, defenseCharacter.transform.position.y);

            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }
    }
}