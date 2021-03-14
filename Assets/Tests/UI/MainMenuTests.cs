using System.Collections;
using Logic;
using NUnit.Framework;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.UI
{
    public class MainMenuTests
    {
        private MainMenu MainMenu;

        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
            yield return SceneManager.LoadSceneAsync(GameManager.SceneNameMainMenu, LoadSceneMode.Single);
        }

        [SetUp]
        public void Setup()
        {
            MainMenu = Object.FindObjectOfType<MainMenu>();
        }

        [TearDown]
        public void TearDown()
        {
            if (MainMenu != null)
            {
                Object.Destroy(MainMenu.gameObject);
            }

            GameManager gameManager = Object.FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                Object.Destroy(gameManager.gameObject);
            }
        }

        [Test]
        public void MainMenuSetupTest1()
        {
            Assert.AreEqual(0, MainMenu.ButtonIndex);
        }

        [UnityTest]
        public IEnumerator NewGameButtonTest1()
        {
            MainMenu.NewGameButtonOnClick();

            yield return null;

            Assert.AreEqual(GameManager.SceneNameFlameSymbol, SceneManager.GetActiveScene().name);

            // Make sure main menu is destroyed
            MainMenu mainMenu = Object.FindObjectOfType<MainMenu>();
            Assert.Null(mainMenu);
        }

        [UnityTest]
        public IEnumerator ContinueButtonOnClickTest()
        {
            yield return null;
            MainMenu.ContinueButtonOnClick();
            LogAssert.Expect(LogType.Error, "ContinueButtonOnClick not yet implemented");
        }

        [UnityTest]
        public IEnumerator CopyButtonOnClickTest()
        {
            yield return null;
            MainMenu.CopyButtonOnClick();
            LogAssert.Expect(LogType.Error, "CopyButtonOnClick not yet implemented");
        }
    }
}
