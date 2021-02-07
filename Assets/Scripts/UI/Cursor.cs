using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UI;

public class Cursor : FocusableObject
{
    public static readonly float cursorSpeed = 10f;

    public int AttackableSpacesWithCharactersIndex { get; private set; } = 0;
    public List<Transform> AttackableSpacesWithCharacters { get; } = new List<Transform>();

    public int TradableSpacesWithCharactersIndex { get; private set; } = 0;
    public List<Transform> TradableSpacesWithCharacters { get; } = new List<Transform>();

    public int AssistableTransformsWithCharactersIndex { get; private set; } = 0;
    public List<Transform> AssistableTransformsWithCharacters { get; private set; } = new List<Transform>();

    public Vector2 SelectedCharacterOldPosition { get; private set; }
    public Character SelectedCharacter { get; private set; }

    public AttackableRange AttackableRange { get; private set; }

    public Path Path;

    public enum State
    {
        ChoosingAttackTarget,
        ChoosingMove,
        ChoosingAssistTarget,
        ChoosingTradeTarget,
        Free,
        Error
    };

    private State currentState;

    public State CurrentState
    {
        get
        {
            return currentState;
        }

        set
        {
            Debug.LogFormat("Current state: {0}", value);
            currentState = value;

            switch (currentState)
            {
                case State.Free:
                    UpdateInformationPanels();
                    ShowMovableAndAttackablePositions(transform.position);
                    break;

            }
        }
    }

    public void Start()
    {
        AttackableRange = ScriptableObject.CreateInstance<AttackableRange>();
        Path = ScriptableObject.CreateInstance<Path>();
    }

    private void HideInformationPanels()
    {
        GameManager.CharacterInformationPanel.Hide();
        GameManager.TerrainInformationPanel.Hide();
    }

    /// <summary>
    /// Update the information panels (e.g. character/terrain) based on the current position.
    /// </summary>
    private void UpdateInformationPanels()
    {

        Character character = GameManager.CurrentLevel.GetCharacter(transform.position);
        if (character != null)
        {
            GameManager.CharacterInformationPanel.Show(character);
        }
        else
        {
            GameManager.CharacterInformationPanel.Hide();
        }
        Terrain.Terrain terrain = GameManager.CurrentLevel.GetTerrain(transform.position);
        GameManager.TerrainInformationPanel.Show(terrain);
    }

    /// <summary>
    /// Callback when the user cancels from the CharacterActionMenu.
    /// </summary>
    /// <param name="character"></param>
    public void CharacterActionMenuOnCancel(params Object[] objects)
    {
        SelectedCharacter.DestroyMovableAndAttackableAndAssistableTransforms();
        DestroyTradableSpaces();
        SelectedCharacter.Move(SelectedCharacterOldPosition);
        SelectedCharacter.CreateAttackableTransforms();
        GameManager.CharacterActionMenu.Hide();
        Focus();
        CurrentState = State.ChoosingMove;
        Path.Show();
    }

    private void CharacterActionMenuAttack(Object[] objects)
    {
        Debug.Log("OnEnterCharacterActionMenuAttack");

        GameManager.ItemSelectionMenu.Clear();
        foreach (Item item in SelectedCharacter.GetUsableItems<Attackable>())
        {
            GameManager.ItemSelectionMenu.AddMenuItem(item, item.Text, ItemSelectionMenuOnSelectAttackItem);
        }

        GameManager.ItemSelectionMenu.Focus();
        GameManager.CharacterActionMenu.Hide();
        GameManager.ItemSelectionMenu.Show(ItemSelectionMenuOnCancel);
    }

    private void CharacterActionMenuAssist(Object[] objects = default)
    {
        Debug.Log("OnEnterCharacterActionMenuAssist");

        GameManager.ItemSelectionMenu.Clear();
        foreach (Item item in SelectedCharacter.GetUsableItems<Assistable>())
        {
            GameManager.ItemSelectionMenu.AddMenuItem(item, item.Text, ItemSelectionMenuOnSelectSupportItem);
        }

        GameManager.ItemSelectionMenu.Focus();
        GameManager.CharacterActionMenu.Hide();
        GameManager.ItemSelectionMenu.Show(ItemSelectionMenuOnCancel);
    }

