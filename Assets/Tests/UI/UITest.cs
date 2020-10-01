using System.Collections;
using NUnit.Framework;
using UnityEngine;

namespace Tests.UI
{
    public class UITest
    {

        protected GameManager GameManager;

        [SetUp]
        public void Setup()
        {
            GameObject gameGameObject = Object.Instantiate(Resources.Load<GameObject>("Prefabs/UI/Main Camera"));

            GameManager = gameGameObject.GetComponent<GameManager>();
        }

        [TearDown]
        public void Destroy()
        {
            Object.Destroy(GameManager.gameObject);
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

            for (int i = 0; i < Mathf.Abs(currentPosition.x - x); i++)
            {
                cursor.OnArrow(1, 0);
            }

            for (int i = 0; i < Mathf.Abs(currentPosition.y - y); i++)
            {
                cursor.OnArrow(0, 1);
            }

            yield return null;
        }
    }
}
