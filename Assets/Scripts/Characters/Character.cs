using System.Collections.Generic;
using Items;
using Items.Weapons;
using Items.Weapons.Assistable;
using Items.Weapons.Attackable;
using Items.Weapons.Attackable.Strength;
using Logic;
using UnityEngine;

namespace Characters
{

    /// <summary>
    /// TODO weapon Proficiencies
    /// </summary>
    public abstract class Character : ManagedMonoBehavior
    {
        private Player player;
        public List<Item> Items = new List<Item>();

        public readonly List<Transform> MovableTransforms = new List<Transform>();
        public readonly List<Transform> AttackableTransforms = new List<Transform>();
        public readonly List<Transform> AssistableTransforms = new List<Transform>();
        public readonly List<Transform> TradableTransforms = new List<Transform>();

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
        public bool IsFlyer = false;

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

        public List<Proficiency> Proficiencies = new List<Proficiency>();

        public void AddProficiency(Proficiency proficiency)
        {
            foreach (Proficiency p in Proficiencies)
            {
                if (p.Type.Equals(proficiency.Type))
                {
                    Proficiencies.Remove(p);
                    break;
                }
            }

            Proficiencies.Add(proficiency);
        }

        public HashSet<int> CalculateRanges<T>() where T : Weapon
        {
            HashSet<int> ranges = new HashSet<int>();

            foreach (T weapon in GetUsableItems<T>())
            {
                if (IsProficient(weapon))
                {
                    ranges.UnionWith(weapon.Ranges);
                }
            }

            return ranges;
        }

        public HashSet<Vector2> CalculateMovablePositions()
        {
            return CalculateMovablePositions(transform.position.x, transform.position.y, Moves);
        }

        private HashSet<Vector2> CalculateMovablePositions(float x, float y, int remainingMoves)
        {
            HashSet<Vector2> movableSpaces = new HashSet<Vector2>();
            Character character = GameManager.CurrentLevel.GetCharacter(x, y);
            Terrain.Terrain terrain = GameManager.CurrentLevel.GetTerrain(x, y);
            if (remainingMoves <= 0 || GameManager.CurrentLevel.IsOutOfBounds(x, y) || !terrain.IsPassable(this, x, y) || (character != null && !character.Player.Equals(Player)))
            {
                return movableSpaces;
            }

            Vector2 position = new Vector2(x, y);
            remainingMoves -= CalculateMovementCost(position);

            _ = movableSpaces.Add(position);

            movableSpaces.UnionWith(CalculateMovablePositions(x - 1, y, remainingMoves));
            movableSpaces.UnionWith(CalculateMovablePositions(x + 1, y, remainingMoves));
            movableSpaces.UnionWith(CalculateMovablePositions(x, y - 1, remainingMoves));
            movableSpaces.UnionWith(CalculateMovablePositions(x, y + 1, remainingMoves));
            return movableSpaces;
        }

        public void Heal(int hp)
        {
            CurrentHp = Mathf.Clamp(CurrentHp + hp, 0, MaxHp);
        }

        /// <summary>
        /// Creates the movable transforms from the collection of movable positions
        /// </summary>
        /// <param name="movablePositions"></param>
        /// <returns></returns>
        public List<Transform> CreateMovableTransforms(ICollection<Vector2> movablePositions)
        {
            foreach (Vector2 movablePosition in movablePositions)
            {
                MovableTransforms.Add(Instantiate(GameManager.MovableSpacePrefab, new Vector2(movablePosition.x, movablePosition.y), Quaternion.identity, GameManager.transform));
            }

            return MovableTransforms;
        }

        public HashSet<Vector2> CalculateAssistablePositions(HashSet<Vector2> movablePositions)
        {
            HashSet<Vector2> staffablePositions = new HashSet<Vector2>();

            HashSet<int> ranges = CalculateRanges<Assistable>();

            foreach (Vector2 movablePosition in movablePositions)
            {
                staffablePositions.UnionWith(CalculateAttackablePositions(movablePosition.x, movablePosition.y, ranges));
            }

            return staffablePositions;
        }

