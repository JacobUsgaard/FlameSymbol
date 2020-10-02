using UnityEngine;

public abstract class FocusableObject : ManagedMonoBehavior
{
    public void Focus()
    {
        CurrentObject = this;
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
        return CurrentObject.Equals(this);
    }

    public static FocusableObject CurrentObject { get; private set; }
}
