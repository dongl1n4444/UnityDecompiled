namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class SerializedPropertyFilters
    {
        internal static readonly None s_FilterNone = new None();

        internal class Checkbox : SerializedPropertyFilters.SerializableFilter
        {
            [SerializeField]
            private bool m_Enabled;
            [SerializeField]
            private bool m_Mixed = true;

            public override bool Active() => 
                !this.m_Mixed;

            public override bool Filter(SerializedProperty prop) => 
                (prop.boolValue == this.m_Enabled);

            public override void OnGUI(Rect r)
            {
                float num = (r.width / 2f) - 8f;
                r.x += (num < 0f) ? 0f : num;
                bool enabled = this.m_Enabled;
                this.m_Enabled = EditorGUI.Toggle(r, GUIContent.none, this.m_Enabled, !this.m_Mixed ? EditorStyles.toggle : EditorStyles.toggleMixed);
                if (enabled != this.m_Enabled)
                {
                    if (this.m_Mixed)
                    {
                        this.m_Mixed = false;
                        this.m_Enabled = true;
                    }
                    else if (this.m_Enabled)
                    {
                        this.m_Mixed = true;
                    }
                }
            }
        }

        internal class FloatValue : SerializedPropertyFilters.SerializableFilter
        {
            [SerializeField]
            private float m_Max;
            [SerializeField]
            private string m_MaxString = "";
            [SerializeField]
            private float m_Min;
            [SerializeField]
            private string m_MinString = "";

            public override bool Active() => 
                ((this.m_MinString != "") || (this.m_MaxString != ""));

            public override bool Filter(SerializedProperty prop) => 
                ((prop.floatValue >= this.m_Min) && (prop.floatValue <= this.m_Max));

            public override void OnGUI(Rect r)
            {
                r.width *= 0.5f;
                EditorGUI.LabelField(r, GUIContent.Temp(string.Empty, "Minimum value"));
                string s = EditorGUI.TextField(r, this.m_MinString);
                r.x += r.width;
                EditorGUI.LabelField(r, GUIContent.Temp(string.Empty, "Maximum value"));
                string str2 = EditorGUI.TextField(r, this.m_MaxString);
                if (s == "")
                {
                    this.m_MinString = "";
                    this.m_Min = float.MinValue;
                }
                else
                {
                    float num;
                    if (float.TryParse(s, out num))
                    {
                        this.m_Min = num;
                        this.m_MinString = this.m_Min.ToString();
                    }
                }
                if (str2 == "")
                {
                    this.m_MaxString = "";
                    this.m_Max = float.MaxValue;
                }
                else
                {
                    float num2;
                    if (float.TryParse(str2, out num2))
                    {
                        this.m_Max = num2;
                        this.m_MaxString = this.m_Max.ToString();
                    }
                }
            }
        }

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

