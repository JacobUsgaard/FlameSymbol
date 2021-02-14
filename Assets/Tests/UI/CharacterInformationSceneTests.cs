using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;


namespace Tests.UI
{
    public class CharacterInformationSceneTests : GameManagerTest
    {

        /// <summary>
        /// Test for when the character information scene is activate with
        /// no character selected
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator InvalidCharacterTest()
        {
            yield return MoveCursor(1, 0);

            SceneManager.LoadScene("Scenes/CharacterInformation", LoadSceneMode.Additive);

            yield return null;

            LogAssert.Expect(LogType.Error, "Failed to find character");
        }

        /// <summary>
        /// Given: The CharacterInformationScene is loaded
        ///     And: The CharacterInformationScene is in focus
        /// When: The arrow button is pressed
        /// Then: A message is logged
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnArrowTest()
        {

            yield return Information();

            Scene scene = SceneManager.GetSceneByName("CharacterInformation");

            Assert.AreEqual(typeof(CharacterInformationScene), FocusableObject.CurrentObject.GetType());
            Assert.True(scene.isLoaded);

            yield return DownArrow();
            LogAssert.Expect(LogType.Log, "CharacterInformationScene.OnArrow not implemented");
        }

        /// <summary>
        /// Given: The CharacterInformationScene is loaded
        ///     And: The CharacterInformationScene is in focus
        /// When: The Information button is pressed
        /// Then: A message is logged
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnInformationTest()
        {
            yield return Information();

            Assert.AreEqual(typeof(CharacterInformationScene), FocusableObject.CurrentObject.GetType());
            Scene scene = SceneManager.GetSceneByName("CharacterInformation");
            Assert.True(scene.isLoaded);

            yield return Information();
            LogAssert.Expect(LogType.Log, "CharacterInformationScene.OnInformation not implemented");
        }

        /// <summary>
        /// Given: The CharacterInformationScene is loaded
        ///     And: The CharacterInformationScene is in focus
        /// When: The Submit button is pressed
        /// Then: A message is logged
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OnSubmitTest()
        {
            yield return Information();

            Assert.AreEqual(typeof(CharacterInformationScene), FocusableObject.CurrentObject.GetType());
            Scene scene = SceneManager.GetSceneByName("CharacterInformation");
            Assert.True(scene.isLoaded);

            yield return Submit();
            LogAssert.Expect(LogType.Log, "CharacterInformationScene.OnSubmit not implemented");
        }
    }
}
