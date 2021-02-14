using UnityEngine;

/// <summary>
/// Object that has the user's input.
/// </summary>
public abstract class FocusableObject : ManagedMonoBehavior
{
    /// <summary>
    /// Focus on this object.
    /// </summary>
    public void Focus()
    {
        CurrentObject = this;
        Debug.LogFormat("Focus: {0}", CurrentObject);
    }

    /// <summary>
    /// Callback when an arrow button is pressed
    /// </summary>
    /// <param name="horizontal">The horizontal direction of the button press.</param>
    /// <param name="vertical">The vertical direction of the button press.</param>
    public abstract void OnArrow(float horizontal, float vertical);

    /// <summary>
    /// Callback when the submit button is pressed
    /// </summary>
    public abstract void OnSubmit();

    /// <summary>
    /// Callback when the cancel button is pressed
    /// </summary>
    public abstract void OnCancel();

    /// <summary>
    /// Callback when the information button is pressed
    /// </summary>
    public abstract void OnInformation();

    /// <summary>
    /// Whether or not this object is currently in focus
    /// </summary>
    /// <returns></returns>
    public bool IsInFocus()
    {
        return CurrentObject.Equals(this);
    }

    /// <summary>
    /// The object currently in focus
    /// </summary>
    public static FocusableObject CurrentObject { get; private set; }
}
