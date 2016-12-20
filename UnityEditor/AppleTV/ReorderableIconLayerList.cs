namespace UnityEditor.AppleTV
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    internal class ReorderableIconLayerList
    {
        public string headerString = "";
        private const int kIconSpacing = 6;
        private const int kSlotSize = 0x60;
        public int m_ImageHeight = 20;
        public int m_ImageWidth = 20;
        private ReorderableList m_List = new ReorderableList(new List<Texture2D>(), typeof(Texture2D));
        public int minItems = 1;
        public ChangedCallbackDelegate onChangedCallback = null;

        public ReorderableIconLayerList()
        {
            this.m_List.onAddCallback = new ReorderableList.AddCallbackDelegate(this.OnAdd);
            this.m_List.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.OnRemove);
            this.m_List.onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.OnChange);
            this.m_List.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.OnElementDraw);
            this.m_List.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.OnHeaderDraw);
            this.m_List.onCanRemoveCallback = new ReorderableList.CanRemoveCallbackDelegate(this.OnCanRemove);
            this.UpdateElementHeight();
        }

        public void DoLayoutList()
        {
            this.m_List.DoLayoutList();
        }

        private void OnAdd(ReorderableList list)
        {
            this.textures.Add(null);
            this.m_List.index = this.textures.Count - 1;
            this.OnChange(list);
        }

        private bool OnCanRemove(ReorderableList list)
        {
            if (list.count <= this.minItems)
            {
                return false;
            }
            return true;
        }

        private void OnChange(ReorderableList list)
        {
            if (this.onChangedCallback != null)
            {
                this.onChangedCallback(this);
            }
        }

        private void OnElementDraw(Rect rect, int index, bool isActive, bool isFocused)
        {
            string text = string.Format(LocalizationDatabase.GetLocalizedString("Layer {0}"), index);
            float num = Mathf.Min(rect.width, ((EditorGUIUtility.labelWidth + 4f) + 96f) + 6f);
            GUI.Label(new Rect(rect.x, rect.y, (num - 96f) - 6f, 20f), text);
            int num2 = 0x60;
            int num3 = (int) ((((float) this.m_ImageHeight) / ((float) this.m_ImageWidth)) * 96f);
            Rect position = new Rect(((rect.x + num) - 96f) - 6f, rect.y, (float) num2, (float) num3);
            EditorGUI.BeginChangeCheck();
            this.textures[index] = (Texture2D) EditorGUI.ObjectField(position, this.textures[index], typeof(Texture2D), false);
            if (EditorGUI.EndChangeCheck())
            {
                this.OnChange(this.m_List);
            }
        }

        private void OnHeaderDraw(Rect rect)
        {
            GUI.Label(rect, LocalizationDatabase.GetLocalizedString(this.headerString), EditorStyles.label);
        }

        private void OnRemove(ReorderableList list)
        {
            this.textures.RemoveAt(list.index);
            list.index = 0;
            this.OnChange(list);
        }

        public void SetImageSize(int width, int height)
        {
            this.m_ImageWidth = width;
            this.m_ImageHeight = height;
            this.UpdateElementHeight();
        }

        private void UpdateElementHeight()
        {
            this.m_List.elementHeight = 96f * (((float) this.m_ImageHeight) / ((float) this.m_ImageWidth));
        }

        public List<Texture2D> textures
        {
            get
            {
                return (List<Texture2D>) this.m_List.list;
            }
            set
            {
                this.m_List.list = value;
            }
        }

        public delegate void ChangedCallbackDelegate(ReorderableIconLayerList list);
    }
}

