using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UI;
using System.Collections;

public class Cursor : FocusableObject
{
    public static readonly float cursorSpeed = 10f;

    private int AttackableSpacesWithCharactersIndex = 0;
    public readonly List<Transform> AttackableSpacesWithCharacters = new List<Transform>();

    private int TradableSpacesWithCharactersIndex = 0;
    public readonly List<Transform> TradableSpacesWithCharacters = new List<Transform>();

    private Vector2 SelectedCharacterOldPosition;
    private Character SelectedCharacter;

    public AttackableRange AttackableRange;

    public Path Path;

    public enum State
    {
        ChoosingAttackTarget,
        ChoosingMove,
        ChoosingTradeTarget,
        Free
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
            Debug.Log("Current state: " + value);
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

    private void UpdateInformationPanels()
    {
        if (currentState == State.Free)
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
        else
        {
            HideInformationPanels();
        }

    }

    public void Update()
    {
        if (CurrentState != State.Free && GameManager.CharacterInformationPanel.gameObject.activeSelf)
        {
            GameManager.CharacterInformationPanel.Hide();
        }
    }

    /// <summary>
    /// Callback when the user selects 'Info' from the CharacterActionMenu.
    /// </summary>
    /// <param name="objects"></param>
    private void CharacterActionMenuInfo(Object[] objects)
    {
        SceneManager.LoadScene("Scenes/CharacterInformation", LoadSceneMode.Additive);
    }

    /// <summary>
    /// Callback when the user cancels from the CharacterActionMenu.
    /// </summary>
    /// <param name="character"></param>
    public void CharacterActionMenuOnCancel(Object[] objects)
    {
        SelectedCharacter.DestroyMovableAndAttackableTransforms();
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
        DestroyTradableSpaces();
        GameManager.ItemSelectionMenu.Clear();
        foreach (Item item in SelectedCharacter.GetUsableWeapons())
        {
            GameManager.ItemSelectionMenu.AddMenuItem(item, item.Text, ItemSelectionMenuOnSelect);
        }

        GameManager.ItemSelectionMenu.Focus();
        GameManager.CharacterActionMenu.Hide();
        GameManager.ItemSelectionMenu.Show(ItemSelectionMenuOnCancel);
    }

    private void CharacterActionMenuTrade(Object[] objects)
    {
        Debug.Log("OnEnterCharacterActionMenuTrade");
        CurrentState = State.ChoosingTradeTarget;
        SelectedCharacter.DestroyAttackableTransforms();
        SetTradableSpaceWithCharacter();
        Focus();
    }

    /// <summary>
    /// Callback when the user selects the wait option from the CharacterActionMenu.
    /// </summary>
    /// <param name="character"></param>
    private void CharacterActionMenuWait(Object[] objects)
    {
        SelectedCharacter.DestroyMovableAndAttackableTransforms();
        DestroyTradableSpaces();
        GameManager.CharacterActionMenu.Hide();
        CurrentState = State.Free;
        Focus();
    }

    private void ItemSelectionMenuOnSelect(Object[] objects)
    {
        Focus();
        Item item = GameManager.ItemSelectionMenu.MenuItems[GameManager.ItemSelectionMenu.CurrentMenuItemIndex].ItemObject;
        Debug.Log("Selected a weapon: " + item.Text.text);
        SelectedCharacter.Equip(item);
        CurrentState = State.ChoosingAttackTarget;
        GameManager.ItemSelectionMenu.Hide();
        SetAttackableSpaceWithCharacter();
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

        GameManager.ItemActionMenu.Show(ItemActionMenuOnCancel);
    }

    private void ItemActionMenuOnCancel(Object[] objects)
    {
        GameManager.ItemActionMenu.Hide();
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
        Item item = (Item)objects[0];

        SelectedCharacter.Items.Remove(item);

        GameManager.ItemActionMenu.Hide();
        GameManager.ItemDetailMenu.Hide();
        SelectedCharacter.DestroyMovableAndAttackableTransforms();
        Focus();
        CurrentState = State.Free;
    }

    private void ShowCharacterActionMenu(Character character)
    {
        character.DestroyMovableAndAttackableTransforms();
        GameManager.CharacterActionMenu.Clear();
        Path.Hide();

        // Attack
        AttackableSpacesWithCharacters.Clear();
        AttackableSpacesWithCharacters.AddRange(character.GetAttackableSpacesWithCharacters());
        if (AttackableSpacesWithCharacters.Count > 0)
        {
            GameManager.CharacterActionMenu.AddMenuItem(null, GameManager.AttackTextPrefab, CharacterActionMenuAttack);
        }

        // Items
        if (character.Items.Count > 0)
        {
            GameManager.CharacterActionMenu.AddMenuItem(null, GameManager.ItemsTextPrefab, CharacterActionMenuItems);
        }

        // Trade
        DestroyTradableSpaces();
        TradableSpacesWithCharacters.Clear();
        TradableSpacesWithCharacters.AddRange(character.CalculateTradableSpaces());
        if (TradableSpacesWithCharacters.Count > 0)
        {
            GameManager.CharacterActionMenu.AddMenuItem(null, GameManager.TradeTextPrefab, CharacterActionMenuTrade);
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

            case State.ChoosingTradeTarget:
                OnArrowChoosingTradeTarget(horizontal, vertical);
                break;

            default:
                Debug.LogError("Invalid GameManager.State in OnArrow: " + CurrentState);
                break;
        }
    }

