namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>The GenericMenu lets you create a custom context and dropdown menus.</para>
    /// </summary>
    public sealed class GenericMenu
    {
        private ArrayList menuItems = new ArrayList();

        /// <summary>
        /// <para>Add a disabled item to the menu.</para>
        /// </summary>
        /// <param name="content">The GUIContent to display as a disabled menu item.</param>
        public void AddDisabledItem(GUIContent content)
        {
            this.menuItems.Add(new MenuItem(content, false, false, null));
        }

        public void AddItem(GUIContent content, bool on, MenuFunction func)
        {
            this.menuItems.Add(new MenuItem(content, false, on, func));
        }

        public void AddItem(GUIContent content, bool on, MenuFunction2 func, object userData)
        {
            this.menuItems.Add(new MenuItem(content, false, on, func, userData));
        }

        /// <summary>
        /// <para>Add a seperator item to the menu.</para>
        /// </summary>
        /// <param name="path">The path to the submenu, if adding a separator to a submenu. When adding a separator to the top level of a menu, use an empty string as the path.</param>
        public void AddSeparator(string path)
        {
            this.menuItems.Add(new MenuItem(new GUIContent(path), true, false, null));
        }

        private void CatchMenu(object userData, string[] options, int selected)
        {
            MenuItem item = (MenuItem) this.menuItems[selected];
            if (item.func2 != null)
            {
                item.func2(item.userData);
            }
            else if (item.func != null)
            {
                item.func();
            }
        }

        /// <summary>
        /// <para>Show the menu at the given screen rect.</para>
        /// </summary>
        /// <param name="position">The position at which to show the menu.</param>
        public void DropDown(Rect position)
        {
            string[] options = new string[this.menuItems.Count];
            bool[] enabled = new bool[this.menuItems.Count];
            ArrayList list = new ArrayList();
            bool[] separator = new bool[this.menuItems.Count];
            for (int i = 0; i < this.menuItems.Count; i++)
            {
                MenuItem item = (MenuItem) this.menuItems[i];
                options[i] = item.content.text;
                enabled[i] = (item.func != null) || (item.func2 != null);
                separator[i] = item.separator;
                if (item.on)
                {
                    list.Add(i);
                }
            }
            EditorUtility.DisplayCustomMenuWithSeparators(position, options, enabled, separator, (int[]) list.ToArray(typeof(int)), new EditorUtility.SelectMenuItemFunction(this.CatchMenu), null, true);
        }

        /// <summary>
        /// <para>Get number of items in the menu.</para>
        /// </summary>
        /// <returns>
        /// <para>The number of items in the menu.</para>
        /// </returns>
        public int GetItemCount()
        {
            return this.menuItems.Count;
        }

        internal void Popup(Rect position, int selectedIndex)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                this.DropDown(position);
            }
            else
            {
                this.DropDown(position);
            }
        }

        /// <summary>
        /// <para>Show the menu under the mouse when right-clicked.</para>
        /// </summary>
        public void ShowAsContext()
        {
            if (Event.current != null)
            {
                this.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0f, 0f));
            }
        }

        /// <summary>
        /// <para>Callback function, called when a menu item is selected.</para>
        /// </summary>
        public delegate void MenuFunction();

        /// <summary>
        /// <para>Callback function with user data, called when a menu item is selected.</para>
        /// </summary>
        /// <param name="userData">The data to pass through to the callback function.</param>
        public delegate void MenuFunction2(object userData);

        private sealed class MenuItem
        {
            public GUIContent content;
            public GenericMenu.MenuFunction func;
            public GenericMenu.MenuFunction2 func2;
            public bool on;
            public bool separator;
            public object userData;

            public MenuItem(GUIContent _content, bool _separator, bool _on, GenericMenu.MenuFunction _func)
            {
                this.content = _content;
                this.separator = _separator;
                this.on = _on;
                this.func = _func;
            }

            public MenuItem(GUIContent _content, bool _separator, bool _on, GenericMenu.MenuFunction2 _func, object _userData)
            {
                this.content = _content;
                this.separator = _separator;
                this.on = _on;
                this.func2 = _func;
                this.userData = _userData;
            }
        }
    }
}