        public HashSet<Vector2> CalculateAssistablePositions()
        {
            return CalculateAssistablePositions(CalculateMovablePositions());
        }

        /// <summary>
        /// Create the assistable transforms from the staffable positions excluding
        /// the provided movable and attackable positions
        /// </summary>
        /// <param name="staffablePositions">The assistable positions to create transforms for</param>
        /// <param name="movablePositions"></param>
        /// <param name="attackablePositions"></param>
        /// <returns></returns>
        public List<Transform> CreateAssistableTransforms(ICollection<Vector2> staffablePositions, HashSet<Vector2> movablePositions = null, HashSet<Vector2> attackablePositions = null)
        {
            movablePositions = movablePositions ?? new HashSet<Vector2>();
            attackablePositions = attackablePositions ?? new HashSet<Vector2>();

            AssistableTransforms.Clear();

            foreach (Vector2 staffablePosition in staffablePositions)
            {
                if (movablePositions.Contains(staffablePosition) || attackablePositions.Contains(staffablePosition))
                {
                    continue;
                }

                AssistableTransforms.Add(Instantiate(GameManager.AssistableTransformPrefab, new Vector2(staffablePosition.x, staffablePosition.y), Quaternion.identity, GameManager.transform));
            }

            return AssistableTransforms;
        }

        /// <summary>
        /// Calculates the attackable positions for all of the character's movable positions.
        /// </summary>
        /// <returns></returns>
        public HashSet<Vector2> CalculateAttackablePositions()
        {
            return CalculateAttackablePositions(CalculateMovablePositions());
        }

        public HashSet<Vector2> CalculateAttackablePositions(HashSet<Vector2> movablePositions)
        {
            HashSet<Vector2> attackablePositions = new HashSet<Vector2>();
            HashSet<int> ranges = CalculateRanges<Attackable>();

            foreach (Vector2 movablePosition in movablePositions)
            {
                attackablePositions.UnionWith(CalculateAttackablePositions(movablePosition.x, movablePosition.y, ranges));
            }

            return attackablePositions;
        }

        /// <summary>
        /// Calculates the attackable positions for the given position and the ranges (e.g. of weapons)
        /// </summary>
        /// <param name="x">The x coordinate of the position</param>
        /// <param name="y">The y coordinate of the position</param>
        /// <param name="ranges">The list of ranges</param>
        /// <returns>The list of attackable positions</returns>
        public HashSet<Vector2> CalculateAttackablePositions(float x, float y, HashSet<int> ranges)
        {
            ranges = ranges ?? CalculateRanges<Attackable>();

            HashSet<Vector2> attackablePositions = new HashSet<Vector2>();
            foreach (int range in ranges)
            {
                attackablePositions.UnionWith(CalculateAttackablePositions(x, y, range));
            }
            return attackablePositions;
        }

        public HashSet<Vector2> CalculateAssistablePositions(float x, float y, HashSet<int> ranges)
        {
            ranges = ranges ?? CalculateRanges<Assistable>();

            HashSet<Vector2> assistablePositions = new HashSet<Vector2>();
            foreach (int range in ranges)
            {
                assistablePositions.UnionWith(CalculateAssistablePositions(x, y, range));
            }
            return assistablePositions;
        }

