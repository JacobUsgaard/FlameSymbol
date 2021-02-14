using UnityEngine;
using UnityEngine.UI;

public abstract class Item : ManagedScriptableObject
{
    public Text Text;
    public int UsesTotal;
    public int UsesRemaining;

    /// <summary>
    /// Whether or not this item is broken.
    /// </summary>
    /// <returns></returns>
    public virtual bool IsBroken()
    {
        return UsesRemaining == 0;
    }

    /// <summary>
    /// Letting the item know it's been used.
    /// </summary>
    public virtual void Use()
    {
        UsesRemaining--;
        if (UsesRemaining < 0)
        {
            Debug.LogErrorFormat("Item {0} cannot have negative uses remaining.", Text.text);
        }
        else if (UsesRemaining == 0)
        {
            Break();
        }
    }

    public virtual void Break()
    {
        Debug.LogFormat("Breaking {0}", Text.text);
        Destroy(Text);
    }
}
