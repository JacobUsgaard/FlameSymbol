using UnityEngine;

public class AIPlayer : Player
{
    public void Move()
    {

    }

    public Vector2 FindMove(Character attackingCharacter)
    {
        System.Random random = new System.Random();

        Vector2 oldPosition = attackingCharacter.transform.position;
        Vector2 newPosition = new Vector2(-1, -1);
        do
        {
            switch (random.Next(3))
            {
                case 0:
                    newPosition = new Vector2(oldPosition.x + 1, oldPosition.y);
                    break;
                case 1:
                    newPosition = new Vector2(oldPosition.x - 1, oldPosition.y);
                    break;
                case 2:
                    newPosition = new Vector2(oldPosition.x, oldPosition.y + 1);
                    break;
                case 3:
                    newPosition = new Vector2(oldPosition.x, oldPosition.y - 1);
                    break;
            }
        } while (GameManager.CurrentLevel.IsOutOfBounds(newPosition));

        return newPosition;
    }

    public class MoveInfo
    {
        public Vector2 Move;
        public Vector2 Attack;

        public MoveInfo() { }
    }
}
