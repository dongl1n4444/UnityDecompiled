﻿namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class MenuUtils
    {
        [CompilerGenerated]
        private static GenericMenu.MenuFunction2 <>f__mg$cache0;

        public static void ExtractMenuItemWithPath(string menuString, GenericMenu menu, string replacementMenuString, UnityEngine.Object[] temporaryContext, int userData, Action<string, UnityEngine.Object[], int> onBeforeExecuteCallback, Action<string, UnityEngine.Object[], int> onAfterExecuteCallback)
        {
            MenuCallbackObject obj2 = new MenuCallbackObject {
                menuItemPath = menuString,
                temporaryContext = temporaryContext,
                onBeforeExecuteCallback = onBeforeExecuteCallback,
                onAfterExecuteCallback = onAfterExecuteCallback,
                userData = userData
            };
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new GenericMenu.MenuFunction2(MenuUtils.MenuCallback);
            }
            menu.AddItem(new GUIContent(replacementMenuString), false, <>f__mg$cache0, obj2);
        }

        public static void ExtractSubMenuWithPath(string path, GenericMenu menu, string replacementPath, UnityEngine.Object[] temporaryContext)
        {
            HashSet<string> set = new HashSet<string>(Unsupported.GetSubmenus(path));
            foreach (string str in Unsupported.GetSubmenusIncludingSeparators(path))
            {
                string replacementMenuString = replacementPath + str.Substring(path.Length);
                if (set.Contains(str))
                {
                    ExtractMenuItemWithPath(str, menu, replacementMenuString, temporaryContext, -1, null, null);
                }
            }
        }

        public static void MenuCallback(object callbackObject)
        {
            MenuCallbackObject obj2 = callbackObject as MenuCallbackObject;
            if (obj2.onBeforeExecuteCallback != null)
            {
                obj2.onBeforeExecuteCallback(obj2.menuItemPath, obj2.temporaryContext, obj2.userData);
            }
            if (obj2.temporaryContext != null)
            {
                EditorApplication.ExecuteMenuItemWithTemporaryContext(obj2.menuItemPath, obj2.temporaryContext);
            }
            else
            {
                EditorApplication.ExecuteMenuItem(obj2.menuItemPath);
            }
            if (obj2.onAfterExecuteCallback != null)
            {
                obj2.onAfterExecuteCallback(obj2.menuItemPath, obj2.temporaryContext, obj2.userData);
            }
        }

        private class MenuCallbackObject
        {
            public string menuItemPath;
            public Action<string, UnityEngine.Object[], int> onAfterExecuteCallback;
            public Action<string, UnityEngine.Object[], int> onBeforeExecuteCallback;
            public UnityEngine.Object[] temporaryContext;
            public int userData;
        }
    }
}

