using NUnit.Framework;

namespace Tests.Items.Weapons
{
    public class WeaponTests
    {
        public class FakeWeapon : Weapon { }

        [Test]
        public void IsInRangeTest1()
        {
            Assert.False(Weapon.IsInRange(0, 0, 0, 0, -1));
        }
    }
}
