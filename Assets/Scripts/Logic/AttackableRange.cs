using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The range displayed for a selected group of characters
/// </summary>
public class AttackableRange : ManagedScriptableObject
{
    public readonly List<Character> Characters = new List<Character>();
    public readonly List<Transform> AttackableTransforms = new List<Transform>();

    public void AddCharacter(Character character)
    {
        Debug.LogFormat("Adding character to attackable range");
        Characters.Add(character);
        CreateAttackableTransforms();

        Debug.LogFormat("Total attackable transforms: {0}", AttackableTransforms.Count);
    }

    private void CreateAttackableTransforms()
    {
        Debug.LogFormat("Creating attackable transforms");
        DeleteAll(AttackableTransforms);
        HashSet<Vector2> positions = new HashSet<Vector2>();
        foreach (Character character in Characters)
        {
            positions.UnionWith(character.CalculateAttackablePositions());
        }

        foreach (Vector2 position in positions)
        {
            AttackableTransforms.Add(Instantiate(GameManager.AttackableSpacePrefab, new Vector2(position.x, position.y), Quaternion.identity, GameManager.transform));
        }
    }

    public void RemoveCharacter(Character character)
    {
        Characters.Remove(character);
        CreateAttackableTransforms();
    }

    public void Clear()
    {
        DeleteAll(AttackableTransforms);
        Characters.Clear();
    }
}
