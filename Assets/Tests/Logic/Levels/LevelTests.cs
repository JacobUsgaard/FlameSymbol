using System.Collections;
using Characters;
using Logic.Levels;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Logic.Levels
{
    public class LevelTests : GameManagerTest
    {
        [UnityTest]
        public IEnumerator CheckColor()
        {
            yield return null;

            Character character = GameManager.CurrentLevel.GetCharacter(0, 0);

            SpriteRenderer spriteRenderer = Level.FindComponentInChildWithTag<SpriteRenderer>(character.gameObject, Level.CharacterColorTag);
            Assert.AreEqual(Color.red, spriteRenderer.color);
        }

        [UnityTest]
        public IEnumerator AddCharacterTest1()
        {
            GameManager.CurrentLevel.AddCharacter(GameManager.CurrentLevel.GetCharacter(0, 0), 0, 0);
            LogAssert.Expect(LogType.Error, "Character, Knight, already exists at (0, 0)");
            yield return null;
        }

        [UnityTest]
        public IEnumerator DrawCharacterTest1()
        {
            Character character = GameManager.CurrentLevel.GetCharacter(0, 0);
            SpriteRenderer spriteRenderer = Level.FindComponentInChildWithTag<SpriteRenderer>(character.gameObject, Level.CharacterColorTag);
            Assert.NotNull(spriteRenderer);
            Object.Destroy(spriteRenderer);
            yield return null;
            GameManager.CurrentLevel.DrawCharacter(character, 3, 0);
            LogAssert.Expect(LogType.Error, "Character Knight_0(Clone) at (3.0, 0.0) does not have sprite renderer with tag 'CharacterColor'");
        }

        [Test]
        public void InitTest()
        {
            BadLevel badLevel = ScriptableObject.CreateInstance<BadLevel>();
            badLevel.Init(null, null, null);
            LogAssert.Expect(LogType.Error, "Character map and terrain map must be the same dimensions");
        }

        /// <summary>
        /// Given: A game object with a child
        ///     And: That child does not have a component of that specified type
        /// When: Trying to find a component of that type
        /// Then: FindComponentInChildWithTag returns null
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator FindComponentInChildWithTagTest()
        {
            GameObject gameObject = new GameObject("Test");
            GameObject childObject = new GameObject("Child");
            childObject.transform.parent = gameObject.transform;
            SpriteRenderer spriteRenderer = Level.FindComponentInChildWithTag<SpriteRenderer>(gameObject, Level.CharacterColorTag);
            Assert.Null(spriteRenderer);
            yield return null;
        }

        [UnityTest]
        public IEnumerator SetCharacterTest()
        {
            GameManager.CurrentLevel.SetCharacter(null, -1, 0);
            LogAssert.Expect(LogType.Error, "Position is out of bounds: (-1,0)");
            yield return null;
        }
    }

    public class BadLevel : Level
    {
        public override bool IsLevelOver()
        {
            throw new System.NotImplementedException();
        }

        protected override void Init()
        {
            CharacterMap = new Character[1, 1];
            TerrainMap = new Terrain.Terrain[2, 2];
        }
    }
}
