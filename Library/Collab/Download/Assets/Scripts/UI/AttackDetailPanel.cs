using UnityEngine;
using UnityEngine.UI;

public class AttackDetailPanel : MonoBehaviour {

    public Transform panel;

    public Text AttackNameText;
    public Text InfoText;
    public Text DefenseNameText;

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
        AttackNameText.text = attackCharacter.name;
        DefenseNameText.text = defenseCharacter.name;

        transform.gameObject.SetActive(true);
    }

    public void Hide()
    {
        transform.gameObject.SetActive(false);
    }
}
