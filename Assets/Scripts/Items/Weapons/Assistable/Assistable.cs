using Characters;

namespace Items.Weapons.Assistable
{
    /// <summary>
    /// Weapons that are used for assisting other characters on the same team
    /// </summary>
    public abstract class Assistable : Weapon
    {
        public abstract void Assist(Character sourceCharacter, Character targetCharacter);
    }
}