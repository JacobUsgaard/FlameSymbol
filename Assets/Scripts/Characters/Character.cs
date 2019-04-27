using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// TODO weapon proficiencies
/// </summary>
public abstract class Character : MonoBehaviour
{
    private Player player;
    public List<Item> Items = new List<Item>();

    public readonly List<Transform> MovableSpaces = new List<Transform>();
    public readonly List<Transform> AttackableSpaces = new List<Transform>();

    public string CharacterName;
    public int Strength;
    public int Magic;
    public int Skill;
    public int Resistance;
    public int Defense;
    public int Speed;
    public int MaxHp;
    public int CurrentHp;
    public int Moves;
    public int Level;
    public int Experience;

    // TODO do these better
    public Weapon.Proficiency SwordProficiency;
    public Weapon.Proficiency FireMagicProficiency;
    public Weapon.Proficiency AxeProficiency;

    public GameManager GameManager;

    public System.Random Random = new System.Random();

    public Player Player
    {
        get
        {
            return player;
        }

        set
        {
            player = value;
            player.Characters.Add(this);
        }
    }

    public void Start()
    {
        GameManager = GetComponentInParent<GameManager>();
    }

    /// <summary>
    /// Calculate ranges based on the character's items.
    /// </summary>
    /// <returns></returns>
    public HashSet<int> GetWeaponRanges()
    {
        HashSet<int> ranges = new HashSet<int>();
        Debug.Log("Items: " + Items.Count);

        foreach(Item item in Items)
        {
            if(item is Weapon)
            {
                Weapon weapon = (Weapon)item;

                if (IsProficient(weapon))
                {
                    ranges.UnionWith(weapon.Ranges);
                }
            }
        }

        return ranges;
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public HashSet<Vector2> CalculateMovablePositions()
    {
        return CalculateMovablePositions(transform.position.x, transform.position.y, Moves);
    }

    private HashSet<Vector2> CalculateMovablePositions(float x, float y, int moves)
    {
        HashSet<Vector2> movableSpaces = new HashSet<Vector2>();
        Character character = GameManager.GetCharacter(x, y);
        if (moves == 0 || GameManager.IsOutOfBounds(x, y) || (character != null && !character.Player.Equals(Player)))
        {
            return movableSpaces;
        }

        movableSpaces.Add(new Vector2(x, y));

        movableSpaces.UnionWith(CalculateMovablePositions(x - 1, y, moves - 1));
        movableSpaces.UnionWith(CalculateMovablePositions(x + 1, y, moves - 1));
        movableSpaces.UnionWith(CalculateMovablePositions(x, y - 1, moves - 1));
        movableSpaces.UnionWith(CalculateMovablePositions(x, y + 1, moves - 1));
        return movableSpaces;
    }

    public List<Transform> CreateMovableTransforms()
    {
        return CreateMovableTransforms(CalculateMovablePositions());
    }

    public List<Transform> CreateMovableTransforms(ICollection<Vector2> movablePositions)
    {
        foreach (Vector2 movablePosition in movablePositions)
        {
            MovableSpaces.Add(Instantiate(GameManager.MovableSpacePrefab, new Vector2(movablePosition.x, movablePosition.y), Quaternion.identity, GameManager.transform));
        }

        return MovableSpaces;
    }

    public HashSet<Vector2> CalculateAttackablePositions()
    {
        HashSet<Vector2> attackablePositions = new HashSet<Vector2>();
        HashSet<Vector2> movablePositions = CalculateMovablePositions();

        HashSet<int> ranges =  GetWeaponRanges();

        foreach(Vector2 movablePosition in movablePositions)
        {
            attackablePositions.UnionWith(CalculateAttackablePositions(movablePosition.x, movablePosition.y, ranges));
        }

        return attackablePositions;
    }

    public HashSet<Vector2> CalculateAttackablePositions(float x, float y, HashSet<int> ranges)
    {
        HashSet<Vector2> attackablePositions = new HashSet<Vector2>();
        foreach (int range in ranges)
        {
            attackablePositions.UnionWith(CalculateAttackablePositions(x, y, range));
        }
        return attackablePositions;
    }

    public HashSet<Vector2> CalculateAttackablePositions(float x, float y, int range)
    {
        HashSet<Vector2> attackablePositions = new HashSet<Vector2>();
        if (range < 0 || GameManager.IsOutOfBounds(x, y))
        {
            return attackablePositions;
        }

        if (range == 0)
        {
            attackablePositions.Add(new Vector2(x, y));
            return attackablePositions;
        }

        attackablePositions.UnionWith(CalculateAttackablePositions(x + 1, y, range - 1));
        attackablePositions.UnionWith(CalculateAttackablePositions(x - 1, y, range - 1));
        attackablePositions.UnionWith(CalculateAttackablePositions(x, y + 1, range - 1));
        attackablePositions.UnionWith(CalculateAttackablePositions(x, y - 1, range - 1));

        return attackablePositions;
    }

    public List<Transform> CreateAttackableTransforms()
    {
        HashSet<Vector2> movablePositions = CalculateMovablePositions();
        CreateMovableTransforms(movablePositions);
        
        return CreateAttackableTransforms(CalculateAttackablePositions(), movablePositions);
    }

    public List<Transform> CreateAttackableTransforms(ICollection<Vector2> attackablePositions, ICollection<Vector2> movablePositions = null)
    {
        movablePositions = movablePositions ?? new HashSet<Vector2>();
        foreach (Vector2 attackablePosition in attackablePositions)
        {
            if (movablePositions.Contains(attackablePosition))
            {
                continue;
            }

            AttackableSpaces.Add(Instantiate(GameManager.AttackableSpacePrefab, new Vector2(attackablePosition.x, attackablePosition.y), Quaternion.identity, GameManager.transform));
        }

        return AttackableSpaces;
    }

    public void Move(Vector2 position)
    {
        if(transform.position.x == position.x && transform.position.y == position.y)
        {
            return;
        }

        if (GameManager.GetCharacter(position) != null)
        {
            Debug.LogError("Position is already taken: " + position);
        }
        Vector2 oldPosition = transform.position;
        GameManager.SetCharacter(this, position);

        if (position.x != oldPosition.x || position.y != oldPosition.y)
        {
            GameManager.SetCharacter(null, oldPosition);
        }
    }

    public void DestroyAttackableTransforms()
    {
        foreach (Transform attackableSpace in AttackableSpaces)
        {
            Destroy(attackableSpace.gameObject);
        }
        AttackableSpaces.Clear();
    }

    public void DestroyMovableTransforms()
    {
        foreach (Transform movableSpace in MovableSpaces)
        {
            Destroy(movableSpace.gameObject);
        }

        MovableSpaces.Clear();
    }

    public void DestroyMovableAndAttackableSpaces()
    {
        DestroyMovableTransforms();
        DestroyAttackableTransforms();
    }

    public List<Transform> CalculateTradableSpaces()
    {
        List<Transform> tradableSpaces = new List<Transform>();
        float y = transform.position.x;
        float x = transform.position.y;

        CalculateTradableSpace(x - 1, y, tradableSpaces);
        CalculateTradableSpace(x + 1, y, tradableSpaces);
        CalculateTradableSpace(x, y - 1, tradableSpaces);
        CalculateTradableSpace(x, y + 1, tradableSpaces);

        return tradableSpaces;
    }

    private void CalculateTradableSpace(float x, float y, List<Transform> tradableSpaces)
    {
        Character character = GameManager.GetCharacter(x, y);
        if (character != null && character.Player.Equals(Player))
        {
            tradableSpaces.Add(Instantiate(GameManager.AttackableSpacePrefab, new Vector2(x, y), Quaternion.identity, GameManager.transform));
        }
    }

    public List<Transform> GetAttackableSpacesWithCharacters()
    {
        DestroyAttackableTransforms();
        HashSet<int> ranges = GetWeaponRanges();

        foreach (int range in ranges)
        {
            CreateAttackableTransforms(CalculateAttackablePositions(transform.position.x, transform.position.y, range));
        }

        AttackableSpaces.RemoveAll(attackableSpace => {
            Character defendingCharacter = GameManager.GetCharacter(attackableSpace.position);

            if (defendingCharacter == null || defendingCharacter.Player.Equals(Player))
            {
                Destroy(attackableSpace.gameObject);
                return true;
            }

            return false;
        });

        return AttackableSpaces;
    }

    /// <summary>
    /// Equip an item from the Character's inventory.
    /// // TODO make sure this item actually exists in their inventory
    /// </summary>
    /// <param name="item"></param>
    public void Equip(Item item)
    {
        if (Items.Count > 1)
        {
            Items.Remove(item);
            Items.Insert(0, item);
        }
    }

    /// <summary>
    /// Called when a Character dies
    /// </summary>
    public void Die()
    {
        Debug.Log("Die: " + this);
        Destroy(gameObject);
    }

    // TODO make hits actually based off of hit percentage
    // TODO make criticals a thing
    // TODO make speed a thing
    public void Attack(Character defenseCharacter)
    {
        AttackInfo attackInfo = CalculateAttackInfo(defenseCharacter);

        int attackHitPercentage = Random.Next(100);

        int attackExperience = 1;
        int defenseExperience = 0;

        if (attackHitPercentage <= attackInfo.AttackHitPercentage)
        {
            if (attackInfo.AttackDamage != 0)
            {
                attackExperience += 9;
                defenseCharacter.CurrentHp = Mathf.Max(0, defenseCharacter.CurrentHp - attackInfo.AttackDamage);
            }
        }
        
        if (defenseCharacter.CurrentHp == 0)
        {
            attackExperience += 10;
            defenseCharacter.Die();
        }
        else
        {
            defenseExperience += 1;

            int defenseHitPercentage = Random.Next(100);
            if(defenseHitPercentage <= attackInfo.DefenseHitPercentage)
            {
                if (attackInfo.DefenseDamage != 0)
                {
                    defenseExperience += 9;
                    CurrentHp = Mathf.Max(0, CurrentHp - attackInfo.DefenseDamage);
                }
            }
            
            if (CurrentHp == 0)
            {
                defenseExperience += 10;
                Die();
            }
        }

        Experience += attackExperience;
        defenseCharacter.Experience += defenseExperience;
    }

    public AttackInfo CalculateAttackInfo(Character defenseCharacter)
    {
        // TODO check if weapon is usable
        Weapon attackWeapon = GetUsableWeapon();/* (Weapon)Items[0];*/
        Debug.Log("Attack weapon: " + attackWeapon);

        Weapon defenseWeapon = null ;
        List<Weapon> usableWeapons = defenseCharacter.GetUsableWeapons();
        if(usableWeapons.Count > 0)
        {
            defenseWeapon = usableWeapons[0];
        }
        
        if(defenseWeapon != null) {
            Debug.Log("Defense weapon: " + defenseWeapon);

            if(!defenseWeapon.IsInRange(defenseCharacter.GetPosition(), GetPosition()))
            {
                Debug.Log("Weapon is not in range");
                defenseWeapon = null;
            }
        }

        int attackHitPercentage = CalculateHitPercentage(this, attackWeapon, defenseCharacter);
        int attackDamage = CalculateDamage(this, attackWeapon, defenseCharacter);
        int attackCriticalPercentage = CalculateCriticalPercentage(this, attackWeapon, defenseCharacter);

        int defenseHitPercentage = 0;
        int defenseDamage = 0;
        int defenseCriticalPercentage = 0;
        if (defenseWeapon != null)
        {
            defenseHitPercentage = CalculateHitPercentage(defenseCharacter, defenseWeapon, this);
            defenseDamage = CalculateDamage(defenseCharacter, defenseWeapon, this);
            defenseCriticalPercentage = CalculateCriticalPercentage(defenseCharacter, defenseWeapon, this);
        }

        return new AttackInfo(attackWeapon, attackHitPercentage, attackDamage, attackCriticalPercentage, defenseWeapon, defenseHitPercentage, defenseDamage, defenseCriticalPercentage);
    }

    private int CalculateHitPercentage(Character attackCharacter, Weapon attackWeapon, Character defenseCharacter)
    {
        return Mathf.Clamp(attackCharacter.Skill + attackWeapon.HitPercentage - defenseCharacter.Speed, 0, 100);
    }

    private int CalculateDamage(Character attackCharacter, Weapon attackWeapon, Character defenseCharacter)
    {
        int damage = attackWeapon.CalculateDamage(defenseCharacter);
        if(attackWeapon is StrengthWeapon)
        {
            damage += attackCharacter.Strength - defenseCharacter.Defense;
        }else if(attackWeapon is MagicWeapon)
        {
            damage += attackCharacter.Magic - defenseCharacter.Resistance;
        }
        else
        {
            Debug.LogError("Unknown weapon type");
        }

        return Mathf.Max(damage, 0);
    }

    private int CalculateCriticalPercentage(Character attackCharacter, Weapon attackWeapon, Character defenseCharacter)
    {
        return Mathf.Clamp((attackCharacter.Skill / 4) + attackWeapon.CriticalPercentage, 0, 100);
    }

    public List<Weapon> GetUsableWeapons()
    {
        List<Weapon> weapons = new List<Weapon>();
        foreach(Item item in Items)
        {
            if(item is Weapon)
            {
                Weapon weapon = (Weapon)item;
                if (IsProficient(weapon))
                {
                    weapons.Add(weapon);
                }
            }
        }

        return weapons;
    }

    public bool IsProficient(Weapon weapon)
    {
        Weapon.Proficiency requiredProficiency = weapon.RequiredProficiency;
        Weapon.Proficiency proficiency;
        if (weapon is Sword)
        {
            proficiency = SwordProficiency;
        }
        else if (weapon is FireMagic)
        {
            proficiency = FireMagicProficiency;
        }
        else if (weapon is Axe)
        {
            proficiency = AxeProficiency;
        }
        else
        {
            return false;
        }

        return requiredProficiency <= proficiency;
    }

    public Weapon GetUsableWeapon()
    {
        List<Weapon> weapons = GetUsableWeapons();
        if(weapons.Count > 0)
        {
            return weapons[0];
        }

        return null;
    }

    public class AttackInfo
    {
        public Weapon AttackWeapon;
        public int AttackHitPercentage;
        public int AttackDamage;
        public int AttackCriticalPercentage;

        public Weapon DefenseWeapon;
        public int DefenseHitPercentage;
        public int DefenseDamage;
        public int DefenseCriticalPercentage;

        public AttackInfo(Weapon attackWeapon, int attackHitPercentage, int attackDamage, int attackCriticalPercentage, Weapon defenseWeapon, int defenseHitPercentage, int defenseDamage, int defenseCriticalPercentage)
        {
            AttackWeapon = attackWeapon;
            AttackHitPercentage = attackHitPercentage;
            AttackDamage = attackDamage;
            AttackCriticalPercentage = attackCriticalPercentage;

            DefenseWeapon = defenseWeapon;
            DefenseHitPercentage = defenseHitPercentage;
            DefenseDamage = defenseDamage;
            DefenseCriticalPercentage = defenseCriticalPercentage;
        }
    }
}