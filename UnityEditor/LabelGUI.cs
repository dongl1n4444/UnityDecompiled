﻿namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class LabelGUI
    {
        [CompilerGenerated]
        private static Func<PopupList.ListElement, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<PopupList.ListElement, GUIContent> <>f__am$cache1;
        private PopupList.InputData m_AssetLabels;
        private string m_ChangedLabel;
        private bool m_ChangeWasAdd = false;
        private HashSet<Object> m_CurrentAssetsSet;
        private bool m_CurrentChanged = false;
        private bool m_IgnoreNextAssetLabelsChangedCall = false;
        private static Action<Object> s_AssetLabelsForObjectChangedDelegates;
        private static int s_MaxShownLabels = 10;

        public void AssetLabelListCallback(PopupList.ListElement element)
        {
            this.m_ChangedLabel = element.text;
            element.selected = !element.selected;
            this.m_ChangeWasAdd = element.selected;
            element.partiallySelected = false;
            this.m_CurrentChanged = true;
            this.SaveLabels();
            InspectorWindow.RepaintAllInspectors();
        }

        public void AssetLabelsChangedForObject(Object asset)
        {
            if ((!this.m_IgnoreNextAssetLabelsChangedCall && (this.m_CurrentAssetsSet != null)) && this.m_CurrentAssetsSet.Contains(asset))
            {
                this.m_AssetLabels = null;
            }
            this.m_IgnoreNextAssetLabelsChangedCall = false;
        }

        private void DrawLabelList(bool partiallySelected, float xMax)
        {
            <DrawLabelList>c__AnonStorey1 storey = new <DrawLabelList>c__AnonStorey1 {
                partiallySelected = partiallySelected
            };
            GUIStyle style = !storey.partiallySelected ? EditorStyles.assetLabel : EditorStyles.assetLabelPartial;
            Event current = Event.current;
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<PopupList.ListElement, string>(null, (IntPtr) <DrawLabelList>m__0);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<PopupList.ListElement, GUIContent>(null, (IntPtr) <DrawLabelList>m__1);
            }
            foreach (GUIContent content in Enumerable.Select<PopupList.ListElement, GUIContent>(Enumerable.OrderBy<PopupList.ListElement, string>(Enumerable.Where<PopupList.ListElement>(this.m_AssetLabels.m_ListElements, new Func<PopupList.ListElement, bool>(storey, (IntPtr) this.<>m__0)), <>f__am$cache0), <>f__am$cache1).Take<GUIContent>(s_MaxShownLabels))
            {
                Rect position = GUILayoutUtility.GetRect(content, style);
                if ((Event.current.type == EventType.Repaint) && (position.xMax >= xMax))
                {
                    break;
                }
                GUI.Label(position, content, style);
                if ((((position.xMax <= xMax) && (current.type == EventType.MouseDown)) && (position.Contains(current.mousePosition) && (current.button == 0))) && GUI.enabled)
                {
                    current.Use();
                    position.x = xMax;
                    PopupWindow.Show(position, new PopupList(this.m_AssetLabels, content.text), null, ShowMode.PopupMenuWithKeyboardFocus);
                }
            }
        }

        private void GetLabelsForAssets(Object[] assets, out List<string> all, out List<string> partial)
        {
            all = new List<string>();
            partial = new List<string>();
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            foreach (Object obj2 in assets)
            {
                string[] labels = AssetDatabase.GetLabels(obj2);
                foreach (string str in labels)
                {
                    dictionary[str] = !dictionary.ContainsKey(str) ? 1 : (dictionary[str] + 1);
                }
            }
            foreach (KeyValuePair<string, int> pair in dictionary)
            {
                ((pair.Value != assets.Length) ? partial : all).Add(pair.Key);
            }
        }

        public void InitLabelCache(Object[] assets)
        {
            HashSet<Object> other = new HashSet<Object>(assets);
            if ((this.m_CurrentAssetsSet == null) || !this.m_CurrentAssetsSet.SetEquals(other))
            {
                List<string> list;
                List<string> list2;
                this.GetLabelsForAssets(assets, out list, out list2);
                PopupList.InputData data = new PopupList.InputData {
                    m_CloseOnSelection = false,
                    m_AllowCustom = true,
                    m_OnSelectCallback = new PopupList.OnSelectCallback(this.AssetLabelListCallback),
                    m_MaxCount = 15,
                    m_SortAlphabetically = true
                };
                this.m_AssetLabels = data;
                Dictionary<string, float> allLabels = AssetDatabase.GetAllLabels();
                using (Dictionary<string, float>.Enumerator enumerator = allLabels.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        <InitLabelCache>c__AnonStorey0 storey = new <InitLabelCache>c__AnonStorey0 {
                            pair = enumerator.Current
                        };
                        PopupList.ListElement element = this.m_AssetLabels.NewOrMatchingElement(storey.pair.Key);
                        if (element.filterScore < storey.pair.Value)
                        {
                            element.filterScore = storey.pair.Value;
                        }
                        element.selected = Enumerable.Any<string>(list, new Func<string, bool>(storey, (IntPtr) this.<>m__0));
                        element.partiallySelected = Enumerable.Any<string>(list2, new Func<string, bool>(storey, (IntPtr) this.<>m__1));
                    }
                }
            }
            this.m_CurrentAssetsSet = other;
            this.m_CurrentChanged = false;
        }

        public void InvalidateLabels()
        {
            this.m_AssetLabels = null;
            this.m_CurrentAssetsSet = null;
        }

        public void OnDisable()
        {
            s_AssetLabelsForObjectChangedDelegates = (Action<Object>) Delegate.Remove(s_AssetLabelsForObjectChangedDelegates, new Action<Object>(this.AssetLabelsChangedForObject));
            EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.InvalidateLabels));
            this.SaveLabels();
        }

        public void OnEnable()
        {
            s_AssetLabelsForObjectChangedDelegates = (Action<Object>) Delegate.Combine(s_AssetLabelsForObjectChangedDelegates, new Action<Object>(this.AssetLabelsChangedForObject));
            EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.InvalidateLabels));
        }

        public void OnLabelGUI(Object[] assets)
        {
            this.InitLabelCache(assets);
            float minWidth = 1f;
            float num2 = 2f;
            float pixels = 3f;
            float num4 = 5f;
            GUIStyle assetLabelIcon = EditorStyles.assetLabelIcon;
            float num5 = (assetLabelIcon.margin.left + assetLabelIcon.fixedWidth) + num2;
            GUILayout.Space(pixels);
            Rect rect = GUILayoutUtility.GetRect(0f, 10240f, (float) 0f, (float) 0f);
            rect.width -= num5;
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutUtility.GetRect(minWidth, minWidth, (float) 0f, (float) 0f);
            this.DrawLabelList(false, rect.xMax);
            this.DrawLabelList(true, rect.xMax);
            GUILayout.FlexibleSpace();
            Rect position = GUILayoutUtility.GetRect(assetLabelIcon.fixedWidth, assetLabelIcon.fixedWidth, (float) (assetLabelIcon.fixedHeight + num4), (float) (assetLabelIcon.fixedHeight + num4));
            position.x = rect.xMax + assetLabelIcon.margin.left;
            if (EditorGUI.ButtonMouseDown(position, GUIContent.none, FocusType.Passive, assetLabelIcon))
            {
                PopupWindow.Show(position, new PopupList(this.m_AssetLabels), null, ShowMode.PopupMenuWithKeyboardFocus);
            }
            EditorGUILayout.EndHorizontal();
        }

        public void OnLostFocus()
        {
            this.SaveLabels();
        }

        public void SaveLabels()
        {
            if ((this.m_CurrentChanged && (this.m_AssetLabels != null)) && (this.m_CurrentAssetsSet != null))
            {
                bool flag = false;
                foreach (Object obj2 in this.m_CurrentAssetsSet)
                {
                    bool flag2 = false;
                    List<string> list = AssetDatabase.GetLabels(obj2).ToList<string>();
                    if (this.m_ChangeWasAdd)
                    {
                        if (!list.Contains(this.m_ChangedLabel))
                        {
                            list.Add(this.m_ChangedLabel);
                            flag2 = true;
                        }
                    }
                    else if (list.Contains(this.m_ChangedLabel))
                    {
                        list.Remove(this.m_ChangedLabel);
                        flag2 = true;
                    }
                    if (flag2)
                    {
                        AssetDatabase.SetLabels(obj2, list.ToArray());
                        if (s_AssetLabelsForObjectChangedDelegates != null)
                        {
                            this.m_IgnoreNextAssetLabelsChangedCall = true;
                            s_AssetLabelsForObjectChangedDelegates(obj2);
                        }
                        flag = true;
                    }
                }
                if (flag)
                {
                    EditorApplication.Internal_CallAssetLabelsHaveChanged();
                }
                this.m_CurrentChanged = false;
            }
        }

        [CompilerGenerated]
        private sealed class <DrawLabelList>c__AnonStorey1
        {
            internal bool partiallySelected;

            internal bool <>m__0(PopupList.ListElement i) => 
                (!this.partiallySelected ? i.selected : i.partiallySelected);
        }

        [CompilerGenerated]
        private sealed class <InitLabelCache>c__AnonStorey0
        {
            internal KeyValuePair<string, float> pair;

            internal bool <>m__0(string label) => 
                string.Equals(label, this.pair.Key, StringComparison.OrdinalIgnoreCase);

            internal bool <>m__1(string label) => 
                string.Equals(label, this.pair.Key, StringComparison.OrdinalIgnoreCase);
        }
    }
}

