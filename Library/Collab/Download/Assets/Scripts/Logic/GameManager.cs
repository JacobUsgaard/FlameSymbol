using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Characters")]
    public Transform KnightPrefab;
    public Transform WizardPrefab;

    [Header("Text")]
    public Text AttackTextPrefab;
    public Text DropTextPrefab;
    public Text EndTurnTextPrefab;
    public Text EquipTextPrefab;
    public Text ItemsTextPrefab;
    public Text TradeTextPrefab;
    public Text UseTextPrefab;
    public Text WaitTextPrefab;

    [Header("UI")]
    public Transform AttackableSpacePrefab;
    public AttackDetailPanel AttackDetailPanel;
    public Menu CharacterActionMenu;
    public Cursor Cursor;
    public Menu ItemActionMenu;
    public Menu ItemDetailMenu;
    public Menu ItemSelectionMenu;
    public TradeDetailPanel TradeDetailPanel;
    public Transform MovableSpacePrefab;
    public Menu PlayerActionMenu;

    [Header("Weapons")]
    public Text FireballTextPrefab;
    public Text IronAxeTextPrefab;
    public Text IronSwordTextPrefab;

    private bool IsHumanTurn = false;
    private Player currentPlayer;
    private Character[,] _characterMap;
    private Player HumanPlayer;
    private Player AiPlayer;
    private bool _characterIsMoving = false;

    // Use this for initialization
    public void Start()
    {
        Debug.Log(ItemActionMenu.Equals(ItemDetailMenu));

        CreatePlayerActionMenu();
        HumanPlayer = Player.CreateInstance(this);
        AiPlayer = Player.CreateInstance(this);

        _characterMap = new Character[4, 4];

        Transform transform = Instantiate(KnightPrefab, parent: this.transform);
        Character character = transform.GetComponent<Character>();
        character.Player = AiPlayer;
        Text text = Instantiate(IronSwordTextPrefab, parent: this.transform);
        Item item = text.GetComponent<Item>();
        character.Items.Add(item);
        SetCharacter(character, 0, 1);

        transform = Instantiate(WizardPrefab, parent: this.transform);
        character = transform.GetComponent<Character>();
        character.Player = AiPlayer;
        text = Instantiate(IronSwordTextPrefab, parent: this.transform);
        character.Items.Add(text.GetComponent<Item>());
        text = Instantiate(FireballTextPrefab, parent: this.transform);
        character.Items.Add(text.GetComponent<Item>());
        SetCharacter(character, 1, 2);

        transform = Instantiate(WizardPrefab, parent: this.transform);
        character = transform.GetComponent<Character>();
        character.Player = HumanPlayer;
        text = Instantiate(IronSwordTextPrefab, parent: this.transform);
        character.Items.Add(text.GetComponent<Item>());
        SetCharacter(character, 2, 2);

        transform = Instantiate(KnightPrefab, parent: this.transform);
        character = transform.GetComponent<Character>();
        character.Player = HumanPlayer;
        text = Instantiate(IronSwordTextPrefab, parent: this.transform);
        character.Items.Add(text.GetComponent<Item>());
        SetCharacter(character, 3, 3);

        /* _characterMap = new Character[,] {
            {null, Character.CreateInstance<KnightScript>(AiPlayer), null, null},
            {null, null, Character.CreateInstance<WizardScript>(HumanPlayer), null},
            {null, null, Character.CreateInstance<WizardScript>(HumanPlayer), null},
            {null, null, null, Character.CreateInstance<KnightScript>(HumanPlayer)}
        }; */

        for (int x = 0; x < _characterMap.GetLength(0); x++)
        {
            for (int y = 0; y < _characterMap.GetLength(1); y++)
            {
                character = GetCharacter(x, y);
                if (character != null)
                {
                    character.DrawCharacter(x, y);
                }
            }
        }

        //transform = Instantiate(CursorPrefab, new Vector3(0, 0), Quaternion.identity, this.transform);

        //Cursor = transform.GetComponent<Cursor>();

        CurrentPlayer = HumanPlayer;
        IsHumanTurn = true;
        Cursor.Focus();

        //CurrentPlayer = AiPlayer;
    }

    public delegate void Callback(params Object[] objects);

    private void CreatePlayerActionMenu()
    {
        PlayerActionMenu.AddMenuItem(null, EndTurnTextPrefab, delegate (Object[] objects)
        {
            Debug.Log("Ending turn");
            //GameManager.EndTurn();
            //CurrentState = State.Free;
            //FocusableObject.Revert();
        });
    }

    public class Tuple<Type1, Type2>
    {
        public Type1 value1;
        public Type2 value2;

        public Tuple(Type1 type1, Type2 type2)
        {
            value1 = type1;
            value2 = type2;
        }
    }

    void Update()
    {
        if (IsHumanTurn || _characterIsMoving)
        {
            HandleInput();
            return;
        }

        Debug.Log("AI time");
        CurrentPlayer = AiPlayer;
        foreach (Character character in AiPlayer.Characters)
        {
            Debug.Log("Attacking character: " + character.transform.position);
            Tuple<Vector3, Vector3> moveAttack = Moves(character, character.transform.position.x, character.transform.position.y, character.Moves, character.CalculateRanges());

            if (moveAttack != null)
            {
                Debug.Log("Found move: " + moveAttack.value1 + ", attack: " + moveAttack.value2);
                _characterIsMoving = true;
                Debug.LogError("Need to actually move character");
                //MoveCharacter(character, moveAttack.value1, delegate (Object[] objects)
                //{
                //    Attack(character, GetCharacter(moveAttack.value2));
                //}); 
            }
            else
            {
                Debug.Log("Could not find move attack. Trying to find move.");
                //Vector3 move = FindMove(character);
                Debug.LogError("Need to actually move character");
                //MoveCharacter(character, move, CurrentState);
            }
        }

        IsHumanTurn = true;
        CurrentPlayer = HumanPlayer;
    }

    //public void MoveCharacter(Character character, Vector3 newPosition, State nextState)
    //{
    //    CurrentState = State.Busy;
    //    SetCharacter(null, character.GetPosition());
    //    SetCharacter(character, newPosition);
    //    character.Move(newPosition);
    //    CurrentState = nextState;
    //}

    //public void MoveCharacter(Character character, Vector3 newPosition, Callback callback)
    //{
    //    MoveCharacter(character, newPosition, State.Busy);
    //    callback();
    //}

    void HandleInput()
    {
        FocusableObject currentFocusableObject = FocusableObject.CurrentObject;

        if (Input.GetButtonDown("Submit"))
        {
            Debug.Log("Submit");
            currentFocusableObject.OnSubmit();
            return;
        }

        if (Input.GetButtonDown("Cancel"))
        {
            Debug.Log("Cancel");
            currentFocusableObject.OnCancel();
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0f || vertical != 0f)
        {
            Debug.Log("Arrow");
            FocusableObject.CurrentObject.OnArrow(horizontal, vertical);
            System.Threading.Thread.Sleep(50);
            return;
        }
    }

    public void ShowPlayerActionMenu()
    {
        PlayerActionMenu.Show(delegate(Object[] objects)
        {
            PlayerActionMenu.Hide();
            //FocusableObject.Revert();
        });
        PlayerActionMenu.Focus();
    }

    public Vector3 FindMove(Character attackingCharacter)
    {
        System.Random random = new System.Random();

        Vector3 oldPosition = attackingCharacter.transform.position;
        Vector3 newPosition = new Vector3(-1, -1);
        do
        {
            switch (random.Next(3))
            {
                case 0:
                    newPosition = new Vector3(oldPosition.x + 1, oldPosition.y);
                    break;
                case 1:
                    newPosition = new Vector3(oldPosition.x - 1, oldPosition.y);
                    break;
                case 2:
                    newPosition = new Vector3(oldPosition.x, oldPosition.y + 1);
                    break;
                case 3:
                    newPosition = new Vector3(oldPosition.x, oldPosition.y - 1);
                    break;
            }
        } while (IsOutOfBounds(newPosition));

        return newPosition;
    }

    /// <summary>
    /// A callback after the character has finished moving and has an attack all lined up.
    /// </summary>
    /// <param name="attackingCharacter"></param>
    /// <param name="defendingCharacter"></param>
    /// <returns></returns>
    public void Attack(Character attackingCharacter, Character defendingCharacter)
    {
        Attack(attackingCharacter, defendingCharacter);
        _characterIsMoving = false;
    }

    /// <summary>
    /// Find a move and attack.
    /// TODO make this smarter by moving closer to an enemy. Any enemy.
    /// </summary>
    /// <param name="attackingCharacter"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="movesRemaining"></param>
    /// <param name="attackRanges"></param>
    /// <returns></returns>
    public Tuple<Vector3, Vector3> Moves(Character attackingCharacter, float x, float y, int movesRemaining, HashSet<int> attackRanges)
    {
        if (movesRemaining == 0 || IsOutOfBounds(x, y))
        {
            return null;
        }

        Character defendingCharacter = Attacks(x, y, attackRanges);
        if (defendingCharacter != null && !defendingCharacter.Equals(attackingCharacter))
        {
            return new Tuple<Vector3, Vector3>(new Vector3(x, y), defendingCharacter.transform.position);
        }

        Tuple<Vector3, Vector3> moveAttack = Moves(attackingCharacter, x - 1, y, movesRemaining - 1, attackRanges);
        if (moveAttack != null)
        {
            return moveAttack;
        }

        moveAttack = Moves(attackingCharacter, x + 1, y, movesRemaining - 1, attackRanges);
        if (moveAttack != null)
        {
            return moveAttack;
        }

        moveAttack = Moves(attackingCharacter, x, y - 1, movesRemaining - 1, attackRanges);
        if (moveAttack != null)
        {
            return moveAttack;
        }

        moveAttack = Moves(attackingCharacter, x, y + 1, movesRemaining - 1, attackRanges);
        if (moveAttack != null)
        {
            return moveAttack;
        }

        return null;
    }

    /// <summary>
    /// Find the first available enemy for attacking.
    /// TODO make this smarter so that it evaluates options a bit
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="attackRanges"></param>
    /// <returns></returns>
    public Character Attacks(float x, float y, HashSet<int> attackRanges)
    {
        foreach (int attackRange in attackRanges)
        {
            Character character = Attacks(x, y, attackRange);
            if (character != null)
            {
                return character;
            }
        }

        return null;
    }

    /// <summary>
    /// Find the first available enemy for attacking.
    /// TODO make this smarter so that it evaluates options a bit
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="attackRangeRemaining"></param>
    /// <returns></returns>
    public Character Attacks(float x, float y, int attackRangeRemaining)
    {
        if (attackRangeRemaining < 0 || IsOutOfBounds(x, y))
        {
            return null;
        }

        Character character;
        if (attackRangeRemaining == 0)
        {
            character = GetCharacter(x, y);
            if (character != null && !character.Player.Equals(CurrentPlayer))
            {
                return character;
            }
        }

        character = Attacks(x - 1, y, attackRangeRemaining - 1);
        if (character != null)
        {
            return character;
        }

        character = Attacks(x + 1, y, attackRangeRemaining - 1);
        if (character != null)
        {
            return character;
        }

        character = Attacks(x, y - 1, attackRangeRemaining - 1);
        if (character != null)
        {
            return character;
        }

        character = Attacks(x, y + 1, attackRangeRemaining - 1);
        if (character != null)
        {
            return character;
        }

        return null;
    }

    public void EndTurn()
    {
        EndTurn(CurrentPlayer);
    }

    public void EndTurn(Player player)
    {
        if (!player.Equals(CurrentPlayer))
        {
            Debug.LogError("Player that is not current player is ending their turn. Current player: " + CurrentPlayer + ", calling player: " + player);
            return;
        }

        if (HumanPlayer.Equals(CurrentPlayer))
        {
            IsHumanTurn = false;
            CurrentPlayer = AiPlayer;
        }
        else if (AiPlayer.Equals(CurrentPlayer))
        {
            IsHumanTurn = true;
            CurrentPlayer = HumanPlayer;
        }
        else
        {
            Debug.LogError("Player 3 has entered");
            return;
        }
    }

    /// <summary>
    /// Get the character at the given position.
    /// </summary>
    /// <param name="x">The x coordination of the position in question.</param>
    /// <param name="y">The y coordination of the position in question.</param>
    /// <returns>The character, if it exists</returns>
    public Character GetCharacter(float x, float y)
    {
        if (IsOutOfBounds(x, y))
        {
            return null;
        }
        return _characterMap[(int)x, (int)y];
    }

    /// <summary>
    /// Get the character at the given position.
    /// </summary>
    /// <param name="position">The position in question.</param>
    /// <returns>The character, if it exists</returns>
    public Character GetCharacter(Vector3 position)
    {
        return GetCharacter(position.x, position.y);
    }

    /// <summary>
    /// Check the board to see if the position is out of bounds.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool IsOutOfBounds(Vector3 position)
    {
        return IsOutOfBounds(position.x, position.y);
    }

    /// <summary>
    /// Check the board to see if the position is out of bounds.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool IsOutOfBounds(float x, float y)
    {
        //    Debug.Log("new position: (" + x + "," + y + ")");
        return (x < 0f || x >= _characterMap.GetLength(0) || y < 0f || y >= _characterMap.GetLength(1));
    }

    /// <summary>
    /// Update the character map.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="newPosition"></param>
    public void SetCharacter(Character character, Vector3 newPosition)
    {
        SetCharacter(character, newPosition.x, newPosition.y);
    }

    /// <summary>
    /// Update the character map.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetCharacter(Character character, float x, float y)
    {
        if (IsOutOfBounds(x, y))
        {
            Debug.LogError("Position is out of bounds: (" + x + "," + y + ")");
        }
        _characterMap[(int)x, (int)y] = character;
    }

    public Player CurrentPlayer
    {
        get
        {
            return currentPlayer;
        }

        set
        {
            currentPlayer = value;
        }
    }
}