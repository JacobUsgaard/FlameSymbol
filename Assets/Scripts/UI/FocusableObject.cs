using UnityEngine;

public abstract class FocusableObject :  MonoBehaviour {

    public static FocusableObject CurrentObject;

    public static GameManager GameManager;

    public void Focus()
    {
        CurrentObject = this;
        Debug.Log("Focus: " + CurrentObject);
    }

    public void Start()
    {
        if(GameManager == null)
        {
            GameManager = GameManager.gameManager;
        }
    }

    public abstract void OnArrow(float horizontal, float vertical);

    public abstract void OnSubmit();

    public abstract void OnCancel();
}
