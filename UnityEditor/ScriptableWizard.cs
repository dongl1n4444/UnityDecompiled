namespace UnityEditor
{
    using System;
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.Internal;

    /// <summary>
    /// <para>Derive from this class to create an editor wizard.</para>
    /// </summary>
    public class ScriptableWizard : EditorWindow
    {
        private string m_CreateButton = "Create";
        private string m_ErrorString = "";
        private string m_HelpString = "";
        private GenericInspector m_Inspector;
        private bool m_IsValid = true;
        private string m_OtherButton = "";
        private Vector2 m_ScrollPosition;

        public static T DisplayWizard<T>(string title) where T: ScriptableWizard
        {
            return DisplayWizard<T>(title, "Create", "");
        }

        public static T DisplayWizard<T>(string title, string createButtonName) where T: ScriptableWizard
        {
            return DisplayWizard<T>(title, createButtonName, "");
        }

        [ExcludeFromDocs]
        public static ScriptableWizard DisplayWizard(string title, Type klass)
        {
            string otherButtonName = "";
            string createButtonName = "Create";
            return DisplayWizard(title, klass, createButtonName, otherButtonName);
        }

        public static T DisplayWizard<T>(string title, string createButtonName, string otherButtonName) where T: ScriptableWizard
        {
            return (T) DisplayWizard(title, typeof(T), createButtonName, otherButtonName);
        }

        [ExcludeFromDocs]
        public static ScriptableWizard DisplayWizard(string title, Type klass, string createButtonName)
        {
            string otherButtonName = "";
            return DisplayWizard(title, klass, createButtonName, otherButtonName);
        }

        /// <summary>
        /// <para>Creates a wizard.</para>
        /// </summary>
        /// <param name="title">The title shown at the top of the wizard window.</param>
        /// <param name="klass">The class implementing the wizard. It has to derive from ScriptableWizard.</param>
        /// <param name="createButtonName">The text shown on the create button.</param>
        /// <param name="otherButtonName">The text shown on the optional other button. Leave this parameter out to leave the button out.</param>
        /// <returns>
        /// <para>The wizard.</para>
        /// </returns>
        public static ScriptableWizard DisplayWizard(string title, Type klass, [DefaultValue("\"Create\"")] string createButtonName, [DefaultValue("\"\"")] string otherButtonName)
        {
            ScriptableWizard wizard = ScriptableObject.CreateInstance(klass) as ScriptableWizard;
            wizard.m_CreateButton = createButtonName;
            wizard.m_OtherButton = otherButtonName;
            wizard.titleContent = new GUIContent(title);
            if (wizard != null)
            {
                wizard.InvokeWizardUpdate();
                wizard.ShowUtility();
            }
            return wizard;
        }

        /// <summary>
        /// <para>Will be called for drawing contents when the ScriptableWizard needs to update its GUI.</para>
        /// </summary>
        /// <returns>
        /// <para>Returns true if any property has been modified.</para>
        /// </returns>
        protected virtual bool DrawWizardGUI()
        {
            if (this.m_Inspector == null)
            {
                this.m_Inspector = ScriptableObject.CreateInstance<GenericInspector>();
                this.m_Inspector.hideFlags = HideFlags.HideAndDontSave;
                Object[] t = new Object[] { this };
                this.m_Inspector.InternalSetTargets(t);
            }
            return this.m_Inspector.DrawDefaultInspector();
        }

        private void InvokeWizardUpdate()
        {
            MethodInfo method = base.GetType().GetMethod("OnWizardUpdate", BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (method != null)
            {
                method.Invoke(this, null);
            }
        }

        private void OnDestroy()
        {
            Object.DestroyImmediate(this.m_Inspector);
        }

        private void OnGUI()
        {
            EditorGUIUtility.labelWidth = 150f;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandHeight(true) };
            GUILayout.Label(this.m_HelpString, EditorStyles.wordWrappedLabel, options);
            this.m_ScrollPosition = EditorGUILayout.BeginVerticalScrollView(this.m_ScrollPosition, false, GUI.skin.verticalScrollbar, "OL Box", new GUILayoutOption[0]);
            GUIUtility.GetControlID(0x9da9d, FocusType.Passive);
            bool flag = this.DrawWizardGUI();
            EditorGUILayout.EndScrollView();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            if (this.m_ErrorString != string.Empty)
            {
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MinHeight(32f) };
                GUILayout.Label(this.m_ErrorString, Styles.errorText, optionArray2);
            }
            else
            {
                GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.MinHeight(32f) };
                GUILayout.Label(string.Empty, optionArray3);
            }
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUI.enabled = this.m_IsValid;
            if (this.m_OtherButton != "")
            {
                GUILayoutOption[] optionArray4 = new GUILayoutOption[] { GUILayout.MinWidth(100f) };
                if (GUILayout.Button(this.m_OtherButton, optionArray4))
                {
                    MethodInfo method = base.GetType().GetMethod("OnWizardOtherButton", BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (method != null)
                    {
                        method.Invoke(this, null);
                        GUIUtility.ExitGUI();
                    }
                    else
                    {
                        Debug.LogError("OnWizardOtherButton has not been implemented in script");
                    }
                }
            }
            if (this.m_CreateButton != "")
            {
                GUILayoutOption[] optionArray5 = new GUILayoutOption[] { GUILayout.MinWidth(100f) };
                if (GUILayout.Button(this.m_CreateButton, optionArray5))
                {
                    MethodInfo info2 = base.GetType().GetMethod("OnWizardCreate", BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (info2 != null)
                    {
                        info2.Invoke(this, null);
                    }
                    else
                    {
                        Debug.LogError("OnWizardCreate has not been implemented in script");
                    }
                    base.Close();
                    GUIUtility.ExitGUI();
                }
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            if (flag)
            {
                this.InvokeWizardUpdate();
            }
        }

        /// <summary>
        /// <para>Allows you to set the text shown on the create button of the wizard.</para>
        /// </summary>
        public string createButtonName
        {
            get
            {
                return this.m_CreateButton;
            }
            set
            {
                string str;
                if (value != null)
                {
                    str = value;
                }
                else
                {
                    str = string.Empty;
                }
                if (this.m_CreateButton != str)
                {
                    this.m_CreateButton = str;
                    base.Repaint();
                }
            }
        }

        /// <summary>
        /// <para>Allows you to set the error text of the wizard.</para>
        /// </summary>
        public string errorString
        {
            get
            {
                return this.m_ErrorString;
            }
            set
            {
                string str;
                if (value != null)
                {
                    str = value;
                }
                else
                {
                    str = string.Empty;
                }
                if (this.m_ErrorString != str)
                {
                    this.m_ErrorString = str;
                    base.Repaint();
                }
            }
        }

        /// <summary>
        /// <para>Allows you to set the help text of the wizard.</para>
        /// </summary>
        public string helpString
        {
            get
            {
                return this.m_HelpString;
            }
            set
            {
                string str;
                if (value != null)
                {
                    str = value;
                }
                else
                {
                    str = string.Empty;
                }
                if (this.m_HelpString != str)
                {
                    this.m_HelpString = str;
                    base.Repaint();
                }
            }
        }

        /// <summary>
        /// <para>Allows you to enable and disable the wizard create button, so that the user can not click it.</para>
        /// </summary>
        public bool isValid
        {
            get
            {
                return this.m_IsValid;
            }
            set
            {
                this.m_IsValid = value;
            }
        }

        /// <summary>
        /// <para>Allows you to set the text shown on the optional other button of the wizard. Leave this parameter out to leave the button out.</para>
        /// </summary>
        public string otherButtonName
        {
            get
            {
                return this.m_OtherButton;
            }
            set
            {
                string str;
                if (value != null)
                {
                    str = value;
                }
                else
                {
                    str = string.Empty;
                }
                if (this.m_OtherButton != str)
                {
                    this.m_OtherButton = str;
                    base.Repaint();
                }
            }
        }

        private class Styles
        {
            public static string box = "Wizard Box";
            public static string errorText = "Wizard Error";
        }
    }
}

