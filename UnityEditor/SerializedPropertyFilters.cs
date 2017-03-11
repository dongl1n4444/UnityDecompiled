namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class SerializedPropertyFilters
    {
        internal static readonly None s_FilterNone = new None();

        internal interface IFilter
        {
            bool Active();
            void DeserializeState(string state);
            bool Filter(SerializedProperty prop);
            void OnGUI(Rect r);
            string SerializeState();
        }

        internal sealed class Name : SerializedPropertyFilters.String
        {
            public bool Filter(string str) => 
                (str.IndexOf(base.m_Text, 0, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        internal sealed class None : SerializedPropertyFilters.IFilter
        {
            public bool Active() => 
                false;

            public void DeserializeState(string state)
            {
            }

            public bool Filter(SerializedProperty prop) => 
                true;

            public void OnGUI(Rect r)
            {
            }

            public string SerializeState() => 
                null;
        }

        internal abstract class SerializableFilter : SerializedPropertyFilters.IFilter
        {
            protected SerializableFilter()
            {
            }

            public abstract bool Active();
            public void DeserializeState(string state)
            {
                JsonUtility.FromJsonOverwrite(state, this);
            }

            public abstract bool Filter(SerializedProperty prop);
            public abstract void OnGUI(Rect r);
            public string SerializeState() => 
                JsonUtility.ToJson(this);
        }

        internal class String : SerializedPropertyFilters.SerializableFilter
        {
            [SerializeField]
            protected string m_Text = "";

            public override bool Active() => 
                !string.IsNullOrEmpty(this.m_Text);

            public override bool Filter(SerializedProperty prop) => 
                (prop.stringValue.IndexOf(this.m_Text, 0, StringComparison.OrdinalIgnoreCase) >= 0);

            public override void OnGUI(Rect r)
            {
                r.width -= 15f;
                this.m_Text = EditorGUI.TextField(r, GUIContent.none, this.m_Text, Styles.searchField);
                r.x += r.width;
                r.width = 15f;
                bool flag = this.m_Text != "";
                if (GUI.Button(r, GUIContent.none, !flag ? Styles.searchFieldCancelButtonEmpty : Styles.searchFieldCancelButton) && flag)
                {
                    this.m_Text = "";
                    GUIUtility.keyboardControl = 0;
                }
            }

            private static class Styles
            {
                public static readonly GUIStyle searchField = "SearchTextField";
                public static readonly GUIStyle searchFieldCancelButton = "SearchCancelButton";
                public static readonly GUIStyle searchFieldCancelButtonEmpty = "SearchCancelButtonEmpty";
            }
        }
    }
}

