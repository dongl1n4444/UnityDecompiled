namespace UnityEditor.Android
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    internal class AndroidKeystoreWindow : EditorWindow
    {
        private string m_Alias;
        private string m_City;
        private string m_Country;
        private string m_Keystore;
        private string m_Name;
        private string m_Organization;
        private string m_OrgUnit;
        private string m_PassConfirm;
        private string m_Password;
        private string m_State;
        private string m_StorePass;
        private int m_Validity = 50;
        private static string[] s_AvailableKeyalias = null;
        private static string s_CurrentKeystore;

        private bool CreateKey(string keystore, string storepass)
        {
            string[] array = new string[0];
            if (this.m_Name.Length != 0)
            {
                ArrayUtility.Add<string>(ref array, "CN=" + escapeDName(this.m_Name));
            }
            if (this.m_OrgUnit.Length != 0)
            {
                ArrayUtility.Add<string>(ref array, "OU=" + escapeDName(this.m_OrgUnit));
            }
            if (this.m_Organization.Length != 0)
            {
                ArrayUtility.Add<string>(ref array, "O=" + escapeDName(this.m_Organization));
            }
            if (this.m_City.Length != 0)
            {
                ArrayUtility.Add<string>(ref array, "L=" + escapeDName(this.m_City));
            }
            if (this.m_State.Length != 0)
            {
                ArrayUtility.Add<string>(ref array, "ST=" + escapeDName(this.m_State));
            }
            if (this.m_Country.Length != 0)
            {
                ArrayUtility.Add<string>(ref array, "C=" + escapeDName(this.m_Country));
            }
            string dname = string.Join(", ", array);
            try
            {
                AndroidSDKTools.GetInstanceOrThrowException().CreateKey(keystore, storepass, this.m_Alias, this.m_Password, dname, this.m_Validity * 0x16d);
                s_AvailableKeyalias = null;
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.ToString());
                return false;
            }
            return true;
        }

        private static string escapeDName(string value)
        {
            char[] anyOf = new char[] { '"', ',' };
            for (int i = value.IndexOfAny(anyOf, 0); i >= 0; i = value.IndexOfAny(anyOf, i))
            {
                value = value.Insert(i, @"\");
                i += 2;
            }
            return value;
        }

        public static string[] GetAvailableKeys(string keystore, string storepass)
        {
            if ((keystore.Length == 0) || (storepass.Length == 0))
            {
                return (string[]) (s_AvailableKeyalias = null);
            }
            if ((s_AvailableKeyalias != null) && keystore.Equals(s_CurrentKeystore))
            {
                return s_AvailableKeyalias;
            }
            s_CurrentKeystore = keystore;
            try
            {
                if (AndroidSDKTools.GetInstance() == null)
                {
                    throw new UnityException("Unable to find Android SDK!");
                }
                return (s_AvailableKeyalias = AndroidSDKTools.GetInstanceOrThrowException().ReadAvailableKeys(keystore, storepass));
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.ToString());
                return null;
            }
        }

        private Rect GetRect()
        {
            float num = 16f;
            float minWidth = (80f + (EditorGUIUtility.fieldWidth * 0.5f)) + 5f;
            float maxWidth = (80f + (EditorGUIUtility.fieldWidth * 0.5f)) + 5f;
            return GUILayoutUtility.GetRect(minWidth, maxWidth, num + 5f, num + 5f, EditorStyles.layerMaskField, null);
        }

        private void IntInput(string label, ref int value)
        {
            int num = EditorGUI.IntField(this.GetRect(), EditorGUIUtility.TextContent(label), value);
            if (GUI.changed)
            {
                value = num;
            }
        }

        public void OnGUI()
        {
            GUILayout.Label(EditorGUIUtility.TextContent("Key Creation"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            bool flag = false;
            string[] availableKeys = null;
            if (File.Exists(this.m_Keystore))
            {
                availableKeys = GetAvailableKeys(this.m_Keystore, this.m_StorePass);
            }
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 150f;
            EditorGUILayout.Space();
            this.TextInput("Alias", ref this.m_Alias);
            this.PasswordInput("Password", ref this.m_Password);
            this.PasswordInput("Confirm", ref this.m_PassConfirm);
            this.IntInput("Validity (years)", ref this.m_Validity);
            this.m_Alias = this.m_Alias.ToLower();
            EditorGUILayout.Space();
            this.TextInput("First and Last Name", ref this.m_Name);
            this.TextInput("Organizational Unit", ref this.m_OrgUnit);
            this.TextInput("Organization", ref this.m_Organization);
            this.TextInput("City or Locality", ref this.m_City);
            this.TextInput("State or Province", ref this.m_State);
            this.TextInput("Country Code (XX)", ref this.m_Country);
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            bool flag2 = this.m_Alias == null;
            bool flag3 = false;
            if (availableKeys != null)
            {
                foreach (string str in availableKeys)
                {
                    if (str.Equals(this.m_Alias))
                    {
                        flag3 = true;
                    }
                }
            }
            bool flag4 = (((!flag2 && (this.m_Name.Length == 0)) && ((this.m_OrgUnit.Length == 0) && (this.m_Organization.Length == 0))) && ((this.m_City.Length == 0) && (this.m_State.Length == 0))) && (this.m_Country.Length == 0);
            GUIContent content = null;
            if (flag2 || (this.m_Alias.Length == 0))
            {
                content = EditorGUIUtility.TextContent("Enter alias for your new key.");
            }
            else if (flag3)
            {
                content = EditorGUIUtility.TextContent("The alias entered already exists in keystore.");
            }
            else if (this.m_Password.Length == 0)
            {
                content = EditorGUIUtility.TextContent("Enter key password.");
            }
            else if (this.m_Password.Length < 6)
            {
                content = EditorGUIUtility.TextContent("Password must be at least 6 characters.");
            }
            else if (!this.m_Password.Equals(this.m_PassConfirm))
            {
                content = EditorGUIUtility.TextContent("Passwords don't match.");
            }
            else if (this.m_Validity == 0)
            {
                content = EditorGUIUtility.TextContent("Enter key validity time.");
            }
            else if (this.m_Validity > 0x3e8)
            {
                content = EditorGUIUtility.TextContent("Maximum validity time is 1000 years.");
            }
            else if (flag4)
            {
                content = EditorGUIUtility.TextContent("At least one Certificate issuer field is required.");
            }
            else
            {
                flag = true;
                if (this.m_Validity < 0x19)
                {
                    content = EditorGUIUtility.TextContent("A 25 years validity time is recommended.");
                }
                else
                {
                    content = EditorGUIUtility.TempContent(" ");
                }
            }
            GUILayout.Label(content, EditorStyles.miniLabel, new GUILayoutOption[0]);
            GUILayout.Space(5f);
            GUI.enabled = flag;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(140f) };
            if (GUILayout.Button(EditorGUIUtility.TextContent("Create Key"), options) && this.CreateKey(this.m_Keystore, this.m_StorePass))
            {
                base.Close();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUIUtility.labelWidth = labelWidth;
        }

        private void PasswordInput(string label, ref string value)
        {
            if (value == null)
            {
                value = new string(' ', 0);
            }
            string str = EditorGUI.PasswordField(this.GetRect(), EditorGUIUtility.TextContent(label), value);
            if (GUI.changed)
            {
                value = str;
            }
        }

        public static void ShowAndroidKeystoreWindow(string company, string keystore, string storepass)
        {
            if (File.Exists(keystore))
            {
                try
                {
                    AndroidSDKTools.GetInstanceOrThrowException().ReadAvailableKeys(keystore, storepass);
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception.ToString());
                    return;
                }
            }
            Rect rect = new Rect(100f, 100f, 500f, 330f);
            AndroidKeystoreWindow window = (AndroidKeystoreWindow) EditorWindow.GetWindowWithRect(typeof(AndroidKeystoreWindow), rect, true, EditorGUIUtility.TextContent("Create a new key").text);
            window.position = rect;
            window.m_Parent.window.m_DontSaveToLayout = true;
            window.m_Organization = company;
            window.m_Keystore = keystore;
            window.m_StorePass = storepass;
        }

        private void TextInput(string label, ref string value)
        {
            if (value == null)
            {
                value = new string(' ', 0);
            }
            string str = EditorGUI.TextField(this.GetRect(), EditorGUIUtility.TextContent(label), value);
            if (GUI.changed)
            {
                value = str;
            }
        }
    }
}

