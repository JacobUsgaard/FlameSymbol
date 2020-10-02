using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : FocusableObject
{

    protected GameManager.Callback CancelCallback;

    public readonly List<MenuItem<Item>> MenuItems;
    public int CurrentMenuItemIndex = -1;
    private static readonly string INDICATOR = " <";

    public Menu()
    {
        MenuItems = new List<MenuItem<Item>>();
    }

    private void IndicateMenuItem(int newMenuItemIndex)
    {
        if (CurrentMenuItemIndex >= 0 && CurrentMenuItemIndex < MenuItems.Count)
        {
            Text text = MenuItems[CurrentMenuItemIndex].DisplayText;
            text.text = text.text.Replace(INDICATOR, "");
        }

        if (newMenuItemIndex >= 0 && newMenuItemIndex < MenuItems.Count)
        {
            CurrentMenuItemIndex = newMenuItemIndex;
            Text text = MenuItems[CurrentMenuItemIndex].DisplayText;
            text.text = text.text + INDICATOR;
        }
    }

    public void Show(GameManager.Callback cancelCallback)
    {
        if (CurrentMenuItemIndex < 0 || CurrentMenuItemIndex >= MenuItems.Count)
        {
            IndicateMenuItem(0);
        }

        CancelCallback = cancelCallback;

        float x;
        if (GameManager.Cursor.transform.position.x >= GameManager.CurrentLevel.TerrainMap.GetLength(0) / 2)
        {
            x = GameManager.Cursor.transform.position.x - 1;
        }
        else
        {
            x = GameManager.Cursor.transform.position.x + 1;
        }

        transform.position = new Vector2(x, GameManager.Cursor.transform.position.y);
        transform.gameObject.SetActive(true);
        Focus();
    }

    public void Clear()
    {
        foreach (MenuItem<Item> menuItem in MenuItems)
        {
            Destroy(menuItem.DisplayText.gameObject);
        }
        CurrentMenuItemIndex = -1;
        MenuItems.Clear();
    }

    public void Hide()
    {
        transform.gameObject.SetActive(false);
        Clear();
    }

    public void AddMenuItem(Item type, Text displayText, GameManager.Callback menuItemCallback)
    {
        MenuItem<Item> menuItem = new MenuItem<Item>(type, displayText, menuItemCallback, this);
        MenuItems.Add(menuItem);
    }

    public void SelectMenuItem()
    {
        SelectMenuItem(CurrentMenuItemIndex);
    }

    private void SelectMenuItem(int index)
    {
        if (index < 0 || index >= MenuItems.Count)
        {
            return;
        }

        GameManager.Callback callback = MenuItems[index].MenuItemCallback;
        Item item = (MenuItems[index].ItemObject);
        callback(item);
    }

    public override void OnArrow(float horizontal, float vertical)
    {
        if (vertical == 0f)
        {
            return;
        }
        int newMenuItemIndex = (CurrentMenuItemIndex + MenuItems.Count - System.Math.Sign(vertical)) % MenuItems.Count;
        IndicateMenuItem(newMenuItemIndex);
    }

    public override void OnSubmit()
    {
        SelectMenuItem();
    }

    public override void OnCancel()
    {
        Hide();
        CancelCallback();
    }

    public class MenuItem<Item>
    {
        protected readonly Item itemObject;
        protected readonly GameManager.Callback menuItemCallback;
        protected readonly Menu menu;
        protected readonly Text displayText;

        public MenuItem(Item itemObject, Text displayText, GameManager.Callback callback, Menu menu)
        {
            this.itemObject = itemObject;
            this.displayText = Instantiate(displayText, menu.transform);
            menuItemCallback = callback;
            this.menu = menu;
        }

        public Item ItemObject
        {
            get
            {
                return itemObject;
            }
        }

        public GameManager.Callback MenuItemCallback
        {
            get
            {
                return menuItemCallback;
            }
        }

        public Menu Menu
        {
            get
            {
                return menu;
            }
        }

        public Text DisplayText
        {
            get
            {
                return displayText;
            }
        }
    }
}
