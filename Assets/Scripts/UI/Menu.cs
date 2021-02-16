using System.Collections.Generic;
using Items;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
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
                Debug.LogErrorFormat("Invalid menu index: {0}", index);
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

        public override void OnInformation()
        {
            Debug.LogFormat("Menu.OnArrow not implemented");
        }

        public class MenuItem<Item>
        {
            public Item ItemObject { get; private set; }
            public GameManager.Callback MenuItemCallback { get; private set; }
            public Menu Menu { get; private set; }
            public Text DisplayText { get; private set; }

            public MenuItem(Item itemObject, Text displayText, GameManager.Callback callback, Menu menu)
            {
                ItemObject = itemObject;
                DisplayText = Instantiate(displayText, menu.transform);
                MenuItemCallback = callback;
                Menu = menu;
            }
        }
    }
}