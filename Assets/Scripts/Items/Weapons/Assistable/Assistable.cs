using Characters;

namespace Items.Weapons.Assistable
{
    public abstract class Assistable : Weapon
    {
        public abstract void Assist(Character sourceCharacter, Character targetCharacter);
    }
}