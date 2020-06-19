using UnityEngine;

public abstract class FocusableObject :  ManagedMonoBehavior {

    private static FocusableObject currentObject;

    public void Focus()
    {
        currentObject = this;
        Debug.Log("Focus: " + CurrentObject);
    }

    public abstract void OnArrow(float horizontal, float vertical);

    public abstract void OnSubmit();

    public abstract void OnCancel();

    public virtual void OnRightMouse(Vector2 mousePosition)
    {
        Debug.LogError("Not yet implemented");
    }

    public bool IsInFocus()
    {
        return currentObject.Equals(this);
    }

    public static FocusableObject CurrentObject
    {
        get
        {
            return currentObject;
        }
    }
}
