using UnityEngine;

public abstract class FocusableObject :  ManagedMonoBehavior {

    private static FocusableObject _currentObject;

    public virtual void Focus()
    {
        _currentObject = this;
        Debug.Log("Focus: " + _currentObject);
    }

    public abstract void OnArrow(float horizontal, float vertical);

    public abstract void OnSubmit();

    public abstract void OnCancel();

    public static FocusableObject CurrentObject
    {
        get
        {
            return _currentObject;
        }
    }
}
