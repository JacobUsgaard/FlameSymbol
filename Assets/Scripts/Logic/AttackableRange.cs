using System.Collections.Generic;
using Characters;
using UnityEngine;

namespace Logic
{

    /// <summary>
    /// The range displayed for a selected group of characters
    /// </summary>
    public class AttackableRange : ManagedScriptableObject
    {
        public readonly HashSet<Character> Characters = new HashSet<Character>();
        public readonly List<Transform> AttackableTransforms = new List<Transform>();

        public void AddCharacter(Character character)
        {
            Debug.LogFormat("Adding character to attackable range");
            if (!Characters.Contains(character) && Characters.Add(character))
            {
                CreateAttackableTransforms();
            }
            Debug.LogFormat("Total attackable transforms: {0} for {1} characters", AttackableTransforms.Count, Characters.Count); ;
        }

        private void CreateAttackableTransforms()
        {
            Debug.LogFormat("Creating attackable transforms");
            GameManager.DestroyAll(AttackableTransforms);
            HashSet<Vector2> AttackablePositions = new HashSet<Vector2>();
            foreach (Character character in Characters)
            {
                AttackablePositions.UnionWith(character.CalculateAttackablePositions());
            }

            foreach (Vector2 position in AttackablePositions)
            {
                AttackableTransforms.Add(Instantiate(GameManager.AttackableSpacePrefab, new Vector2(position.x, position.y), Quaternion.identity, GameManager.transform));
            }
        }

        public void RemoveCharacter(Character character)
        {
            if (!Characters.Remove(character))
            {
                Debug.LogErrorFormat("Cannot remove character {0} from AttackableRange at {1}", character.CharacterName, character.transform.position);
                return;
            }

            CreateAttackableTransforms();
        }

        public void Clear()
        {
            Characters.Clear();
            GameManager.DestroyAll(AttackableTransforms);
        }
    }
}