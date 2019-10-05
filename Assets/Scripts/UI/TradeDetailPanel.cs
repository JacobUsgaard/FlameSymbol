using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeDetailPanel : FocusableObject {

    public Text SourceText;
    public Transform TradeSourcePanel;
    public Text DestinationText;
    public Transform TradeDestinationPanel;

    private int SourceItemsIndex = 0;
    private readonly List<TradeMenuItem> TradeSourceMenuItems = new List<TradeMenuItem>();

    private int DestinationItemsIndex = 0;
    private readonly List<TradeMenuItem> TradeDestinationMenuItems = new List<TradeMenuItem>();

    private Character SourceCharacter;
    private Character DestinationCharacter;

    private enum Side
    {
        SOURCE,
        DESTINATION
    }

    private Side CurrentSide = Side.SOURCE;

    private static readonly string INDICATOR = " <";

    public override void OnArrow(float horizontal, float vertical)
    {
        if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
        {
            int sign = System.Math.Sign(horizontal);
            if (CurrentSide == Side.SOURCE && TradeDestinationMenuItems.Count > 0 && sign == 1)
            {
                TradeMenuItem previousItem = TradeSourceMenuItems[SourceItemsIndex];
                if(previousItem != null)
                {
                    previousItem.Text.text = previousItem.Text.text.Replace(INDICATOR, "");
                }

                SetItemsTextsIndex(TradeDestinationMenuItems, ref DestinationItemsIndex);
                CurrentSide = Side.DESTINATION;
            }
            else if(CurrentSide == Side.DESTINATION  && TradeSourceMenuItems.Count > 0 && sign == -1)
            {
                TradeMenuItem previousItem = TradeDestinationMenuItems[DestinationItemsIndex];
                if (previousItem != null)
                {
                    previousItem.Text.text = previousItem.Text.text.Replace(INDICATOR, "");
                }

                SetItemsTextsIndex(TradeSourceMenuItems, ref SourceItemsIndex);
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
                SetItemsTextsIndex(TradeSourceMenuItems, ref SourceItemsIndex, (SourceItemsIndex + TradeSourceMenuItems.Count + sign) % TradeSourceMenuItems.Count);
            }
            else
            {
                SetItemsTextsIndex(TradeDestinationMenuItems, ref DestinationItemsIndex, (DestinationItemsIndex + TradeDestinationMenuItems.Count + sign) % TradeDestinationMenuItems.Count);
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

        

        foreach(TradeMenuItem tradeMenuItem in TradeSourceMenuItems)
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

        if(TradeSourceMenuItems.Count > 0)
        {
            SetItemsTextsIndex(TradeSourceMenuItems, ref SourceItemsIndex);
            CurrentSide = Side.SOURCE;
        }
        else if(TradeDestinationMenuItems.Count > 0)
        {
            SetItemsTextsIndex(TradeDestinationMenuItems, ref DestinationItemsIndex);
            CurrentSide = Side.DESTINATION;
        }
        else
        {
            Debug.LogError("Neither character has items to trade. What do");
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
        currentItem.Text.text = currentItem.Text.text + INDICATOR;
    }

    public override void OnCancel()
    {
        transform.gameObject.SetActive(false);
        GameManager.Cursor.TradeDetailPanelOnClose();
    }

    public override void OnSubmit()
    {
        
        if(CurrentSide == Side.SOURCE)
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
