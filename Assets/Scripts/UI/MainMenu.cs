using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        public Button NewGameButton;
        public Button ContinueButton;

        public List<Button> Buttons { get; set; }
        public int ButtonIndex { get; set; }

        public void Start()
        {
            Buttons = new List<Button> { NewGameButton, ContinueButton };

            SelectButton(0);
        }

        public void Update()
        {
            if (Input.GetButtonDown("Submit"))
            {
                Debug.Log("Submit");
                Buttons[ButtonIndex].onClick.Invoke();
            }
            else if (Input.GetAxis("Vertical") is var vertical && vertical != 0f)
            {
                SelectButton((ButtonIndex + Buttons.Count - System.Math.Sign(vertical)) % Buttons.Count);
            }
        }

        /// <summary>
        /// Marks the button as selected so that when the player presses 'Enter',
        /// that button will be click
        /// </summary>
        public void SelectButton(int index)
        {
            ButtonIndex = index;
            Buttons[ButtonIndex].Select();
        }

        public void ContinueButtonOnClick()
        {
            Debug.LogError("ContinueButtonOnClick not yet implemented");
        }

        public void CopyButtonOnClick()
        {
            Debug.LogError("CopyButtonOnClick not yet implemented");
        }

        public void NewGameButtonOnClick()
        {
            Debug.Log("NewGameButtonOnClick");
            SceneManager.LoadScene(GameManager.SceneNameFlameSymbol);
        }
    }
}