namespace Terrain
{
    public class Wall : Terrain
    {
        public override bool IsPassable(Character character, float x, float y)
        {
            return character.IsFlyer;
        }
    }
}
