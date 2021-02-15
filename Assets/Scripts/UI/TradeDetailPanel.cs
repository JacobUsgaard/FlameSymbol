using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeDetailPanel : FocusableObject
{

    private static readonly string INDICATOR = " <";

    /// <summary>
    /// The text that goes above the source items panel
    /// </summary>
    public Text SourceText;

    /// <summary>
    /// The source items panel
    /// </summary>
    public Transform TradeSourcePanel;

    /// <summary>
    /// The text that goes above the destination items panel
    /// </summary>
    public Text DestinationText;

    /// <summary>
    /// The destination items panel
    /// </summary>
    public Transform TradeDestinationPanel;

    private int _sourceItemsIndex = 0;
    public int SourceItemsIndex => _sourceItemsIndex;

    /// <summary>
    /// The menu items in the source panel
    /// </summary>
    public List<TradeMenuItem> TradeSourceMenuItems { get; } = new List<TradeMenuItem>();

    private int _destinationItemsIndex;
    public int DestinationItemsIndex => _destinationItemsIndex;

    /// <summary>
    /// The menu items in the destination panel
    /// </summary>
    public List<TradeMenuItem> TradeDestinationMenuItems { get; } = new List<TradeMenuItem>();

    public Character SourceCharacter { get; private set; }
    public Character DestinationCharacter { get; private set; }

    /// <summary>
    /// The valid sides that the cursor can be
    /// </summary>
    public enum Side
    {
        SOURCE,
        DESTINATION
    }

    /// <summary>
    /// Which side the cursor is currently on
    /// </summary>
    public Side CurrentSide { get; set; } = Side.SOURCE;


    public override void OnArrow(float horizontal, float vertical)
    {
        if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
        {
            int sign = System.Math.Sign(horizontal);
            if (CurrentSide == Side.SOURCE && TradeDestinationMenuItems.Count > 0 && sign == 1)
            {
                TradeMenuItem previousItem = TradeSourceMenuItems[SourceItemsIndex];
                if (previousItem != null)
                {
                    previousItem.Text.text = previousItem.Text.text.Replace(INDICATOR, "");
                }

                SetItemsTextsIndex(TradeDestinationMenuItems, ref _destinationItemsIndex);
                CurrentSide = Side.DESTINATION;
            }
            else if (CurrentSide == Side.DESTINATION && TradeSourceMenuItems.Count > 0 && sign == -1)
            {
                TradeMenuItem previousItem = TradeDestinationMenuItems[DestinationItemsIndex];
                if (previousItem != null)
                {
                    previousItem.Text.text = previousItem.Text.text.Replace(INDICATOR, "");
                }

                SetItemsTextsIndex(TradeSourceMenuItems, ref _sourceItemsIndex);
                CurrentSide = Side.SOURCE;
            }
            else
            {
                Debug.Log("Just stay where you are I guess");
            }
        }
        else
        {
            int sign = System.Math.Sign(vertical);
            if (CurrentSide == Side.SOURCE)
            {
                SetItemsTextsIndex(TradeSourceMenuItems, ref _sourceItemsIndex, (SourceItemsIndex + TradeSourceMenuItems.Count + sign) % TradeSourceMenuItems.Count);
            }
            else
            {
                SetItemsTextsIndex(TradeDestinationMenuItems, ref _destinationItemsIndex, (DestinationItemsIndex + TradeDestinationMenuItems.Count + sign) % TradeDestinationMenuItems.Count);
            }
        }
    }

    public void Show(Character sourceCharacter, Character destinationCharacter)
    {
        SourceCharacter = sourceCharacter;
        SourceText.text = sourceCharacter.CharacterName;
        Debug.Log("Source character: " + sourceCharacter);

        DestinationCharacter = destinationCharacter;
        DestinationText.text = destinationCharacter.CharacterName;
        Debug.Log("Destination character: " + destinationCharacter);

        foreach (TradeMenuItem tradeMenuItem in TradeSourceMenuItems)
        {
            Destroy(tradeMenuItem.Text.gameObject);
        }
        TradeSourceMenuItems.Clear();

        foreach (TradeMenuItem tradeMenuItem in TradeDestinationMenuItems)
        {
            Destroy(tradeMenuItem.Text.gameObject);
        }
        TradeDestinationMenuItems.Clear();

        foreach (Item item in sourceCharacter.Items)
        {
            TradeSourceMenuItems.Add(new TradeMenuItem(TradeSourcePanel, item));
        }

        foreach (Item item in destinationCharacter.Items)
        {
            TradeDestinationMenuItems.Add(new TradeMenuItem(TradeDestinationPanel, item));
        }

        if (TradeSourceMenuItems.Count > 0)
        {
            SetItemsTextsIndex(TradeSourceMenuItems, ref _sourceItemsIndex);
            CurrentSide = Side.SOURCE;
        }
        else if (TradeDestinationMenuItems.Count > 0)
        {
            SetItemsTextsIndex(TradeDestinationMenuItems, ref _destinationItemsIndex);
            CurrentSide = Side.DESTINATION;
        }
        else
        {
            Debug.LogError("Neither character has items to trade.");
        }

        transform.position = sourceCharacter.transform.position;

        Focus();
        transform.gameObject.SetActive(true);
    }

    private void SetItemsTextsIndex(List<TradeMenuItem> tradeMenuItems, ref int masterIndex, int index = 0)
    {
        // Remove indicator from previous text
        TradeMenuItem previousItem = tradeMenuItems[masterIndex];
        previousItem.Text.text = previousItem.Text.text.Replace(INDICATOR, "");

        // Update currently selected text
        masterIndex = index;
        TradeMenuItem currentItem = tradeMenuItems[masterIndex];
        currentItem.Text.text += INDICATOR;
    }

    public override void OnCancel()
    {
        transform.gameObject.SetActive(false);
        GameManager.Cursor.TradeDetailPanelOnClose();
    }

    public override void OnSubmit()
    {

        if (CurrentSide == Side.SOURCE)
        {
            TradeMenuItem tradeMenuItem = TradeSourceMenuItems[SourceItemsIndex];
            SourceCharacter.Items.Remove(tradeMenuItem.Item);
            DestinationCharacter.Items.Add(tradeMenuItem.Item);
        }
        else
        {
            TradeMenuItem tradeMenuItem = TradeDestinationMenuItems[DestinationItemsIndex];
            DestinationCharacter.Items.Remove(tradeMenuItem.Item);
            SourceCharacter.Items.Add(tradeMenuItem.Item);
        }

        Show(SourceCharacter, DestinationCharacter);
    }

    public override void OnInformation()
    {
        Debug.LogFormat("TradeDetailPanel.OnInformation is not implemented");
    }

    /// <summary>
    /// Information representing an item on the trade menu
    /// </summary>
    public class TradeMenuItem
    {
        public Item Item;
        public Text Text;

        public TradeMenuItem(Transform parent, Item item)
        {
            Item = item;
            Text = Instantiate(item.Text, parent);
        }
    }
}