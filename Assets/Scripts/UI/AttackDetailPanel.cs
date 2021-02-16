using Characters;
using Logic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AttackDetailPanel : ManagedMonoBehavior
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
            AttackNameText.text = attackCharacter.CharacterName;
            DefenseNameText.text = defenseCharacter.CharacterName;

            Character.AttackInformation AttackInformation = attackCharacter.CalculateAttackInformation(defenseCharacter);

            AttackWeaponText.text = AttackInformation.AttackWeapon.Text.text;
            AttackHpText.text = attackCharacter.CurrentHp.ToString();
            AttackHitText.text = AttackInformation.AttackHitPercentage + "%";
            AttackDamageText.text = AttackInformation.AttackDamage.ToString();
            AttackCriticalText.text = AttackInformation.AttackCriticalPercentage + "%";

            DefenseWeaponText.text = AttackInformation.DefenseWeapon == null ? "--" : AttackInformation.DefenseWeapon.Text.text;

            DefenseHpText.text = defenseCharacter.CurrentHp.ToString();
            DefenseHitText.text = AttackInformation.DefenseCanAttack ? AttackInformation.DefenseHitPercentage + "%" : "--";
            DefenseDamageText.text = AttackInformation.DefenseCanAttack ? AttackInformation.DefenseDamage.ToString() : "--";
            DefenseCriticalText.text = AttackInformation.DefenseCanAttack ? AttackInformation.DefenseCriticalPercentage + "%" : "--";

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