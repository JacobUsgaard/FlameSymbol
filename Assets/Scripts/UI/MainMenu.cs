using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void NewGameButtonOnClick()
    {
        SceneManager.LoadScene("Resources/Scenes/FlameSymbol");
    }
}