    private void CharacterActionMenuTrade(Object[] objects)
    {
        Debug.Log("OnEnterCharacterActionMenuTrade");

        TradableSpacesWithCharacters.Clear();
        TradableSpacesWithCharacters.AddRange(SelectedCharacter.CreateTradableTransforms(SelectedCharacter.CalculateTradablePositionsWithCharacters()));
        GameManager.CharacterActionMenu.Hide();
        CurrentState = State.ChoosingTradeTarget;
        SetTradableSpaceWithCharacter();
        Focus();
    }

    /// <summary>
    /// Callback when the user selects the wait option from the CharacterActionMenu.
    /// </summary>
    /// <param name="character"></param>
    private void CharacterActionMenuWait(Object[] objects)
    {
        SelectedCharacter.DestroyMovableAndAttackableAndAssistableTransforms();
        DestroyTradableSpaces();
        GameManager.CharacterActionMenu.Hide();
        CurrentState = State.Free;
        Focus();
    }

    private void ItemSelectionMenuOnSelectSupportItem(Object[] objects)
    {
        Item item = GameManager.ItemSelectionMenu.MenuItems[GameManager.ItemSelectionMenu.CurrentMenuItemIndex].ItemObject;
        SelectedCharacter.Equip(item);

        AssistableTransformsWithCharacters.Clear();
        HashSet<Vector2> assistablePositions = SelectedCharacter.CalculateAssistablePositions(transform.position.x, transform.position.y, ((Assistable)item).Ranges);

        AssistableTransformsWithCharacters
            .AddRange(
                SelectedCharacter.CreateAssistableTransforms(
                    SelectedCharacter.ExtractAssistablePositionsWithCharacters(assistablePositions)));


        CurrentState = State.ChoosingAssistTarget;
        GameManager.ItemSelectionMenu.Hide();
        SetAssistableTransformWithCharacter();
        Focus();
    }

    private void ItemSelectionMenuOnSelectAttackItem(Object[] objects)
    {
        Item item = GameManager.ItemSelectionMenu.MenuItems[GameManager.ItemSelectionMenu.CurrentMenuItemIndex].ItemObject;
        Debug.LogFormat("Selected a weapon: {0}", item.Text.text);
        SelectedCharacter.Equip(item);

        AttackableSpacesWithCharacters.Clear();
        HashSet<Vector2> attackablePositions = SelectedCharacter.CalculateAttackablePositions(transform.position.x, transform.position.y, ((Attackable)item).Ranges);

        AttackableSpacesWithCharacters
            .AddRange(SelectedCharacter
                .CreateAttackableTransforms(SelectedCharacter
                    .ExtractAttackablePositionsWithCharacters(attackablePositions)));

        CurrentState = State.ChoosingAttackTarget;
        GameManager.ItemSelectionMenu.Hide();
        SetAttackableSpaceWithCharacter();
        Focus();
    }

    private void ItemSelectionMenuOnCancel(Object[] objects)
    {
        GameManager.ItemSelectionMenu.Hide();
        ShowCharacterActionMenu(SelectedCharacter);
    }

    private void CharacterActionMenuItems(Object[] objects)
    {
        GameManager.ItemDetailMenu.Clear();
        foreach (Item item in SelectedCharacter.Items)
        {
            GameManager.ItemDetailMenu.AddMenuItem(item, item.Text, ItemDetailMenuOnSelect);
        }

        SelectedCharacter.DestroyAttackableTransforms();
        DestroyTradableSpaces();

        GameManager.CharacterActionMenu.Hide();
        GameManager.ItemDetailMenu.Show(ItemDetailMenuOnCancel);
    }

    private void ItemDetailMenuOnCancel(Object[] objects)
    {
        GameManager.ItemDetailMenu.Hide();
        ShowCharacterActionMenu(SelectedCharacter);
    }

