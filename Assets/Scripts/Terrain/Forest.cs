namespace Terrain
{
    public class Forest : Terrain
    {
        public new void Awake()
        {
            base.Awake();

            MovementCost = 2;
            HitPercentageBoost = 20;
            DefenseBoost = 2;
        }
    }
}
