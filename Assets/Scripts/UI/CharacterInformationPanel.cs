using Characters;
using Logic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CharacterInformationPanel : ManagedMonoBehavior
    {
        public Text CharacterNameText;
        public Text HpDisplayText;
        public Text ExperienceText;
        public Text LevelText;

        public void Show(Character character)
        {
            CharacterNameText.text = character.CharacterName;
            HpDisplayText.text = "HP: " + character.CurrentHp + " / " + character.MaxHp;
            ExperienceText.text = "Exp: " + character.Experience + " / 100";
            LevelText.text = "Lvl: " + character.Level;
            transform.position = new Vector2(character.transform.position.x, character.transform.position.y - 1);

            if (!transform.gameObject.activeSelf)
            {
                transform.gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if (transform.gameObject.activeSelf)
            {
                transform.gameObject.SetActive(false);
            }
        }
    }
}