using UnityEngine;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour {

    private Text _text;
    private readonly int _usesTotal;
    private int _usesRemaining;
    protected GameManager GameManager;

    public Text Text
    {
        get
        {
            return _text;
        }
    }

    public int UsesTotal
    {
        get
        {
            return _usesTotal;
        }
    }

    public int UsesRemaining
    {
        get
        {
            return _usesRemaining;
        }
    }

    public Item(Text text, int uses)
    {
        _text = text;
        _usesTotal = uses;
        _usesRemaining = _usesTotal;
    }

    /// <summary>
    /// Creates the instance of the text for the Text.
    /// </summary>
    public virtual void Start()
    {
        _text = transform.GetComponentInParent<Text>();
    }
}
