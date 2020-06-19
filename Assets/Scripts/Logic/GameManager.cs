using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles are game play aspects.
/// 
/// Initializes/loads game. Maintains all prefabs including menus, terrains, and UI texts. Directs input to correct handler.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Characters")]
    public Transform KnightPrefab;
    public Transform WizardPrefab;

    [Header("Terrain")]
    public Transform ForrestTerrain;
    public Transform GrassTerrain;
    public Transform WaterTerrain;
    public Transform WallTerrain;

    [Header("Text")]
    public Text AttackTextPrefab;
    public Text DropTextPrefab;
    public Text EndTurnTextPrefab;
    public Text EquipTextPrefab;
    public Text IncinerateTextPrefab;
    public Text InfoTextPrefab;
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
    public TerrainInformationPanel TerrainInformationPanel;
    public TradeDetailPanel TradeDetailPanel;
   
    [Header("Weapons")]
    public Text FireTextPrefab;
    public Text IronAxeTextPrefab;
    public Text IronSwordTextPrefab;
    public Text IronLanceTextPrefab;
    public Text AxeTextPrefab;
    public Text LanceTextPrefab;
    public Text SwordTextPrefab;

    [Header("Level")]
    public Level _currentLevel;

    public System.Random Random = new System.Random();
    private bool IsHumanTurn = false;
    private Player currentPlayer;
    private Player HumanPlayer;
    private AIPlayer AiPlayer;

    public Level CurrentLevel
    {
        get
        {
            return _currentLevel;
        }

        private set
        {
            _currentLevel = value;
        }
    }

    public void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        ManagedMonoBehavior.Initialize(this);
        ManagedScriptableObject.Initialize(this);

        HumanPlayer = ScriptableObject.CreateInstance<Player>();
        AiPlayer = ScriptableObject.CreateInstance<AIPlayer>();

        CurrentLevel = ScriptableObject.CreateInstance<TestLevel>();
        CurrentLevel.Init(this, HumanPlayer, AiPlayer);

        Cursor.transform.position = CurrentLevel.StartPosition;

        CurrentPlayer = HumanPlayer;
        IsHumanTurn = true;
        Cursor.Focus();
        Cursor.CurrentState = Cursor.State.Free;

        //CurrentPlayer = AiPlayer;
    }

    public delegate void Callback(params Object[] objects);

    void Update()
    {
        if (CurrentLevel.IsLevelOver())
        {
            Debug.Log("Level is over");
            SceneManager.LoadScene("Resources/Scenes/MainMenu");
            return;
        }

        if (IsHumanTurn)
        {
            HandleInput();
            return;
        }

        Debug.Log("AI time");
        CurrentPlayer = AiPlayer;
        foreach (Character character in AiPlayer.Characters)
        {
            Debug.Log("Attacking character: " + character.transform.position);

            foreach (Vector2 movablePosition in character.CalculateMovablePositions())
            {
                if (CurrentLevel.GetCharacter(movablePosition) != null)
                {
                    continue;
                }

                character.Move(movablePosition);
                HashSet<Vector2> attackablePositions = character.CalculateAttackablePositions(movablePosition.x, movablePosition.y, character.GetWeaponRanges());
                foreach (Vector2 attackablePosition in attackablePositions)
                {
                    Character defendingCharacter = CurrentLevel.GetCharacter(attackablePosition);
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
        }

        if (Input.GetButtonDown("Cancel"))
        {
            Debug.Log("Cancel");
            currentFocusableObject.OnCancel();
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0f || vertical != 0f)
        {
            Debug.Log("Arrow");
            FocusableObject.CurrentObject.OnArrow(horizontal, vertical);
            System.Threading.Thread.Sleep(50);
        }

        Vector3 mousePosition = Input.mousePosition;
        if(mousePosition.x < 0 || mousePosition.x > Screen.width || mousePosition.y < 0 || mousePosition.y > Screen.height)
        {
            return;
            //Debug.Log("Mouse outside window");
        }

        Vector2 mousePositionWorldPoint = TranslateMousePosition(mousePosition);
        //Debug.LogFormat("Mouse position in world: {0}", mousePositionWorldPoint);

        if (Input.GetMouseButtonDown(1))
        {
            Debug.LogFormat("Right click: {0}", mousePositionWorldPoint);
            currentFocusableObject.OnRightMouse(mousePositionWorldPoint);
        }
    }

    public Vector2 TranslateMousePosition(Vector3 mousePosition)
    {
        Vector2 vector2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        vector2.x += 0.5f;
        vector2.y += 0.5f;
                
        return vector2;
    }

    public void ShowPlayerActionMenu()
    {
        PlayerActionMenu.AddMenuItem(null, EndTurnTextPrefab, delegate (Object[] objects)
        {
            Debug.Log("Ending turn");
            EndTurn();
            PlayerActionMenu.Hide();
            Cursor.CurrentState = Cursor.State.Free;
            Cursor.Focus();
        });
        PlayerActionMenu.Show(delegate (Object[] objects)
        {
            PlayerActionMenu.Hide();
            Cursor.Focus();
        });
        PlayerActionMenu.Focus();
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