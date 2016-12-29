namespace UnityEditor.Graphs
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    public class TypeSelector
    {
        [SerializeField]
        private bool m_EditingOther;
        [SerializeField]
        private bool m_OnlyComponents;
        [SerializeField]
        private string m_OtherTypeName;
        [SerializeField]
        private string m_ShownError;
        [SerializeField]
        private string[] m_TypeNames;
        private static Texture2D m_warningIcon;
        private static string[] s_ComponentTypeNames = new string[] { "Transform (UnityEngine.Transform)", "Rigidbody (UnityEngine.Rigidbody)", "Camera (UnityEngine.Camera)", "Light (UnityEngine.Light)", "Animation (UnityEngine.Animation)", "ConstantForce (UnityEngine.ConstantForce)", "Renderer (UnityEngine.Renderer)", "AudioSource (UnityEngine.AudioSource)", "GUIText (UnityEngine.GUIText)", "NetworkView (UnityEngine.NetworkView)", "GUITexture (UnityEngine.GUITexture)", "Collider (UnityEngine.Collider)", "HingeJoint (UnityEngine.HingeJoint)", "ParticleEmitter (UnityEngine.ParticleEmitter)", "", "Other..." };
        private static string[] s_TypeKindNames = new string[] { "Simple", "List", "Array" };
        private static string[] s_TypeNames = new string[] { 
            "string (System.String)", "integer (System.Int32)", "float (System.Single)", "bool (System.Boolean)", "byte (System.Byte)", "char (System.Char)", "GameObject (UnityEngine.GameObject)", "Component (UnityEngine.Component)", "Material (UnityEngine.Material)", "Vector2 (UnityEngine.Vector2)", "Vector3 (UnityEngine.Vector3)", "Vector4 (UnityEngine.Vector4)", "Color (UnityEngine.Color)", "Quaternion (UnityEngine.Quaternion)", "Rectangle (UnityEngine.Rect)", "AnimationCurve (UnityEngine.AnimationCurve)",
            "", "Other..."
        };
        public System.Type selectedType;
        public TypeKind selectedTypeKind;

        public TypeSelector()
        {
            this.selectedType = typeof(DummyNullType);
            this.selectedTypeKind = TypeKind.Simple;
            this.Init(GenericTypeSelectorCommonTypes(), false);
        }

        public TypeSelector(string[] types)
        {
            this.selectedType = typeof(DummyNullType);
            this.selectedTypeKind = TypeKind.Simple;
            this.Init(types, false);
        }

        public TypeSelector(bool onlyComponents)
        {
            this.selectedType = typeof(DummyNullType);
            this.selectedTypeKind = TypeKind.Simple;
            this.Init(!onlyComponents ? GenericTypeSelectorCommonTypes() : s_ComponentTypeNames, onlyComponents);
        }

        public bool DoGUI() => 
            (((this.selectedType != typeof(DummyNullType)) && (this.selectedType != null)) ? this.DoTypeClear() : this.DoTypeSelector());

        private bool DoOtherEditing()
        {
            bool flag = false;
            this.m_OtherTypeName = EditorGUILayout.TextField("Full type Name", this.m_OtherTypeName, new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Set", EditorStyles.miniButton, new GUILayoutOption[0]))
            {
                try
                {
                    this.selectedType = FindType(this.m_OtherTypeName);
                    if (!this.m_OnlyComponents || typeof(Component).IsAssignableFrom(this.selectedType))
                    {
                        this.m_OtherTypeName = string.Empty;
                        this.m_EditingOther = false;
                        this.m_ShownError = string.Empty;
                        flag = true;
                    }
                    this.m_ShownError = "Type must be derived from 'Component'.";
                }
                catch
                {
                    this.m_ShownError = "Could not find a type '" + this.m_OtherTypeName + "'";
                }
            }
            if (GUILayout.Button("Cancel", EditorStyles.miniButton, new GUILayoutOption[0]))
            {
                this.m_OtherTypeName = string.Empty;
                this.m_EditingOther = false;
                this.m_ShownError = string.Empty;
            }
            GUILayout.EndHorizontal();
            if (!string.IsNullOrEmpty(this.m_ShownError))
            {
                ShowError(this.m_ShownError);
            }
            return flag;
        }

        public static string DotNetTypeNiceName(System.Type t)
        {
            if (t == null)
            {
                return string.Empty;
            }
            foreach (string str2 in s_TypeNames)
            {
                if (str2.IndexOf(t.FullName) != -1)
                {
                    return str2;
                }
            }
            return t.FullName;
        }

        private bool DoTypeClear()
        {
            bool flag = false;
            EditorGUILayout.LabelField("Type", DotNetTypeNiceName(this.selectedType), new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Reset", EditorStyles.miniButton, new GUILayoutOption[0]))
            {
                this.selectedType = typeof(DummyNullType);
                flag = true;
            }
            GUILayout.EndHorizontal();
            return flag;
        }

        public bool DoTypeKindGUI()
        {
            bool changed = GUI.changed;
            GUI.changed = false;
            this.selectedTypeKind = (TypeKind) EditorGUILayout.Popup("Kind", (int) this.selectedTypeKind, s_TypeKindNames, new GUILayoutOption[0]);
            bool flag2 = GUI.changed;
            GUI.enabled |= changed;
            return flag2;
        }

        private bool DoTypeSelector()
        {
            if (this.m_EditingOther)
            {
                return this.DoOtherEditing();
            }
            int index = EditorGUILayout.Popup("Choose a type", -1, this.m_TypeNames, new GUILayoutOption[0]);
            if (index != -1)
            {
                string typeName = this.m_TypeNames[index];
                if (typeName == "Other...")
                {
                    this.m_EditingOther = true;
                    this.m_OtherTypeName = string.Empty;
                    return false;
                }
                int num2 = typeName.IndexOf('(');
                if (num2 != -1)
                {
                    typeName = typeName.Substring(num2 + 1, (typeName.IndexOf(')') - num2) - 1);
                }
                this.selectedType = FindType(typeName);
                return true;
            }
            return false;
        }

        private static System.Type FindType(string typeName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                System.Type type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }
            throw new ArgumentException("Type '" + typeName + "' was not found.");
        }

        private static string[] GenericTypeSelectorCommonTypes() => 
            s_TypeNames;

        public static System.Type GetBaseType(TypeKind typeKind, System.Type finalType)
        {
            if (finalType == null)
            {
                return null;
            }
            if (typeKind != TypeKind.Simple)
            {
                if (typeKind != TypeKind.Array)
                {
                    if (typeKind != TypeKind.List)
                    {
                        throw new ArgumentException("Internal error: got weird type kind");
                    }
                    return finalType.GetGenericArguments()[0];
                }
            }
            else
            {
                return finalType;
            }
            return finalType.GetElementType();
        }

        public static System.Type GetFinalType(TypeKind typeKind, System.Type baseType)
        {
            if (typeKind != TypeKind.Simple)
            {
                if (typeKind != TypeKind.Array)
                {
                    if (typeKind != TypeKind.List)
                    {
                        throw new ArgumentException("Internal error: got weird type kind");
                    }
                    System.Type[] typeArguments = new System.Type[] { baseType };
                    return typeof(List<>).MakeGenericType(typeArguments);
                }
            }
            else
            {
                return baseType;
            }
            return baseType.MakeArrayType();
        }

        public static TypeKind GetTypeKind(System.Type dataType)
        {
            if (dataType != null)
            {
                if (dataType.IsArray)
                {
                    return TypeKind.Array;
                }
                if (dataType.IsGenericType)
                {
                    return TypeKind.List;
                }
            }
            return TypeKind.Simple;
        }

        private void Init(string[] types, bool onlyComponents)
        {
            this.m_TypeNames = types;
            this.m_OnlyComponents = onlyComponents;
        }

        private static void ShowError(string shownError)
        {
            GUIContent content = new GUIContent(shownError) {
                image = warningIcon
            };
            GUILayout.Space(5f);
            GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
            GUILayout.Label(content, EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
            GUILayout.EndVertical();
        }

        private static Texture2D warningIcon
        {
            get
            {
                Texture2D warningIcon;
                if (m_warningIcon != null)
                {
                    warningIcon = m_warningIcon;
                }
                else
                {
                    warningIcon = m_warningIcon = EditorGUIUtility.LoadIcon("console.warnicon");
                }
                return warningIcon;
            }
        }

        public enum TypeKind
        {
            Simple,
            List,
            Array
        }
    }
}

