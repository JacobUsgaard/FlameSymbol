using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    /// <summary>
    /// Test that uses the GameManager
    /// </summary>
    public abstract class GameManagerTest
    {
        protected GameManager GameManager;

        [UnitySetUp]
        public IEnumerator LoadScene()
        {
            yield return SceneManager.LoadSceneAsync(GameManager.SceneNameFlameSymbol, LoadSceneMode.Single);
        }

        [SetUp]
        public void SetGameManager()
        {
            GameManager = Object.FindObjectOfType<GameManager>();
        }

        [TearDown]
        public void DestroyGameManager()
        {
            Object.Destroy(GameManager.gameObject);
        }

        public IEnumerator DownArrow(FocusableObject focusableObject)
        {
            focusableObject.OnArrow(0, -1);
            yield return null;
        }

        public IEnumerator UpArrow(FocusableObject focusableObject)
        {
            focusableObject.OnArrow(0, 1);
            yield return null;
        }

        public IEnumerator RightArrow(FocusableObject focusableObject)
        {
            focusableObject.OnArrow(1, 0);
            yield return null;
        }

        public IEnumerator LeftArrow(FocusableObject focusableObject)
        {
            focusableObject.OnArrow(-1, 0);
            yield return null;
        }

        public IEnumerator Enter(FocusableObject focusableObject)
        {
            focusableObject.OnSubmit();
            yield return null;
        }

        public IEnumerator Cancel()
        {
            yield return Cancel(FocusableObject.CurrentObject);
        }

        public IEnumerator Cancel(FocusableObject focusableObject)
        {
            focusableObject.OnCancel();
            yield return null;
        }

        public IEnumerator Information()
        {
            yield return Information(FocusableObject.CurrentObject);
        }

        public IEnumerator Information(FocusableObject focusableObject)
        {
            focusableObject.OnInformation();
            yield return null;
        }

        /// <summary>
        /// Helper method to move the cursor to the desired position by pressing the arrow keys.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public IEnumerator MoveCursor(float x, float y)
        {
            Cursor cursor = GameManager.Cursor;
            Vector2 currentPosition = cursor.transform.position;

            float xDifference = x - currentPosition.x;
            float yDifference = y - currentPosition.y;

            float xSign = Mathf.Sign(xDifference);
            float ySign = Mathf.Sign(yDifference);

            float xDistance = Mathf.Abs(xDifference);
            float yDistance = Mathf.Abs(yDifference);

            for (int i = 0; i < xDistance; i++)
            {
                cursor.OnArrow(xSign, 0);
            }

            for (int i = 0; i < yDistance; i++)
            {
                cursor.OnArrow(0, ySign);
            }

            yield return null;
        }
    }
}
