using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInformationPanel : MonoBehaviour {
    public Transform panel;

    public Text CharacterNameText;
    public Text HpDisplayText;
    public Text ExperienceText;
    public Text StrengthText;
    public Text MagicText;
    public Text SkillText;
    public Text ResistanceText;
    public Text DefenseText;
    public Text SpeedText;
    public Text MovesText;
    public Text LevelText;

    public readonly List<Text> WeaponTexts = new List<Text>();

    public void Show(Character character)
    {
        CharacterNameText.text = character.CharacterName;
        HpDisplayText.text = "HP: " + character.CurrentHp + " / " + character.MaxHp;
        ExperienceText.text = "Exp: " + character.Experience;
        StrengthText.text = "Str: " + character.Strength;
        MagicText.text = "Mag: " + character.Magic;
        SkillText.text = "Skill: " + character.Skill;
        ResistanceText.text = "Res: " + character.Resistance;
        DefenseText.text = "Def: " + character.Defense;
        SpeedText.text = "Spd: " + character.Speed;
        MovesText.text = "Moves: " + character.Moves;
        LevelText.text = "Level: " + character.Level;

        foreach(Text text in WeaponTexts)
        {
            Destroy(text.gameObject);
        }
        WeaponTexts.Clear();

        foreach (Item item in character.Items)
        {
            WeaponTexts.Add(Instantiate(item.Text, panel));
        }
        // TODO proficiencies

        if (!panel.gameObject.activeSelf)
        {
            panel.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        if (panel.gameObject.activeSelf)
        {
            panel.gameObject.SetActive(false);
        }
    }
}