        /// <summary>
        /// Calculates the attackable positions from the given position using the given range
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        private HashSet<Vector2> CalculateAttackablePositions(float x, float y, int range)
        {
            HashSet<Vector2> attackablePositions = new HashSet<Vector2>();
            if (range < 0 || GameManager.CurrentLevel.IsOutOfBounds(x, y))
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

        private HashSet<Vector2> CalculateAssistablePositions(float x, float y, int range)
        {
            HashSet<Vector2> assistablePositions = new HashSet<Vector2>();
            if (range < 0 || GameManager.CurrentLevel.IsOutOfBounds(x, y))
            {
                return assistablePositions;
            }

            if (range == 0)
            {
                assistablePositions.Add(new Vector2(x, y));
                return assistablePositions;
            }

            assistablePositions.UnionWith(CalculateAssistablePositions(x + 1, y, range - 1));
            assistablePositions.UnionWith(CalculateAssistablePositions(x - 1, y, range - 1));
            assistablePositions.UnionWith(CalculateAssistablePositions(x, y + 1, range - 1));
            assistablePositions.UnionWith(CalculateAssistablePositions(x, y - 1, range - 1));

            return assistablePositions;
        }

        /// <summary>
        /// Calculate tradable spaces with eligible characters for this Character's current position
        /// </summary>
        /// <returns></returns>
        public HashSet<Vector2> CalculateTradablePositionsWithCharacters()
        {
            Debug.LogFormat("CalculateTradablePositionsWithCharacters: {0}", transform.position);
            HashSet<Vector2> tradablePositions = new HashSet<Vector2>();
            float x = transform.position.x;
            float y = transform.position.y;

            tradablePositions.UnionWith(CalculateTradablePositions(x - 1, y));
            tradablePositions.UnionWith(CalculateTradablePositions(x + 1, y));
            tradablePositions.UnionWith(CalculateTradablePositions(x, y - 1));
            tradablePositions.UnionWith(CalculateTradablePositions(x, y + 1));

            Debug.LogFormat("Tradable spaces with characters: {0}", tradablePositions.Count);
            return tradablePositions;
        }

        /// <summary>
        /// Calculate whether or not this position is a tradable position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private HashSet<Vector2> CalculateTradablePositions(float x, float y)
        {
            HashSet<Vector2> tradablePositions = new HashSet<Vector2>();
            Character character = GameManager.CurrentLevel.GetCharacter(x, y);
            if (character != null && character.Player.Equals(Player))
            {
                tradablePositions.Add(new Vector2(x, y));
            }

            return tradablePositions;
        }

        public List<Transform> CreateTradableTransforms(HashSet<Vector2> tradablePositions)
        {
            tradablePositions = tradablePositions ?? CalculateTradablePositions(transform.position.x, transform.position.y);

            foreach (Vector2 tradablePosition in tradablePositions)
            {
                TradableTransforms.Add(Instantiate(GameManager.AssistableTransformPrefab, tradablePosition, Quaternion.identity, GameManager.transform));
            }

            return TradableTransforms;
        }


        public void AddExperience(int experience)
        {
            if (experience > 100)
            {
                Debug.LogErrorFormat("Experience cannot be greater than 100: {0}", experience);
                return;
            }

            Experience += experience;
            if (Experience >= 100)
            {
                LevelUp();
                Experience %= experience;
            }
        }

        public void LevelUp()
        {
            Debug.Log("Level up!");
            Level += 1;
            Strength += GameManager.Random.Next(0, 2);
            Magic += GameManager.Random.Next(0, 2);
            Skill += GameManager.Random.Next(0, 2);
            Resistance += GameManager.Random.Next(0, 2);
            Defense += GameManager.Random.Next(0, 2);
            MaxHp += GameManager.Random.Next(0, 2);
        }

        /// <summary>
        /// Creates the movable, attackable, and assistable transforms.
        ///
        /// Will not delete previous movable and attackable transforms.
        /// </summary>
        public void CreateAttackableTransforms()
        {
            HashSet<Vector2> movablePositions = CalculateMovablePositions();
            Debug.LogFormat("Movable positions: {0}", movablePositions.Count);
            List<Transform> movableTransforms = CreateMovableTransforms(movablePositions);
            Debug.LogFormat("Movable transforms: {0}", movableTransforms.Count);

            HashSet<Vector2> attackablePositions = CalculateAttackablePositions(movablePositions);
            Debug.LogFormat("Attackable positions: {0}", attackablePositions.Count);
            List<Transform> attackableTransforms = CreateAttackableTransforms(attackablePositions, movablePositions);
            Debug.LogFormat("Attackable transforms: {0}", attackableTransforms.Count);

            HashSet<Vector2> staffablePositions = CalculateAssistablePositions(movablePositions);
            Debug.LogFormat("Assistable positions: {0}", staffablePositions.Count);
            List<Transform> staffableTransforms = CreateAssistableTransforms(staffablePositions, movablePositions, attackablePositions);
            Debug.LogFormat("Assistable transforms: {0}", staffableTransforms.Count);
        }

        /// <summary>
        /// Create attackable transforms from the provided attackable positions excluding those already in the provided movable positions
        /// </summary>
        /// <param name="attackablePositions"></param>
        /// <param name="movablePositions"></param>
        /// <returns></returns>
        public List<Transform> CreateAttackableTransforms(ICollection<Vector2> attackablePositions, ICollection<Vector2> movablePositions = null)
        {
            movablePositions = movablePositions ?? new HashSet<Vector2>();
            foreach (Vector2 attackablePosition in attackablePositions)
            {
                if (movablePositions.Contains(attackablePosition))
                {
                    continue;
                }

                AttackableTransforms.Add(Instantiate(GameManager.AttackableSpacePrefab, new Vector2(attackablePosition.x, attackablePosition.y), Quaternion.identity, GameManager.transform));
            }

            return AttackableTransforms;
        }

        /// <summary>
        /// Move the Character to the specified position
        /// </summary>
        /// <param name="position"></param>
        public void Move(Vector2 position)
        {
            if (transform.position.x == position.x && transform.position.y == position.y)
            {
                return;
            }

            if (GameManager.CurrentLevel.GetCharacter(position) != null)
            {
                Debug.LogErrorFormat("Position is already taken: {0}", position);
            }
            Vector2 oldPosition = transform.position;
            GameManager.CurrentLevel.SetCharacter(this, position);

            if (position.x != oldPosition.x || position.y != oldPosition.y)
            {
                GameManager.CurrentLevel.SetCharacter(null, oldPosition);
            }
        }

        public int CalculateMovementCost(Vector2 position)
        {
            int cost;
            // Debug.LogFormat("Calculating movement cost for: {0}", position);
            if (position.Equals(transform.position))
            {
                Debug.LogFormat("The same position");
                cost = 0;
            }
            else
            {
                cost = CalculateMovementCost(position.x, position.y);
            }
            // Debug.LogFormat("Movement cost: {0}", cost);
            return cost;
        }

        /// <summary>
        /// Calculates the cost for this character to move to the given position.
        /// TODO add terrain considerations
        /// </summary>
        /// <param name="position">The position for which to determine the cost</param>
        /// <returns>The cost for this character to move to the given position</returns>
        protected virtual int CalculateMovementCost(float x, float y)
        {
            if (GameManager.CurrentLevel.IsOutOfBounds(x, y))
            {
                return int.MaxValue;
            }

            Terrain.Terrain terrain = GameManager.CurrentLevel.GetTerrain(x, y);
            return terrain.MovementCost;
        }

        /// <summary>
        /// Destroys all movable, attackable, and assistable transforms
        /// </summary>
        public void DestroyMovableAndAttackableAndAssistableTransforms()
        {
            GameManager.DestroyAll(MovableTransforms);
            GameManager.DestroyAll(AttackableTransforms);
            GameManager.DestroyAll(AssistableTransforms);
        }

        /// <summary>
        /// Get the list of attackable positions that have attackable characters from the provided list of positions
        /// </summary>
        /// <param name="attackableTransforms">The list of attackable transforms to look for characters</param>
        /// <returns>The list of attackable transforms that have attackable characters</returns>
        public HashSet<Vector2> ExtractAttackablePositionsWithCharacters(ISet<Vector2> attackablePositions)
        {
            HashSet<Vector2> list = new HashSet<Vector2>();

            foreach (Vector2 attackablePosition in attackablePositions)
            {
                Character defendingCharacter = GameManager.CurrentLevel.GetCharacter(attackablePosition);
                if (defendingCharacter != null && !defendingCharacter.Player.Equals(Player))
                {
                    _ = list.Add(attackablePosition);
                }
            }

            return list;
        }

        /// <summary>
        /// Get the list of positions that have assistable characters from the provided list of positions
        /// </summary>
        /// <param name="assistablePositions">The list of positions to look for characters</param>
        /// <returns>The list of positions that have assistable characters</returns>
        public HashSet<Vector2> ExtractAssistablePositionsWithCharacters(ISet<Vector2> assistablePositions)
        {
            HashSet<Vector2> list = new HashSet<Vector2>();

            foreach (Vector2 assistablePosition in assistablePositions)
            {
                Character defendingCharacter = GameManager.CurrentLevel.GetCharacter(assistablePosition);
                if (defendingCharacter != null && defendingCharacter.Player.Equals(Player))
                {
                    _ = list.Add(assistablePosition);
                }
            }

            return list;
        }

        /// <summary>
        /// Equip the item from the Character's inventory.
        /// </summary>
        /// <param name="item"></param>
        public void Equip(Item item)
        {
            if (Items.Count > 1)
            {
                if (!Items.Remove(item))
                {
                    Debug.LogErrorFormat("{0} does not exist in inventory.", item.Text.text);
                }
                else
                {
                    Items.Insert(0, item);
                }
            }
        }

        /// <summary>
        /// Called when a Character dies
        /// </summary>
        public virtual void Die()
        {
            Debug.LogFormat("Die: {0}", this);
            Destroy(gameObject);
            Debug.Assert(Player.Characters.Remove(this));
            GameManager.CurrentLevel.Kill(this);
            Debug.LogFormat("Remaining characters: {0}", Player.Characters.Count);
        }

        public void UseAssist(Character targetCharacter)
        {
            AssistInformation supportInfo = CalculateAssistInformation(targetCharacter);

            int hitPercentage = GameManager.Random.Next(100);
            if (hitPercentage <= supportInfo.HitPercentage)
            {
                supportInfo.Item.Assist(this, targetCharacter);
            }
        }

        /// <summary>
        /// Attack the specified Character and apply damage, effects, etc
        ///
        /// TODO make criticals a thing
        /// TODO make speed a thing
        /// </summary>
        /// <param name="defenseCharacter">The Character to attack</param>
        public void Attack(Character defenseCharacter)
        {
            AttackInformation attackInfo = CalculateAttackInformation(defenseCharacter);

            // TODO use actual hit percentage
            int attackHitPercentage = GameManager.Random.Next(100);

            int attackExperience = 1;
            int defenseExperience = 0;

            if (attackHitPercentage <= attackInfo.AttackHitPercentage)
            {
                attackInfo.AttackWeapon.Use();
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

                int defenseHitPercentage = GameManager.Random.Next(100);
                if (defenseHitPercentage <= attackInfo.DefenseHitPercentage)
                {
                    attackInfo.DefenseWeapon.Use();
                    if (attackInfo.DefenseDamage != 0)
                    {
                        defenseExperience += 9;
                        CurrentHp = Mathf.Max(0, CurrentHp - attackInfo.DefenseDamage);
                    }
                }

                if (CurrentHp == 0)
                {
                    defenseExperience += 10;
                    defenseCharacter.AddExperience(defenseExperience);
                    Die();
                    return;
                }
            }

            AddExperience(attackExperience);
        }

        public AssistInformation CalculateAssistInformation(Character targetCharacter)
        {
            Assistable supportItem = GetUsableItem<Assistable>();

            return new AssistInformation(supportItem, supportItem.HitPercentage, supportItem.Might);
        }

        /// <summary>
        /// Calculate the attack information for the attack on the specified defense Character
        /// </summary>
        /// <param name="defenseCharacter"></param>
        /// <returns></returns>
        public AttackInformation CalculateAttackInformation(Character defenseCharacter)
        {
            Weapon attackWeapon = GetUsableItem<Weapon>();
            Debug.LogFormat("Attack weapon: {0}", attackWeapon);

            Weapon defenseWeapon = defenseCharacter.GetUsableItem<Weapon>();
            Debug.LogFormat("Defense weapon: {0}", defenseWeapon);

            bool defenseCanAttack = false;
            if (defenseWeapon != null)
            {
                defenseCanAttack = defenseWeapon.IsInRange(defenseCharacter.transform.position, transform.position);
            }

            int attackHitPercentage = CalculateHitPercentage(attackWeapon, defenseCharacter, defenseWeapon);
            int attackDamage = CalculateDamage(attackWeapon, defenseCharacter, defenseWeapon);
            int attackCriticalPercentage = CalculateCriticalPercentage(attackWeapon, defenseCharacter, defenseWeapon);
            int attackNumberOfAttacks = CalculateNumberOfAttacks(attackWeapon, defenseCharacter, defenseWeapon);

            int defenseHitPercentage = 0;
            int defenseDamage = 0;
            int defenseCriticalPercentage = 0;
            int defenseNumberOfAttacks = 0;

            if (defenseCanAttack)
            {
                defenseHitPercentage = defenseCharacter.CalculateHitPercentage(defenseWeapon, this, attackWeapon);
                defenseDamage = defenseCharacter.CalculateDamage(defenseWeapon, this, attackWeapon);
                defenseCriticalPercentage = defenseCharacter.CalculateCriticalPercentage(defenseWeapon, this, attackWeapon);
                defenseNumberOfAttacks = defenseCharacter.CalculateNumberOfAttacks(defenseWeapon, this, attackWeapon);
            }

            return new AttackInformation(
                attackWeapon: attackWeapon,
                attackHitPercentage: attackHitPercentage,
                attackDamage: attackDamage,
                attackCriticalPercentage: attackCriticalPercentage,
                attackNumberOfAttacks: attackNumberOfAttacks,
                defenseWeapon: defenseWeapon,
                defenseHitPercentage: defenseHitPercentage,
                defenseDamage: defenseDamage,
                defenseCriticalPercentage: defenseCriticalPercentage,
                defenseCanAttack: defenseCanAttack,
                defenseNumberOfAttacks: defenseNumberOfAttacks);
        }

        /// <summary>
        /// Calculate the hit percentage
        ///
        /// TODO add terrain considerations
        /// </summary>
        /// <param name="attackCharacter"></param>
        /// <param name="attackWeapon"></param>
        /// <param name="defenseCharacter"></param>
        /// <returns></returns>
        private int CalculateHitPercentage(Weapon attackWeapon, Character defenseCharacter, Weapon defenseWeapon)
        {
            int damage =
                attackWeapon.HitPercentage
                + Skill
                - defenseCharacter.Speed
                + (defenseWeapon == null ? 0 : defenseWeapon.Weight);
            return Mathf.Clamp(damage, 0, 100);
        }

        /// <summary>
        /// Calculate the damage done by the attack Character with the specified weapon and the defense Character
        ///
        /// TODO Add terrain considerations
        /// </summary>
        /// <param name="attackCharacter"></param>
        /// <param name="attackWeapon"></param>
        /// <param name="defenseCharacter"></param>
        /// <returns></returns>
        public int CalculateDamage(Weapon attackWeapon, Character defenseCharacter, Weapon defenseWeapon)
        {
            int damage = attackWeapon.CalculateDamage(this, defenseCharacter, defenseWeapon);
            if (attackWeapon is StrengthWeapon)
            {
                damage += Strength - defenseCharacter.Defense;
            }
            else if (attackWeapon is Items.Weapons.Attackable.Magic.Magic)
            {
                damage += Magic - defenseCharacter.Resistance;
            }
            else
            {
                Debug.LogErrorFormat("Unknown weapon type: {0}", attackWeapon.GetType().Name);
            }

            return Mathf.Max(damage, 0);
        }

        public virtual int CalculateNumberOfAttacks(Weapon attackWeapon, Character defenseCharacter, Weapon defenseWeapon)
        {
            int calculation = Speed - attackWeapon.Weight - defenseCharacter.Speed + (defenseWeapon == null ? 0 : defenseWeapon.Weight);
            if (calculation >= 4)
            {
                return 2;
            }

            return 1;
        }

        private int CalculateCriticalPercentage(Weapon attackWeapon, Character defenseCharacter, Weapon defenseWeapon)
        {
            return Mathf.Clamp((Skill / 4) + attackWeapon.CriticalPercentage, 0, 100);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The type of items that should be put in the list</typeparam>
        /// <returns></returns>
        public List<T> GetUsableItems<T>() where T : Weapon
        {
            List<T> items = new List<T>();
            foreach (Item item in Items)
            {
                if (item is T t && IsProficient(t))
                {
                    items.Add(t);
                }
            }

            return items;
        }

        /// <summary>
        /// Gets the first usable item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetUsableItem<T>() where T : Weapon
        {
            List<T> items = GetUsableItems<T>();
            if (items.Count > 0)
            {
                return items[0];
            }

            return null;
        }

        /// <summary>
        /// Whether or not this character is proficient (is able to wield) the weapon in question.
        /// </summary>
        /// <param name="weapon"></param>
        /// <returns></returns>
        public bool IsProficient(Weapon weapon)
        {
            Debug.LogFormat("Is {0} proficient with {1}", CharacterName, weapon);
            foreach (Proficiency proficiency in Proficiencies)
            {
                Debug.LogFormat("Proficiency: {0}", proficiency);
                if (weapon.GetType().IsSubclassOf(proficiency.Type) && proficiency.ProficiencyRank >= weapon.RequiredProficiencyRank)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Represents an attack/defense scenario for two Characters
        /// </summary>
        public class AttackInformation
        {
            public Weapon AttackWeapon;
            public int AttackHitPercentage;
            public int AttackDamage;
            public int AttackCriticalPercentage;
            public int AttackNumberOfAttacks;

            public Weapon DefenseWeapon;
            public int DefenseHitPercentage;
            public int DefenseDamage;
            public int DefenseCriticalPercentage;
            public bool DefenseCanAttack;
            public int DefenseNumberOfAttacks;

            public AttackInformation(
                Weapon attackWeapon,
                int attackHitPercentage,
                int attackDamage,
                int attackCriticalPercentage,
                int attackNumberOfAttacks,
                Weapon defenseWeapon,
                int defenseHitPercentage,
                int defenseDamage,
                int defenseCriticalPercentage,
                bool defenseCanAttack,
                int defenseNumberOfAttacks)
            {
                AttackWeapon = attackWeapon;
                AttackHitPercentage = attackHitPercentage;
                AttackDamage = attackDamage;
                AttackCriticalPercentage = attackCriticalPercentage;
                AttackNumberOfAttacks = attackNumberOfAttacks;

                DefenseWeapon = defenseWeapon;
                DefenseHitPercentage = defenseHitPercentage;
                DefenseDamage = defenseDamage;
                DefenseCriticalPercentage = defenseCriticalPercentage;
                DefenseCanAttack = defenseCanAttack;
                DefenseNumberOfAttacks = defenseNumberOfAttacks;
            }
        }

        public class AssistInformation
        {
            public Assistable Item;
            public int HitPercentage;
            public int Might;

            public AssistInformation(Assistable item, int hitPercentage, int might)
            {
                Item = item;
                HitPercentage = hitPercentage;
                Might = might;
            }
        }
    }
}