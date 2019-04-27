using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : ScriptableObject
{
    /// <summary>
    /// The individual sprite instance of this Character instance.
    /// </summary>
    private Transform _sprite;

    /// <summary>
    /// The prefab to be copied for _sprite.
    /// </summary>
    protected Transform _spritePrefab;
    private Player _player;
    public readonly List<Item> Items = new List<Item>();

    protected bool _hasMoved = true;
    protected int _moves;
    
    protected Character(Transform spritePrefab, int moves) {
        _spritePrefab = spritePrefab;
        _moves = moves;
    }

    /// <summary>
    /// Basic creator
    /// </summary>
    /// <typeparam name="CharacterType"></typeparam>
    /// <param name="player"></param>
    /// <returns></returns>
    public static CharacterType CreateInstance<CharacterType>  (Player player) where CharacterType : Character
    {
        CharacterType character = CreateInstance<CharacterType>();
        character._player = player;
        return character;
    }

    /// <summary>
    /// Calculate ranges based on the character's items.
    /// 
    /// TODO: Make sure the character can actually use the item.
    /// </summary>
    /// <returns></returns>
    public HashSet<int> GetRanges()
    {
        HashSet<int> ranges = new HashSet<int>();

        foreach(Item item in Items)
        {
            if(item is Weapon)
            {
                Weapon weapon = (Weapon) item;
                ranges.UnionWith(weapon.Ranges);
            }
            
        }
        return ranges;
    }

    /// <summary>
    /// Initially draws the character. Should only be called once.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public virtual void DrawCharacter(float x, float y)
    {
        Sprite = Instantiate(_spritePrefab, new Vector3(x, y), Quaternion.identity);
    }

    /// <summary>
    /// Move the character to the newPosition over several frames.
    /// </summary>
    /// <param name="newPosition"></param>
    /// <returns></returns>
    public virtual IEnumerator Move(Vector3 newPosition)
    {
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * CursorControl.cursorSpeed;
            Sprite.position = Vector3.Lerp(Sprite.position, newPosition , time);
            yield return null;
        }
    }

    public bool HasMoved
    {
        get
        {
            return _hasMoved;
        }

        set
        {
            _hasMoved = value;
        }
    }

    public int Moves
    {
        get
        {
            return _moves;
        }
        protected set
        {
            _moves = value;
        }
    }

    public Player Player
    {
        get
        {
            return _player;
        }

        set
        {
            _player = value;
            _player.Characters.Add(this);
        }
    }

    public Transform Sprite
    {
        get
        {
            return _sprite;
        }

        set
        {
            _sprite = value;
        }
    }
}