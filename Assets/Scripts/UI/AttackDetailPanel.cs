using UnityEngine;
using UnityEngine.UI;

public class AttackDetailPanel : MonoBehaviour {

    public Transform panel;

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

    public Character.AttackInfo CurrentAttackInfo;

    public void Show(Character attackCharacter, Character defenseCharacter)
    {
        AttackNameText.text = attackCharacter.CharacterName;
        DefenseNameText.text = defenseCharacter.CharacterName;

        CurrentAttackInfo = attackCharacter.CalculateAttackInfo(defenseCharacter);

        AttackWeaponText.text = CurrentAttackInfo.AttackWeapon.Text.text;
        AttackHpText.text = attackCharacter.CurrentHp.ToString();
        AttackHitText.text = CurrentAttackInfo.AttackHitPercentage + "%";
        AttackDamageText.text = CurrentAttackInfo.AttackDamage.ToString();
        AttackCriticalText.text = CurrentAttackInfo.AttackCriticalPercentage + "%";

        DefenseWeaponText.text = CurrentAttackInfo.DefenseWeapon == null ? "" : CurrentAttackInfo.DefenseWeapon.Text.text;
        DefenseHpText.text = defenseCharacter.CurrentHp.ToString();
        DefenseHitText.text = CurrentAttackInfo.DefenseHitPercentage + "%";
        DefenseDamageText.text = CurrentAttackInfo.DefenseDamage.ToString();
        DefenseCriticalText.text = CurrentAttackInfo.DefenseCriticalPercentage + "%";
        
        transform.gameObject.SetActive(true);
    }

    public void Hide()
    {
        CurrentAttackInfo = null;
        transform.gameObject.SetActive(false);
    }
}