    private void ItemDetailMenuOnSelect(Object[] objects)
    {
        Item item = (Item)objects[0];

        GameManager.ItemActionMenu.AddMenuItem(item, GameManager.EquipTextPrefab, ItemActionMenuEquip);
        GameManager.ItemActionMenu.AddMenuItem(item, GameManager.UseTextPrefab, ItemActionMenuUse);
        GameManager.ItemActionMenu.AddMenuItem(item, GameManager.DropTextPrefab, ItemActionMenuDrop);

        GameManager.ItemDetailMenu.Hide();
        GameManager.ItemActionMenu.Show(ItemActionMenuOnCancel);
    }

    private void ItemActionMenuOnCancel(Object[] objects)
    {
        GameManager.ItemActionMenu.Hide();
        CharacterActionMenuItems(objects);
        //GameManager.ItemDetailMenu.Show(ItemDetailMenuOnCancel);
        GameManager.ItemDetailMenu.Focus();
    }

    private void ItemActionMenuEquip(Object[] objects)
    {
        Debug.Log("ItemActionMenuEquip");
        Item item = (Item)objects[0];

        SelectedCharacter.Equip(item);

        GameManager.ItemActionMenu.Hide();
        CharacterActionMenuItems(null);
    }

    private void ItemActionMenuDrop(Object[] objects)
    {
        Item item = (Item)objects[0];

        SelectedCharacter.Items.Remove(item);

        GameManager.ItemActionMenu.Hide();

        if (SelectedCharacter.Items.Count > 0)
        {
            CharacterActionMenuItems(null);
        }
        else
        {
            GameManager.ItemDetailMenu.Hide();
            ShowCharacterActionMenu(SelectedCharacter);
        }
    }

    private void ItemActionMenuUse(Object[] objects)
    {
        throw new System.NotImplementedException();
    }

    private void ShowCharacterActionMenu(Character character)
    {
        character.DestroyMovableAndAttackableAndAssistableTransforms();
        Path.Hide();

        GameManager.CharacterActionMenu.Clear();

        // Attack
        HashSet<Vector2> attackablePositions = character.CalculateAttackablePositions();
        AttackableSpacesWithCharacters.Clear();
        if (attackablePositions.Count > 0)
        {
            GameManager.CharacterActionMenu.AddMenuItem(null, GameManager.AttackTextPrefab, CharacterActionMenuAttack);
        }

        // Assist
        HashSet<Vector2> assistablePositions = character.CalculateAssistablePositions();
        if (assistablePositions.Count > 0)
        {
            GameManager.CharacterActionMenu.AddMenuItem(null, GameManager.AssistTextPrefab, CharacterActionMenuAssist);
        }

        // Trade
        HashSet<Vector2> tradablePositionsWithCharacters = character.CalculateTradablePositionsWithCharacters();
        if (tradablePositionsWithCharacters.Count > 0)
        {
            GameManager.CharacterActionMenu.AddMenuItem(null, GameManager.TradeTextPrefab, CharacterActionMenuTrade);
        }

        // Items
        if (character.Items.Count > 0)
        {
            GameManager.CharacterActionMenu.AddMenuItem(null, GameManager.ItemsTextPrefab, CharacterActionMenuItems);
        }

        // Wait
        GameManager.CharacterActionMenu.AddMenuItem(null, GameManager.WaitTextPrefab, CharacterActionMenuWait);

        GameManager.CharacterActionMenu.Focus();
        GameManager.CharacterActionMenu.transform.position = new Vector2(character.transform.position.x - 1, character.transform.position.y);
        GameManager.CharacterActionMenu.Show(CharacterActionMenuOnCancel);
    }

    public void Move(Vector2 endposition, bool showInformationPanels = true)
    {
        Move(endposition.x, endposition.y);

        if (showInformationPanels)
        {
            UpdateInformationPanels();
        }
        else
        {
            HideInformationPanels();
        }
    }

    public void CharacterInformationSceneOnCancel()
    {
        Focus();
        UpdateInformationPanels();
    }

