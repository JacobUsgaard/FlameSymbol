using System.Collections.Generic;
using Characters;
using Items.Weapons.Attackable;
using Logic.Levels;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Logic
{

    /// <summary>
    /// Handles all game play aspects.
    /// 
    /// Initializes/loads game. Maintains all prefabs including menus, terrains, and UI texts. Directs input to correct handler.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static readonly string SceneNameMainMenu = "MainMenu";
        public static readonly string SceneNameFlameSymbol = "FlameSymbol";

        [Header("Characters")]
        public Transform KnightPrefab;
        public Transform WizardPrefab;

        [Header("Terrain")]
        public Transform ForrestTerrain;
        public Transform GrassTerrain;
        public Transform WaterTerrain;
        public Transform WallTerrain;

        [Header("Text")]
        public Text AssistTextPrefab;
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
        public Transform AssistableTransformPrefab;
        public AssistDetailPanel AssistDetailPanel;
        public Transform AttackableSpacePrefab;
        public AttackDetailPanel AttackDetailPanel;
        public Menu CharacterActionMenu;
        public CharacterInformationPanel CharacterInformationPanel;
        public UI.Cursor Cursor;

        /// <summary>
        /// Menu for actions to perform using a specific item
        /// </summary>
        public Menu ItemActionMenu;

        /// <summary>
        /// Menu for list of items after 'Items' is selected
        /// </summary>
        public Menu ItemDetailMenu;

        /// <summary>
        /// Menu for list of items after 'Attack' or 'Assist' is selected
        /// </summary>
        public Menu ItemSelectionMenu;
        public Transform MovableSpacePrefab;
        public Transform PathCornerPrefab;
        public Transform PathEndPrefab;
        public Transform PathStraightPrefab;
        public Menu PlayerActionMenu;
        public TerrainInformationPanel TerrainInformationPanel;
        public TradeDetailPanel TradeDetailPanel;

        [Header("Weapons")]
        public Text AxeTextPrefab;
        public Text FireTextPrefab;
        public Text HealTextPrefab;
        public Text IronAxeTextPrefab;
        public Text IronLanceTextPrefab;
        public Text IronSwordTextPrefab;
        public Text LanceTextPrefab;
        public Text SwordTextPrefab;

        [Header("Level")]
        public Level _currentLevel;

        public System.Random Random = new System.Random();
        private Player HumanPlayer;
        private AIPlayer AiPlayer;

        public Player CurrentPlayer { get; set; }
        public List<Player> Players { get; } = new List<Player>();

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
            Debug.Log("Starting GameManager");
            DontDestroyOnLoad(transform.gameObject);

            // Initialize both managed objects
            ManagedMonoBehavior.Initialize(this);
            ManagedScriptableObject.Initialize(this);

            HumanPlayer = ScriptableObject.CreateInstance<Player>();
            HumanPlayer.Color = Color.blue;
            HumanPlayer.IsHuman = true;
            Players.Add(HumanPlayer);

            AiPlayer = ScriptableObject.CreateInstance<AIPlayer>();
            AiPlayer.Color = Color.red;
            AiPlayer.IsHuman = false;
            Players.Add(AiPlayer);

            CurrentLevel = ScriptableObject.CreateInstance<TestLevel>();
            CurrentLevel.Init(this, HumanPlayer, AiPlayer);

            Cursor.transform.position = CurrentLevel.StartPosition;

            CurrentPlayer = HumanPlayer;
            Cursor.Focus();
            Cursor.CurrentState = UI.Cursor.State.Free;
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

            if (CurrentPlayer.HaveAllCharactersMoved())
            {
                Debug.LogFormat("All characters have moved");
                EndTurn();
                return;
            }

            if (CurrentPlayer.IsHuman)
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
                    HashSet<Vector2> attackablePositions = character.CalculateAttackablePositions(movablePosition.x, movablePosition.y, character.CalculateRanges<Attackable>());
                    foreach (Vector2 attackablePosition in attackablePositions)
                    {
                        Character defendingCharacter = CurrentLevel.GetCharacter(attackablePosition);
                        if (defendingCharacter == null || defendingCharacter.Player.Equals(character.Player))
                        {
                            continue;
                        }

                        character.CompleteAttack(defendingCharacter);
                        goto EndTurn;
                    }
                }
            }

        EndTurn:
            CurrentPlayer = HumanPlayer;
            Cursor.Focus();
        }

        public void HandleInput()
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

            if (Input.GetButtonDown("Fire3"))
            {
                Debug.Log("Fire3");
                currentFocusableObject.OnInformation();
            }

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (horizontal != 0f || vertical != 0f)
            {
                Debug.Log("Arrow");
                FocusableObject.CurrentObject.OnArrow(horizontal, vertical);
                System.Threading.Thread.Sleep(50);
            }
        }

        public void ShowPlayerActionMenu()
        {
            PlayerActionMenu.AddMenuItem(null, EndTurnTextPrefab, delegate (Object[] objects)
            {
                Debug.Log("Ending turn");
                EndTurn();
                PlayerActionMenu.Hide();
                Cursor.CurrentState = UI.Cursor.State.Free;
                Cursor.Focus();
            });
            PlayerActionMenu.Show(PlayerActionMenuOnCancel);
            PlayerActionMenu.Focus();
        }

        public void PlayerActionMenuOnCancel(object[] objects)
        {
            Debug.Log("PlayerActionMenuOnCancel");
            PlayerActionMenu.Hide();
            Cursor.Focus();
        }

        public void EndTurn()
        {
            EndTurn(CurrentPlayer);
        }

        public void EndTurn(Player player)
        {
            if (!player.Equals(CurrentPlayer))
            {
                Debug.LogErrorFormat("Player that is not current player is ending their turn. Current player: {0}, calling player: {1}", CurrentPlayer, player);
                return;
            }

            CurrentPlayer = CalculateNextPlayer();
        }

        public static void DestroyAll(ICollection<Transform> transforms = default(List<Transform>))
        {
            foreach (Transform transform in transforms)
            {
                Destroy(transform.gameObject);
            }

            transforms.Clear();
        }

        public static T FindComponentInChildWithTag<T>(GameObject parent, string tag) where T : Component
        {
            Debug.LogFormat("FindComponentInChildWithTag: {0}, {1}", parent.name, tag);

            foreach (Transform transform in parent.transform)
            {
                Debug.LogFormat("Transform: {0}", transform.name);
                if (transform.CompareTag(tag))
                {
                    return transform.GetComponent<T>();
                }
            }
            return null;
        }

        public Player CalculateNextPlayer()
        {
            int currentIndex = Players.IndexOf(CurrentPlayer);
            if (currentIndex == -1)
            {
                Debug.LogErrorFormat("Unable to find player: {0}", CurrentPlayer.Name);
                return null;
            }

            return Players[(Players.Count + 1) % Players.Count];
        }
    }
}