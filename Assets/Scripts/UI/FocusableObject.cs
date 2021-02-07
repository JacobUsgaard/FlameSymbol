using UnityEngine;

public abstract class FocusableObject : ManagedMonoBehavior
{
    public void Focus()
    {
        CurrentObject = this;
        Debug.Log("Focus: " + CurrentObject);
    }

    public virtual void OnArrow(float horizontal, float vertical)
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnSubmit()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnCancel()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnInformation()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnRightMouse(Vector2 mousePosition)
    {
        throw new System.NotImplementedException();
    }

    public bool IsInFocus()
    {
        return CurrentObject.Equals(this);
    }

    public static FocusableObject CurrentObject { get; private set; }
}
