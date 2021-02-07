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
    }
}