    private void OnArrowChoosingTradeTarget(float horizontal, float vertical)
    {
        if (TradableSpacesWithCharacters.Count == 0)
        {
            return;
        }

        float direction = Mathf.Max(vertical, horizontal);
        if (direction != 0f)
        {
            SetTradableSpaceWithCharacter((TradableSpacesWithCharactersIndex + TradableSpacesWithCharacters.Count + System.Math.Sign(direction)) % TradableSpacesWithCharacters.Count);
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
            _ = character.CreateAttackableTransforms();
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
            currentCharacter.DestroyMovableAndAttackableTransforms();
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
        Move(newPosition);

        if (SelectedCharacter.MovableSpaces.Exists(t => t.position.Equals(newPosition)))
        {
            Debug.LogFormat("Space is in movable spaces");

            Path.Add(newPosition);
        }
    }

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

    public void SetAttackableSpaceWithCharacter(int index = 0)
    {
        AttackableSpacesWithCharactersIndex = index;
        transform.position = AttackableSpacesWithCharacters[AttackableSpacesWithCharactersIndex].transform.position;

        GameManager.AttackDetailPanel.Show(SelectedCharacter, GameManager.CurrentLevel.GetCharacter(transform.position));
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

            default:
                Debug.LogError("Invalid state: " + CurrentState);
                break;
        }
    }

    private void OnSubmitChoosingAttackTarget()
    {
        Character defender = GameManager.CurrentLevel.GetCharacter(AttackableSpacesWithCharacters[AttackableSpacesWithCharactersIndex].position);
        Debug.Log("defender: " + defender);

        Character attacker = SelectedCharacter;
        Debug.Log("Attacker: " + attacker);

        attacker.Attack(defender);

        //Destroy(defender.transform.gameObject);
        attacker.DestroyAttackableTransforms();
        GameManager.AttackDetailPanel.Hide();
        GameManager.CharacterActionMenu.Hide();

        CurrentState = State.Free;
    }

    private void OnSubmitChoosingMove(Vector2 currentPosition)
    {
        Debug.Log("OnEnterChoosingMove: " + currentPosition);
        foreach (Transform movableSpace in SelectedCharacter.MovableSpaces)
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
                ShowCharacterActionMenu(SelectedCharacter);
                return;
            }
        }

        Debug.Log("Enter was pressed outside MovableSpaces");

        SelectedCharacter.DestroyMovableAndAttackableTransforms();
        //GameManager.CurrentState = GameManager.State.Menu;
        CurrentState = State.Free;
    }

    private void OnSubmitFree(Vector2 currentPosition)
    {
        SelectedCharacter = GameManager.CurrentLevel.GetCharacter(currentPosition);
        if (SelectedCharacter != null)
        {
            if (SelectedCharacter.Player.Equals(GameManager.CurrentPlayer))
            {
                // SelectedCharacter.CreateAttackableTransforms();
                AttackableRange.Clear();
                CurrentState = State.ChoosingMove;
                Path.StartPath(SelectedCharacter);
            }
            else
            {
                //Debug.LogError("Need to implement enemy attackable spaces");
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
        switch (CurrentState)
        {
            case State.Free:
                Debug.Log("Cursor:OnCancel:Free");
                Character character = GameManager.CurrentLevel.GetCharacter(transform.position);

                if (character != null && AttackableRange.Characters.Contains(character))
                {
                    //character.DestroyMovableAndAttackableSpaces();
                    AttackableRange.RemoveCharacter(character);
                }
                else
                {
                    AttackableRange.Clear();
                }
                break;

            case State.ChoosingMove: // --> Free

                SelectedCharacter.DestroyAttackableTransforms();
                SelectedCharacter.DestroyMovableTransforms();
                Path.Destroy();
                CurrentState = State.Free;
                break;

            case State.ChoosingAttackTarget: // --> ItemSelectionMenu

                transform.position = SelectedCharacter.transform.position;
                GameManager.AttackDetailPanel.Hide();
                CharacterActionMenuAttack(null);
                //                ShowCharacterActionMenu(character);
                break;

            case State.ChoosingTradeTarget: // --> CharacterActionMenu
                DestroyTradableSpaces();
                transform.position = SelectedCharacter.transform.position;
                ShowCharacterActionMenu(SelectedCharacter);
                break;

            default:
                Debug.LogError("Invalid state: " + CurrentState);
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

    public override void OnRightMouse(Vector2 mousePosition)
    {
        Move(mousePosition, false);
        if (GameManager.CurrentLevel.GetCharacter(mousePosition) != null)
        {
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