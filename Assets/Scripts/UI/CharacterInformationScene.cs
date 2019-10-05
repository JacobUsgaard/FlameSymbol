using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterInformationScene :  FocusableObject {

   public Text CharacterNameText;
    public Text HpDisplayText;
    public Text LevelText;
    public Text ExperienceText;
    public Text MovesText;
    public Text StrengthText;
    public Text MagicText;
    public Text SkillText;
    public Text ResistanceText;
    public Text DefenseText;
    public Text SpeedText;

    public RectTransform ItemListContent;
    public RectTransform ProficiencyListContent;
    public Scrollbar ItemScrollbar;
    public Scrollbar ProficiencyScrollbar;

    public override void OnArrow(float horizontal, float vertical)
    {
        
    }

    public override void OnCancel()
    {
        SceneManager.UnloadSceneAsync("Scenes/CharacterInformation");
        GameManager.Cursor.Focus();
    }

    public override void OnSubmit()
    {
        
    }

    void Awake()
    {
        Debug.Log("Awake");
        Focus();
        Character character = GameManager.CurrentLevel.GetCharacter();
        if(character == null)
        {
            Debug.LogError("Failed to find character");
            return;
        }

        CharacterNameText.text = character.CharacterName;
        HpDisplayText.text = character.CurrentHp.ToString();
        LevelText.text = character.Level.ToString();
        ExperienceText.text = character.Experience.ToString();
        MovesText.text = character.Moves.ToString();
        StrengthText.text = character.Strength.ToString();
        MagicText.text = character.Magic.ToString();
        SkillText.text = character.Skill.ToString();
        ResistanceText.text = character.Resistance.ToString();
        DefenseText.text = character.Defense.ToString();
        SpeedText.text = character.Speed.ToString();

        Debug.Log("Adding items");
        foreach(Item item in character.Items)
        {
            
            Text text = Instantiate(item.Text);
            text.text = string.Format("{0}\t{1}/{2}", item.Text.text, item.UsesRemaining, item.UsesTotal);
            text.alignment = TextAnchor.MiddleLeft;
            text.transform.SetParent(ItemListContent);

            Debug.LogFormat("Added item: {0}", item);
        }
        ItemScrollbar.value = 1;

        Debug.Log("Adding proficiencies");
        foreach(Character.Proficiency proficiency in character.Proficiencies) 
        {
            Text text = Instantiate(GameManager.AxeTextPrefab);
            text.alignment = TextAnchor.MiddleLeft;
            text.text = proficiency.ToString();
            text.transform.SetParent(ProficiencyListContent);

            Debug.LogFormat("Added proficiency: {0}", proficiency);
        }
        ProficiencyScrollbar.value = 1;

    }
}
