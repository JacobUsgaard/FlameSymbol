namespace Terrain
{
    public class Forest : Terrain
    {
        public Forest() : base()
        {
            MovementCost = 2;
            HitPercentageBoost = 20;
            DefenseBoost = 2;
        }
    }
}
