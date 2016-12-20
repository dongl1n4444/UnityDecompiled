namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class AssetBundleNameGUI
    {
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache0;
        private static readonly GUIContent kAssetBundleName = new GUIContent("AssetBundle");
        private static readonly int kAssetBundleNameFieldIdHash = "AssetBundleNameFieldHash".GetHashCode();
        private static readonly int kAssetBundleVariantFieldIdHash = "AssetBundleVariantFieldHash".GetHashCode();
        private bool m_ShowAssetBundleNameTextField = false;
        private bool m_ShowAssetBundleVariantTextField = false;

        private void AssetBundlePopup(Rect rect, int id, IEnumerable<Object> assets, bool isVariant)
        {
            bool flag;
            List<string> list = new List<string> { 
                "None",
                ""
            };
            IEnumerable<string> source = this.GetAssetBundlesFromAssets(assets, isVariant, out flag);
            string[] collection = !isVariant ? AssetDatabase.GetAllAssetBundleNamesWithoutVariant() : AssetDatabase.GetAllAssetBundleVariants();
            list.AddRange(collection);
            list.Add("");
            int count = list.Count;
            list.Add("New...");
            int num2 = -1;
            int num3 = -1;
            if (!isVariant)
            {
                num2 = list.Count;
                list.Add("Remove Unused Names");
                num3 = list.Count;
                if (Enumerable.Count<string>(source) != 0)
                {
                    list.Add("Filter Selected Name" + (!flag ? "" : "s"));
                }
            }
            int selected = 0;
            string str = Enumerable.FirstOrDefault<string>(source);
            if (!string.IsNullOrEmpty(str))
            {
                selected = list.IndexOf(str);
            }
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = flag;
            selected = EditorGUI.DoPopup(rect, id, selected, EditorGUIUtility.TempContent(list.ToArray()), Styles.popup);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                if (selected == 0)
                {
                    this.SetAssetBundleForAssets(assets, null, isVariant);
                }
                else if (selected == count)
                {
                    this.ShowNewAssetBundleField(isVariant);
                }
                else if (selected == num2)
                {
                    AssetDatabase.RemoveUnusedAssetBundleNames();
                }
                else if (selected == num3)
                {
                    this.FilterSelected(source);
                }
                else
                {
                    this.SetAssetBundleForAssets(assets, list[selected], isVariant);
                }
            }
        }

        private void AssetBundleTextField(Rect rect, int id, IEnumerable<Object> assets, bool isVariant)
        {
            Color cursorColor = GUI.skin.settings.cursorColor;
            GUI.skin.settings.cursorColor = Styles.cursorColor;
            EditorGUI.BeginChangeCheck();
            string name = EditorGUI.DelayedTextFieldInternal(rect, id, GUIContent.none, "", null, Styles.textField);
            if (EditorGUI.EndChangeCheck())
            {
                this.SetAssetBundleForAssets(assets, name, isVariant);
                this.ShowAssetBundlePopup();
            }
            GUI.skin.settings.cursorColor = cursorColor;
            if (!EditorGUI.IsEditingTextField() && (Event.current.type != EventType.Layout))
            {
                this.ShowAssetBundlePopup();
            }
        }

        private void FilterSelected(IEnumerable<string> assetBundleNames)
        {
            SearchFilter searchFilter = new SearchFilter();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<string, bool>(null, (IntPtr) <FilterSelected>m__0);
            }
            searchFilter.assetBundleNames = Enumerable.ToArray<string>(Enumerable.Where<string>(assetBundleNames, <>f__am$cache0));
            if (ProjectBrowser.s_LastInteractedProjectBrowser != null)
            {
                ProjectBrowser.s_LastInteractedProjectBrowser.SetSearch(searchFilter);
            }
            else
            {
                Debug.LogWarning("No Project Browser found to apply AssetBundle filter.");
            }
        }

        private IEnumerable<string> GetAssetBundlesFromAssets(IEnumerable<Object> assets, bool isVariant, out bool isMixed)
        {
            HashSet<string> set = new HashSet<string>();
            string str = null;
            isMixed = false;
            foreach (Object obj2 in assets)
            {
                if (!(obj2 is MonoScript))
                {
                    AssetImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(obj2));
                    if (atPath != null)
                    {
                        string str2 = !isVariant ? atPath.assetBundleName : atPath.assetBundleVariant;
                        if ((str != null) && (str != str2))
                        {
                            isMixed = true;
                        }
                        str = str2;
                        if (!string.IsNullOrEmpty(str2))
                        {
                            set.Add(str2);
                        }
                    }
                }
            }
            return set;
        }

        public void OnAssetBundleNameGUI(IEnumerable<Object> assets)
        {
            EditorGUIUtility.labelWidth = 90f;
            Rect position = EditorGUILayout.GetControlRect(true, 16f, new GUILayoutOption[0]);
            Rect rect2 = position;
            position.width *= 0.8f;
            rect2.xMin += position.width + 5f;
            int id = GUIUtility.GetControlID(kAssetBundleNameFieldIdHash, FocusType.Passive, position);
            position = EditorGUI.PrefixLabel(position, id, kAssetBundleName, Styles.label);
            if (this.m_ShowAssetBundleNameTextField)
            {
                this.AssetBundleTextField(position, id, assets, false);
            }
            else
            {
                this.AssetBundlePopup(position, id, assets, false);
            }
            id = GUIUtility.GetControlID(kAssetBundleVariantFieldIdHash, FocusType.Passive, rect2);
            if (this.m_ShowAssetBundleVariantTextField)
            {
                this.AssetBundleTextField(rect2, id, assets, true);
            }
            else
            {
                this.AssetBundlePopup(rect2, id, assets, true);
            }
        }

        private void SetAssetBundleForAssets(IEnumerable<Object> assets, string name, bool isVariant)
        {
            bool flag = false;
            foreach (Object obj2 in assets)
            {
                if (!(obj2 is MonoScript))
                {
                    AssetImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(obj2));
                    if (atPath != null)
                    {
                        if (isVariant)
                        {
                            atPath.assetBundleVariant = name;
                        }
                        else
                        {
                            atPath.assetBundleName = name;
                        }
                        flag = true;
                    }
                }
            }
            if (flag)
            {
                EditorApplication.Internal_CallAssetBundleNameChanged();
            }
        }

        private void ShowAssetBundlePopup()
        {
            this.m_ShowAssetBundleNameTextField = false;
            this.m_ShowAssetBundleVariantTextField = false;
        }

        private void ShowNewAssetBundleField(bool isVariant)
        {
            this.m_ShowAssetBundleNameTextField = !isVariant;
            this.m_ShowAssetBundleVariantTextField = isVariant;
            EditorGUIUtility.editingTextField = true;
        }

        private class Styles
        {
            public static Color cursorColor = s_DarkSkin.settings.cursorColor;
            public static GUIStyle label = GetStyle("ControlLabel");
            public static GUIStyle popup = GetStyle("MiniPopup");
            private static GUISkin s_DarkSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
            public static GUIStyle textField = GetStyle("textField");

            private static GUIStyle GetStyle(string name)
            {
                return new GUIStyle(s_DarkSkin.GetStyle(name));
            }
        }
    }
}