    /// <summary>
    /// Move the cursor to the provided position. No validation occurs.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void Move(float x, float y)
    {
        Vector2 endPosition = new Vector2((int)x, (int)y);
        Debug.LogFormat("Moving cursor from {0} to {1}", transform.position, endPosition);

        transform.position = endPosition;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="horizontal"></param>
    /// <param name="vertical"></param>
    public override void OnArrow(float horizontal, float vertical)
    {
        switch (CurrentState)
        {
            case State.Free:
                OnArrowFree(horizontal, vertical);
                break;

            case State.ChoosingMove:
                OnArrowChoosingMove(horizontal, vertical);
                break;

            case State.ChoosingAttackTarget:
                OnArrowChoosingAttackTarget(horizontal, vertical);
                break;

            case State.ChoosingAssistTarget:
                OnArrowChoosingAssistTarget(horizontal, vertical);
                break;

            case State.ChoosingTradeTarget:
                OnArrowChoosingTradeTarget(horizontal, vertical);
                break;

            default:
                Debug.LogErrorFormat("Invalid Cursor.State in OnArrow: {0}", CurrentState);
                break;
        }
    }

    /// <summary>
    /// Shows the movable and attackable positions for the character in the specified position.
    /// </summary>
    /// <param name="position"></param>
    private void ShowMovableAndAttackablePositions(Vector2 position)
    {
        Character character = GameManager.CurrentLevel.GetCharacter(position);

        if (character)
        {
            character.CreateAttackableTransforms();
        }
    }

    /// <summary>
    /// Called when one or several arrow buttons are pressed during the free state
    /// </summary>
    /// <param name="horizontal"></param>
    /// <param name="vertical"></param>
    private void OnArrowFree(float horizontal, float vertical)
    {
        Vector2 startPosition = transform.position;
        Vector2 newPosition = GetNewPosition(horizontal, vertical);

        if (startPosition.Equals(newPosition))
        {
            return;
        }

        Character currentCharacter = GameManager.CurrentLevel.GetCharacter(startPosition);
        if (currentCharacter)
        {
            currentCharacter.DestroyMovableAndAttackableAndAssistableTransforms();
        }

        Move(newPosition);
        ShowMovableAndAttackablePositions(newPosition);
    }

    private Vector2 GetNewPosition(float horizontal, float vertical)
    {
        Vector2 startPosition = transform.position;
        Vector2 newPosition = new Vector2(startPosition.x, startPosition.y);
        if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
        {
            newPosition.x += System.Math.Sign(horizontal);
        }
        else
        {
            newPosition.y += System.Math.Sign(vertical);
        }

        if (!GameManager.CurrentLevel.IsOutOfBounds(newPosition))
        {
            return newPosition;
        }

        return startPosition;
    }

    /// <summary>
    /// Called when one or several arrow buttons are pressed during the choosing move state
    /// </summary>
    /// <param name="horizontal">The horizontal direction of the arrow press</param>
    /// <param name="vertical">The vertical direction of the arrow press</param>
    /// <returns>The new position on the board</returns>
    private void OnArrowChoosingMove(float horizontal, float vertical)
    {
        Vector2 startPosition = transform.position;
        Vector2 newPosition = GetNewPosition(horizontal, vertical);

        if (startPosition.Equals(newPosition))
        {
            return;
        }
        Move(newPosition, true);

        if (SelectedCharacter.MovableTransforms.Exists(t => t.position.Equals(newPosition)))
        {
            Debug.LogFormat("Space is in movable spaces");

            Path.Add(newPosition);
        }
    }

    /// <summary>
    /// Called when one or more arrow keys are pressed during the choosing attack target state
    /// </summary>
    /// <param name="horizontal">The horizontal direction being pressed</param>
    /// <param name="vertical">The vertical direction being pressed</param>
    private void OnArrowChoosingAttackTarget(float horizontal, float vertical)
    {
        if (AttackableSpacesWithCharacters.Count == 1)
        {
            return;
        }

        int direction = System.Math.Sign(Mathf.Abs(vertical) > Mathf.Abs(horizontal) ? vertical : horizontal);
        if (direction != 0f)
        {
            SetAttackableSpaceWithCharacter((AttackableSpacesWithCharactersIndex + AttackableSpacesWithCharacters.Count + direction) % AttackableSpacesWithCharacters.Count);
        }
    }

    private void OnArrowChoosingAssistTarget(float horizontal, float vertical)
    {
        if (AssistableTransformsWithCharacters.Count == 1)
        {
            return;
        }

        int direction = System.Math.Sign(Mathf.Abs(vertical) > Mathf.Abs(horizontal) ? vertical : horizontal);
        if (direction != 0f)
        {
            SetAssistableTransformWithCharacter((AssistableTransformsWithCharactersIndex + AssistableTransformsWithCharacters.Count + direction) % AssistableTransformsWithCharacters.Count);
        }
    }

    private void OnArrowChoosingTradeTarget(float horizontal, float vertical)
    {
        if (TradableSpacesWithCharacters.Count == 1)
        {
            return;
        }

        int direction = System.Math.Sign(Mathf.Abs(vertical) > Mathf.Abs(horizontal) ? vertical : horizontal);
        if (direction != 0f)
        {
            SetTradableSpaceWithCharacter((TradableSpacesWithCharactersIndex + TradableSpacesWithCharacters.Count + direction) % TradableSpacesWithCharacters.Count);
        }
    }

    public void SetAttackableSpaceWithCharacter(int index = 0)
    {
        AttackableSpacesWithCharactersIndex = index;
        transform.position = AttackableSpacesWithCharacters[AttackableSpacesWithCharactersIndex].transform.position;

        GameManager.AttackDetailPanel.Show(SelectedCharacter, GameManager.CurrentLevel.GetCharacter(transform.position));
    }

    public void SetAssistableTransformWithCharacter(int index = 0)
    {
        AssistableTransformsWithCharactersIndex = index;
        transform.position = AssistableTransformsWithCharacters[AssistableTransformsWithCharactersIndex].transform.position;

        GameManager.AssistDetailPanel.Show(SelectedCharacter, GameManager.CurrentLevel.GetCharacter(transform.position));
    }

    public void SetTradableSpaceWithCharacter(int index = 0)
    {
        TradableSpacesWithCharactersIndex = index;
        transform.position = TradableSpacesWithCharacters[TradableSpacesWithCharactersIndex].transform.position;
    }

    public override void OnSubmit()
    {
        Vector2 currentPosition = transform.position;
        switch (CurrentState)
        {
            case State.ChoosingMove:
                OnSubmitChoosingMove(currentPosition);
                break;

            case State.Free:
                OnSubmitFree(currentPosition);
                break;

            case State.ChoosingAttackTarget:
                OnSubmitChoosingAttackTarget();
                break;

            case State.ChoosingTradeTarget:
                OnSubmitChoosingTradeTarget();
                break;

            case State.ChoosingAssistTarget:
                OnSubmitChoosingAssistTarget();
                break;

            default:
                Debug.LogErrorFormat("Invalid Cursor.State in OnSubmit: {0}", CurrentState);
                break;
        }
    }

    private void OnSubmitChoosingAssistTarget()
    {
        Debug.LogFormat("OnSubmitChoosingAssistTarget");

        Character defender = GameManager.CurrentLevel.GetCharacter(AssistableTransformsWithCharacters[AssistableTransformsWithCharactersIndex].position);
        Debug.LogFormat("Defender: {0}", defender);

        Character attacker = SelectedCharacter;
        Debug.LogFormat("Attacker: {0}", attacker);

        attacker.UseAssist(defender);

        attacker.DestroyAssistableTransforms();
        GameManager.AssistDetailPanel.Hide();

        CurrentState = State.Free;
    }

    private void OnSubmitChoosingAttackTarget()
    {
        Character defender = GameManager.CurrentLevel.GetCharacter(AttackableSpacesWithCharacters[AttackableSpacesWithCharactersIndex].position);
        Debug.LogFormat("Defender: {0}", defender);

        Character attacker = SelectedCharacter;
        Debug.LogFormat("Attacker: {0}", attacker);

        attacker.Attack(defender);

        attacker.DestroyAttackableTransforms();
        GameManager.AttackDetailPanel.Hide();
        GameManager.CharacterActionMenu.Hide();

        CurrentState = State.Free;
    }

    private void OnSubmitChoosingMove(Vector2 currentPosition)
    {
        Debug.LogFormat("OnEnterChoosingMove: {0}", currentPosition);
        foreach (Transform movableSpace in SelectedCharacter.MovableTransforms)
        {
            if (currentPosition.x == movableSpace.position.x && currentPosition.y == movableSpace.position.y)
            {
                Debug.Log("Found MovableSpace");
                Character checkCharacter = GameManager.CurrentLevel.GetCharacter(currentPosition);
                if (checkCharacter != null && !checkCharacter.Equals(SelectedCharacter))
                {
                    Debug.Log("Spot already taken!");
                    CurrentState = State.ChoosingMove;
                    return;
                }

                SelectedCharacterOldPosition = SelectedCharacter.transform.position;
                SelectedCharacter.Move(transform.position);
                HideInformationPanels();
                ShowCharacterActionMenu(SelectedCharacter);
                return;
            }
        }

        Debug.Log("Enter was pressed outside MovableSpaces");
    }

    private void OnSubmitFree(Vector2 currentPosition)
    {
        SelectedCharacter = GameManager.CurrentLevel.GetCharacter(currentPosition);
        if (SelectedCharacter != null)
        {
            if (SelectedCharacter.Player.Equals(GameManager.CurrentPlayer))
            {
                //AttackableRange.Clear();
                CurrentState = State.ChoosingMove;
                Path.StartPath(SelectedCharacter);
            }
            else
            {
                AttackableRange.AddCharacter(SelectedCharacter);
            }
        }
        else
        {
            GameManager.ShowPlayerActionMenu();
            return;
        }
    }

    private void OnSubmitChoosingTradeTarget()
    {
        Debug.Log("OnSubmitChoosingTradeTarget");
        GameManager.TradeDetailPanel.Show(SelectedCharacter, GameManager.CurrentLevel.GetCharacter(transform.position));
    }

    public override void OnCancel()
    {
        Debug.LogFormat("OnCancel: {0}", CurrentState);
        switch (CurrentState)
        {
            case State.Free:
                Character character = GameManager.CurrentLevel.GetCharacter(transform.position);

                if (character != null && AttackableRange.Characters.Contains(character))
                {
                    AttackableRange.RemoveCharacter(character);
                }
                else
                {
                    AttackableRange.Clear();
                }
                break;

            case State.ChoosingMove: // --> Free

                SelectedCharacter.DestroyMovableAndAttackableAndAssistableTransforms();
                Path.Destroy();
                CurrentState = State.Free;
                break;

            case State.ChoosingAssistTarget: // --> 
                transform.position = SelectedCharacter.transform.position;
                GameManager.AssistDetailPanel.Hide();
                CharacterActionMenuAssist();
                break;

            case State.ChoosingAttackTarget: // --> ItemSelectionMenu

                transform.position = SelectedCharacter.transform.position;
                GameManager.AttackDetailPanel.Hide();
                CharacterActionMenuAttack(null);
                break;

            case State.ChoosingTradeTarget: // --> CharacterActionMenu
                DestroyTradableSpaces();
                transform.position = SelectedCharacter.transform.position;
                ShowCharacterActionMenu(SelectedCharacter);
                break;

            default:
                Debug.LogErrorFormat("Invalid Cursor.State in OnCancel: {0}", CurrentState);
                break;
        }
    }

    public override void OnInformation()
    {
        Camera.main.GetComponent<AudioListener>().enabled = false;
        if (CurrentState.Equals(State.Free) && GameManager.CurrentLevel.GetCharacter(transform.position) != null)
        {
            Debug.Log("Loading character information scene");
            SceneManager.LoadScene("Scenes/CharacterInformation", LoadSceneMode.Additive);
        }
    }

    public void TradeDetailPanelOnClose()
    {
        Focus();
        CharacterActionMenuTrade(null);
    }

    private void DestroyTradableSpaces()
    {
        foreach (Transform tradableSpace in TradableSpacesWithCharacters)
        {
            Destroy(tradableSpace.gameObject);
        }
        TradableSpacesWithCharacters.Clear();
    }
}