namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class AssetSelectionPopupMenu
    {
        [CompilerGenerated]
        private static Comparison<UnityEngine.Object> <>f__am$cache0;
        [CompilerGenerated]
        private static GenericMenu.MenuFunction2 <>f__mg$cache0;

        private static List<UnityEngine.Object> FindAssetsOfType(string[] classNames)
        {
            HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
            SearchFilter filter = new SearchFilter {
                classNames = classNames
            };
            property.SetSearchFilter(filter);
            List<UnityEngine.Object> list = new List<UnityEngine.Object>();
            while (property.Next(null))
            {
                list.Add(property.pptrValue);
            }
            return list;
        }

        private static void SelectCallback(object userData)
        {
            UnityEngine.Object obj2 = userData as UnityEngine.Object;
            if (obj2 != null)
            {
                Selection.activeInstanceID = obj2.GetInstanceID();
            }
        }

        public static void Show(Rect buttonRect, string[] classNames, int initialSelectedInstanceID)
        {
            GenericMenu menu = new GenericMenu();
            List<UnityEngine.Object> source = FindAssetsOfType(classNames);
            if (source.Any<UnityEngine.Object>())
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = (result1, result2) => EditorUtility.NaturalCompare(result1.name, result2.name);
                }
                source.Sort(<>f__am$cache0);
                foreach (UnityEngine.Object obj2 in source)
                {
                    GUIContent content = new GUIContent(obj2.name);
                    bool on = obj2.GetInstanceID() == initialSelectedInstanceID;
                    if (<>f__mg$cache0 == null)
                    {
                        <>f__mg$cache0 = new GenericMenu.MenuFunction2(AssetSelectionPopupMenu.SelectCallback);
                    }
                    menu.AddItem(content, on, <>f__mg$cache0, obj2);
                }
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("No Audio Mixers found in this project"));
            }
            menu.DropDown(buttonRect);
        }
    }
}

