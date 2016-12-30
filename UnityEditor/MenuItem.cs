namespace UnityEditor
{
    using System;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The MenuItem attribute allows you to add menu items to the main menu and inspector context menus.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=true), RequiredByNativeCode]
    public sealed class MenuItem : Attribute
    {
        public string menuItem;
        public int priority;
        public bool validate;

        /// <summary>
        /// <para>Creates a menu item and invokes the static function following it, when the menu item is selected.</para>
        /// </summary>
        /// <param name="itemName"></param>
        public MenuItem(string itemName) : this(itemName, false)
        {
        }

        /// <summary>
        /// <para>Creates a menu item and invokes the static function following it, when the menu item is selected.</para>
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="isValidateFunction"></param>
        public MenuItem(string itemName, bool isValidateFunction) : this(itemName, isValidateFunction, !itemName.StartsWith("GameObject/Create Other") ? 0x3e8 : 10)
        {
        }

        /// <summary>
        /// <para>Creates a menu item and invokes the static function following it, when the menu item is selected.</para>
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="isValidateFunction"></param>
        /// <param name="priority"></param>
        public MenuItem(string itemName, bool isValidateFunction, int priority) : this(itemName, isValidateFunction, priority, false)
        {
        }

        internal MenuItem(string itemName, bool isValidateFunction, int priority, bool internalMenu)
        {
            if (internalMenu)
            {
                this.menuItem = "internal:" + itemName;
            }
            else
            {
                this.menuItem = itemName;
            }
            this.validate = isValidateFunction;
            this.priority = priority;
        }
    }
}

