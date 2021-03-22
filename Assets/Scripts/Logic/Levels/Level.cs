using System.Collections.Generic;
using Characters;
using UnityEngine;

namespace Logic.Levels
{
    public abstract class Level : ManagedScriptableObject
    {
        public Character[,] CharacterMap;
        public Terrain.Terrain[,] TerrainMap;
        public Player HumanPlayer;
        public Player AiPlayer;

        public Vector2 StartPosition;

        public static readonly string CharacterColorTag = "CharacterColor";

        /// <summary>
        /// Draw the character at the specified position
        /// </summary>
        /// <param name="character"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void DrawCharacter(Character character, int x, int y)
        {
            SpriteRenderer spriteRenderer = GameManager.FindComponentInChildWithTag<SpriteRenderer>(character.gameObject, CharacterColorTag);
            Debug.LogFormat("Sprite Renderer: {0}", spriteRenderer);

            if (spriteRenderer == null)
            {
                Debug.LogErrorFormat("Character {0} at {1} does not have sprite renderer with tag '{2}'", character.name, new Vector2(x, y), CharacterColorTag);
                return;
            }

            spriteRenderer.color = character.Player.Color;
            character.transform.position = new Vector2(x, y);
        }

        public void AddCharacter(Character character, int x, int y)
        {
            if (CharacterMap[x, y] != null)
            {
                Debug.LogErrorFormat("Character, {0}, already exists at ({1}, {2})", character.CharacterName, x, y);
                return;
            }

            CharacterMap[x, y] = character;
            DrawCharacter(character, x, y);
        }

        public void Init(GameManager gameManager, Player humanPlayer, Player aiPlayer)
        {
            GameManager = gameManager;
            HumanPlayer = humanPlayer;
            AiPlayer = aiPlayer;

            Init();

            if (TerrainMap.GetLength(0) != CharacterMap.GetLength(0) || TerrainMap.GetLength(1) != CharacterMap.GetLength(1))
            {
                Debug.LogErrorFormat("Character map and terrain map must be the same dimensions");
                return;
            }

            for (int x = 0; x < TerrainMap.GetLength(0); x++)
            {
                for (int y = 0; y < TerrainMap.GetLength(1); y++)
                {
                    TerrainMap[x, y].transform.position = new Vector2(x, y);

                    Character character = CharacterMap[x, y];
                    if (character != null)
                    {
                        DrawCharacter(character, x, y);
                    }
                }
            }

            GameManager.Cursor.transform.position = StartPosition;
        }

        /// <summary>
        /// Check the board to see if the position is out of bounds.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsOutOfBounds(float x, float y)
        {
            return x < 0f || x >= CharacterMap.GetLength(0) || y < 0f || y >= CharacterMap.GetLength(1);
        }

        /// <summary>
        /// Check the board to see if the position is out of bounds.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsOutOfBounds(Vector2 position)
        {
            return IsOutOfBounds(position.x, position.y);
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

            return CharacterMap[(int)x, (int)y];
        }

        public Character GetCharacter()
        {
            return GetCharacter(GameManager.Cursor.transform.position);
        }

        public Character GetCharacter(Vector2 position)
        {
            return GetCharacter(position.x, position.y);
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
                Debug.LogErrorFormat("Position is out of bounds: ({0},{1})", x, y);
                return;
            }

            CharacterMap[(int)x, (int)y] = character;
            if (character != null)
            {
                character.transform.position = new Vector2(x, y);
            }
        }

        /// <summary>
        /// Set the character in the map at the specified position.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="position"></param>
        public void SetCharacter(Character character, Vector2 position)
        {
            SetCharacter(character, position.x, position.y);
        }

        /// <summary>
        /// Get the Terrain at the specified position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Terrain.Terrain GetTerrain(float x, float y)
        {
            if (IsOutOfBounds(x, y))
            {
                return null;
            }
            return TerrainMap[(int)x, (int)y];
        }

        public Terrain.Terrain GetTerrain(Vector2 position)
        {
            return GetTerrain(position.x, position.y);
        }

        public C Create<C>(Transform original) where C : MonoBehaviour
        {
            Component copy = Instantiate(original, GameManager.transform);
            return copy.GetComponent<C>();
        }

        /// <summary>
        /// Create the terrain and character maps.
        /// The GameManager, HumanPlayer, and AiPlayer have all been initialized before this method is called.
        /// </summary>
        protected abstract void Init();

        /// <summary>
        /// Whether or not the level is complete
        /// </summary>
        /// <returns>True if the level is over. False if not.</returns>
        public abstract bool IsLevelOver();

        public ICollection<Character> GetCharacters()
        {
            List<Character> characters = new List<Character>();
            for (int x = 0; x < CharacterMap.GetLength(0); x++)
            {
                for (int y = 0; y < CharacterMap.GetLength(1); y++)
                {
                    Character character = CharacterMap[x, y];
                    if (character != null)
                    {
                        characters.Add(character);
                    }
                }
            }
            Debug.LogFormat("Characters found: {0}", characters.Count);
            return characters;
        }

        public void Kill(Character character)
        {
            SetCharacter(null, character.transform.position);
        }

        public void SetTerrain(Transform transform, float x, float y)
        {
            if (IsOutOfBounds(x, y))
            {
                Debug.LogErrorFormat("Position is out of bounds: ({0},{1})", x, y);
                return;
            }

            Terrain.Terrain terrain = TerrainMap[(int)x, (int)y];
            if (terrain)
            {
                Destroy(terrain.gameObject);
            }

            TerrainMap[(int)x, (int)y] = Create<Terrain.Terrain>(transform);
        }

        public void MoveCharacter(Character character, Vector2 newPosition)
        {
            Vector2 oldPosition = character.transform.position;
            Debug.LogFormat("Moving {0} from {1} to {2}", character.CharacterName, oldPosition, newPosition);

            if (character.transform.position.x == newPosition.x && character.transform.position.y == newPosition.y)
            {
                return;
            }

            if (GameManager.CurrentLevel.GetCharacter(newPosition) != null)
            {
                Debug.LogErrorFormat("Position is already taken: {0}", newPosition);
            }
            GameManager.CurrentLevel.SetCharacter(character, newPosition);

            if (newPosition.x != oldPosition.x || newPosition.y != oldPosition.y)
            {
                GameManager.CurrentLevel.SetCharacter(null, oldPosition);
            }
        }
    }
}