using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Characters")]
    public Transform KnightPrefab;
    public Transform WizardPrefab;

    [Header("Terrain")]
    public Transform GrassTerrain;
    public Transform WaterTerrain;
    public Transform WallTerrain;

    [Header("Text")]
    public Text AttackTextPrefab;
    public Text DropTextPrefab;
    public Text EndTurnTextPrefab;
    public Text EquipTextPrefab;
    public Text IncinerateTextPrefab;
    public Text ItemsTextPrefab;
    public Text TradeTextPrefab;
    public Text UseTextPrefab;
    public Text WaitTextPrefab;

    [Header("UI")]
    public Transform AttackableSpacePrefab;
    public AttackDetailPanel AttackDetailPanel;
    public Menu CharacterActionMenu;
    public CharacterInformationPanel CharacterInformationPanel;
    public Cursor Cursor;
    public Menu ItemActionMenu;
    public Menu ItemDetailMenu;
    public Menu ItemSelectionMenu;
    public Transform MovableSpacePrefab;
    public Menu PlayerActionMenu;
    public TradeDetailPanel TradeDetailPanel;

    [Header("Weapons")]
    public Text FireTextPrefab;
    public Text IronAxeTextPrefab;
    public Text IronSwordTextPrefab;

    public static GameManager gameManager;
    
    private bool IsHumanTurn = false;
    private Player currentPlayer;
    private Player HumanPlayer;
    private Player AiPlayer;
    private bool _characterIsMoving = false;

    public Level CurrentLevel;

    // Use this for initialization
    public void Start()
    {
        gameManager = this;

        HumanPlayer = ScriptableObject.CreateInstance<Player>();
        AiPlayer = ScriptableObject.CreateInstance<Player>();

        CurrentLevel = ScriptableObject.CreateInstance<TestLevel>();
        CurrentLevel.Init(this, HumanPlayer, AiPlayer);
        
        Cursor.transform.position = CurrentLevel.StartPosition;

        CurrentPlayer = HumanPlayer;
        IsHumanTurn = true;
        Cursor.Focus();

        //CurrentPlayer = AiPlayer;
    }

    public delegate void Callback(params Object[] objects);

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

            foreach(Vector2 movablePosition in character.CalculateMovablePositions())
            {
                if(GetCharacter(movablePosition) != null)
                {
                    continue;
                }

                character.Move(movablePosition);
                HashSet<Vector2> attackablePositions = character.CalculateAttackablePositions(movablePosition.x, movablePosition.y, character.GetWeaponRanges());
                foreach(Vector2 attackablePosition in attackablePositions)
                {
                    Character defendingCharacter = GetCharacter(attackablePosition);
                    if (defendingCharacter == null || defendingCharacter.Player.Equals(character.Player))
                    {
                        continue;
                    }

                    character.Attack(defendingCharacter);
                    goto EndTurn;
                }
            }
            //Tuple<Vector2, Vector2> moveAttack = Moves(character, character.transform.position.x, character.transform.position.y, character.Moves, character.GetWeaponRanges());

            //if (moveAttack != null)
            //{
            //    Debug.Log("Found move: " + moveAttack.value1 + ", attack: " + moveAttack.value2);
            //    _characterIsMoving = true;
            //    character.Move(moveAttack.value1);
            //    character.Attack(CurrentLevel.GetCharacter(moveAttack.value2));
            //    //Debug.LogError("Need to actually move character");
            //    //MoveCharacter(character, moveAttack.value1, delegate (Object[] objects)
            //    //{
            //    //    Attack(character, GetCharacter(moveAttack.value2));
            //    //});
            //}
            //else
            //{
            //    Debug.Log("Could not find move attack. Trying to find move.");
            //    //Vector2 move = FindMove(character);
            //    Debug.LogError("Need to actually move character");
            //    //MoveCharacter(character, move, CurrentState);
            //}
        }

        EndTurn:
        CurrentPlayer = HumanPlayer;
        IsHumanTurn = true;
        Cursor.Focus();
    }

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
        PlayerActionMenu.AddMenuItem(null, EndTurnTextPrefab, delegate (Object[] objects)
        {
            Debug.Log("Ending turn");
            gameManager.EndTurn();
            PlayerActionMenu.Hide();
            Cursor.CurrentState = Cursor.State.Free;
            Cursor.Focus();
        });
        PlayerActionMenu.Show(delegate(Object[] objects)
        {
            PlayerActionMenu.Hide();
        });
        PlayerActionMenu.Focus();
    }

    public Vector2 FindMove(Character attackingCharacter)
    {
        System.Random random = new System.Random();

        Vector2 oldPosition = attackingCharacter.transform.position;
        Vector2 newPosition = new Vector2(-1, -1);
        do
        {
            switch (random.Next(3))
            {
                case 0:
                    newPosition = new Vector2(oldPosition.x + 1, oldPosition.y);
                    break;
                case 1:
                    newPosition = new Vector2(oldPosition.x - 1, oldPosition.y);
                    break;
                case 2:
                    newPosition = new Vector2(oldPosition.x, oldPosition.y + 1);
                    break;
                case 3:
                    newPosition = new Vector2(oldPosition.x, oldPosition.y - 1);
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

    private void EndTurn(Player player)
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

    public class MoveInfo
    {
        public Vector2 Move;
        public Vector2 Attack;

        public MoveInfo() { }
    }

    // TODO delete this and use Level.GetCharacter directly
    public Character GetCharacter(float x, float y)
    {
        return CurrentLevel.GetCharacter(x, y);
    }

    // TODO delete this and use Level.GetCharacter directly
    public Character GetCharacter(Vector2 position)
    {
        return GetCharacter(position.x, position.y);
    }

    
    /// TODO delete this and use Level.IsOutOfBounds directly
    public bool IsOutOfBounds(Vector2 position)
    {
        return CurrentLevel.IsOutOfBounds(position);
    }

    // TODO delete this and use Level.IsOutOfBounds directly
    public bool IsOutOfBounds(float x, float y)
    {
        return CurrentLevel.IsOutOfBounds(x, y);
    }

    // TODO delete this and use Level.IsOutOfBounds directly
    public void SetCharacter(Character character, Vector2 newPosition)
    {
        SetCharacter(character, newPosition.x, newPosition.y);
    }

    // TODO delete this and use Level.IsOutOfBounds directly
    public void SetCharacter(Character character, float x, float y)
    {
        CurrentLevel.SetCharacter(character, x, y);
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