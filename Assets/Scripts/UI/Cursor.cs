using System.Collections.Generic;
using UnityEngine;

public class Cursor : FocusableObject
{
    public static readonly float cursorSpeed = 10f;

    private int AttackableSpacesWithCharactersIndex = 0;
    public List<Transform> AttackableSpacesWithCharacters = new List<Transform>();

    private int TradableSpacesWithCharactersIndex = 0;
    public List<Transform> TradableSpacesWithCharacters = new List<Transform>();

    private Vector3 SelectedCharacterOldPosition;
    private Character SelectedCharacter;

    public enum State
    {
        ChoosingAttackTarget,
        ChoosingMove,
        ChoosingTradeTarget,
        Free
    };

    private State currentState = State.Free;

    public State CurrentState
    {
        get
        {
            return currentState;
        }

        set
        {
            Debug.Log(value);
            currentState = value;
        }
    }

    public void Update()
    {
        if(CurrentState != State.Free && GameManager.CharacterInformationPanel.gameObject.activeSelf) 
        {
            GameManager.CharacterInformationPanel.Hide();
        }
    }

    /// <summary>
    /// Callback when the user cancels from the CharacterActionMenu.
    /// </summary>
    /// <param name="character"></param>
    public void CharacterActionMenuOnCancel(Object[] objects)
    {
        SelectedCharacter.DestroyMovableAndAttackableSpaces();
        DestroyTradableSpaces();
        SelectedCharacter.Move(SelectedCharacterOldPosition);
        SelectedCharacter.CreateAttackableTransforms();
        GameManager.CharacterActionMenu.Hide();
        Focus();
        CurrentState = State.ChoosingMove;
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
    public void CharacterActionMenuWait(Object[] objects)
    {
        SelectedCharacter.DestroyMovableAndAttackableSpaces();
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
        foreach(Item item in SelectedCharacter.Items)
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
        Item item = (Item) objects[0];

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

        if(SelectedCharacter.Items.Count > 0) {
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
        SelectedCharacter.DestroyMovableAndAttackableSpaces();
        Focus();
        CurrentState = State.Free;
    }

    private void ShowCharacterActionMenu(Character character)
    {
        character.DestroyMovableAndAttackableSpaces();
        GameManager.CharacterActionMenu.Clear();

        // Attack
        AttackableSpacesWithCharacters = character.GetAttackableSpacesWithCharacters();
        if (AttackableSpacesWithCharacters.Count > 0)
        {
            GameManager.CharacterActionMenu.AddMenuItem(null, GameManager.AttackTextPrefab, CharacterActionMenuAttack);
        }

        // Items
        if(character.Items.Count > 0)
        {
            GameManager.CharacterActionMenu.AddMenuItem(null, GameManager.ItemsTextPrefab, CharacterActionMenuItems);
        }
        
        // Trade
        DestroyTradableSpaces();
        TradableSpacesWithCharacters = character.CalculateTradableSpaces();
        if (TradableSpacesWithCharacters.Count > 0)
        {
            GameManager.CharacterActionMenu.AddMenuItem(null, GameManager.TradeTextPrefab, CharacterActionMenuTrade);
        }

        // Wait
        GameManager.CharacterActionMenu.AddMenuItem(null, GameManager.WaitTextPrefab, CharacterActionMenuWait);

        GameManager.CharacterActionMenu.Focus();
        GameManager.CharacterActionMenu.Show(CharacterActionMenuOnCancel);
    }

    private void Move(Vector3 endposition)
    {
        transform.position = endposition;
        Character character = GameManager.GetCharacter(endposition);
        if(character != null && CurrentState == State.Free)
        {
            GameManager.CharacterInformationPanel.Show(character);
        }
        else
        {
            GameManager.CharacterInformationPanel.Hide();
        }
    }

    public override void OnArrow(float horizontal, float vertical)
    {
        switch (CurrentState)
        {
            case State.Free:
            case State.ChoosingMove:
                OnArrowFreeChoosingMove(horizontal, vertical);
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
        if(TradableSpacesWithCharacters.Count == 0)
        {
            return;
        }

        float direction = Mathf.Max(vertical, horizontal);
        if (direction != 0f)
        {
            SetTradableSpaceWithCharacter((TradableSpacesWithCharactersIndex + TradableSpacesWithCharacters.Count + System.Math.Sign(direction)) % TradableSpacesWithCharacters.Count);
        }
    }

    private void OnArrowFreeChoosingMove(float horizontal, float vertical)
    {
        Vector3 startPosition = transform.position;
        Vector3 newPosition = new Vector3(startPosition.x, startPosition.y);
        if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
        {
            newPosition.x += System.Math.Sign(horizontal);
        }
        else
        {
            newPosition.y += System.Math.Sign(vertical);
        }

        if (!GameManager.IsOutOfBounds(newPosition))
        {
            Move(newPosition);
        }
    }

    private void OnArrowChoosingAttackTarget(float horizontal, float vertical)
    {
        if(AttackableSpacesWithCharacters.Count == 1)
        {
            return;
        }

        float direction = Mathf.Max(Mathf.Abs(vertical), Mathf.Abs(horizontal));
        if (direction != 0f)
        {
            SetAttackableSpaceWithCharacter((AttackableSpacesWithCharactersIndex + AttackableSpacesWithCharacters.Count + System.Math.Sign(direction)) % AttackableSpacesWithCharacters.Count);
        }
    }

    public void SetAttackableSpaceWithCharacter(int index = 0)
    {
        AttackableSpacesWithCharactersIndex = index;
        transform.position = AttackableSpacesWithCharacters[AttackableSpacesWithCharactersIndex].transform.position;

        GameManager.AttackDetailPanel.Show(SelectedCharacter, GameManager.GetCharacter(transform.position));
    }

    public void SetTradableSpaceWithCharacter(int index = 0)
    {
        TradableSpacesWithCharactersIndex = index;
        transform.position = TradableSpacesWithCharacters[TradableSpacesWithCharactersIndex].transform.position;
    }

    public override void OnSubmit()
    {
        Vector3 currentPosition = transform.position;
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
        Character defender = GameManager.GetCharacter(AttackableSpacesWithCharacters[AttackableSpacesWithCharactersIndex].position);
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

    private void OnSubmitChoosingMove(Vector3 currentPosition)
    {
        Debug.Log("OnEnterChoosingMove: " + currentPosition);
        foreach (Transform movableSpace in SelectedCharacter.CreateMovableTransforms())
        {
            if (currentPosition.x == movableSpace.position.x && currentPosition.y == movableSpace.position.y)
            {
                Debug.Log("Found MovableSpace");
                Character checkCharacter = GameManager.GetCharacter(currentPosition);
                if (checkCharacter != null && !checkCharacter.Equals(SelectedCharacter))
                {
                    Debug.Log("Spot already taken!");
                    CurrentState = State.ChoosingMove;
                    return;
                }

                SelectedCharacterOldPosition = SelectedCharacter.GetPosition();
                SelectedCharacter.Move(transform.position);
                ShowCharacterActionMenu(SelectedCharacter);
                return;
            }
        }

        Debug.Log("Enter was pressed outside MovableSpaces");

        SelectedCharacter.DestroyMovableAndAttackableSpaces();
        //GameManager.CurrentState = GameManager.State.Menu;
        CurrentState = State.Free;
    }

    private void OnSubmitFree(Vector3 currentPosition)
    {
        SelectedCharacter = GameManager.GetCharacter(currentPosition);
        if (SelectedCharacter != null)
        {
            if (SelectedCharacter.Player.Equals(GameManager.CurrentPlayer))
            {
                SelectedCharacter.CreateAttackableTransforms();
                CurrentState = State.ChoosingMove;
            }
            else
            {
                // TODO open character info
                Debug.LogError("Have not created character info yet.");
                return;
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
        GameManager.TradeDetailPanel.Show(SelectedCharacter, GameManager.GetCharacter(transform.position));
    }

    public override void OnCancel()
    {
        switch (CurrentState)
        {
            case State.Free:
                break;

            case State.ChoosingMove: // --> Free

                SelectedCharacter.DestroyAttackableTransforms();
                SelectedCharacter.DestroyMovableTransforms();
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

    //private IEnumerator Move(Vector3 endPosition)
    //{
    //    yield return Move(endPosition, GameManager.CurrentState);
    //}

    //private IEnumerator Move(Vector3 endPosition, GameManager.State nextState)
    //{
    //    GameManager.CurrentState = GameManager.State.Busy;

    //    float time = 0f;

    //    while (time < 1f)
    //    {
    //        time += Time.deltaTime * cursorSpeed;
    //        transform.position = Vector3.Lerp(a: transform.position, b: endPosition, t: time);
    //        yield return null;
    //    }

    //    GameManager.CurrentState = nextState;
    //    yield break;
    //}
}