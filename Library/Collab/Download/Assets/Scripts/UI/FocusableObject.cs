using UnityEngine;

public abstract class FocusableObject :  MonoBehaviour {

    public static FocusableObject CurrentObject;

    public GameManager GameManager;

    public virtual void Start()
    {
        if (GameManager == null)
        {
            GameManager = transform.GetComponentInParent<GameManager>();
        }
        Debug.Log("Starting: " + name + " with GameManager: " + GameManager);
    }

    public void Focus()
    {
        CurrentObject = this;
        Debug.Log("Focus: " + CurrentObject);
    }

    public abstract void OnArrow(float horizontal, float vertical);

    public abstract void OnSubmit();

    public abstract void OnCancel();
}
