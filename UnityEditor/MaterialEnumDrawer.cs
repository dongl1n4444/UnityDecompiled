namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class MaterialEnumDrawer : MaterialPropertyDrawer
    {
        [CompilerGenerated]
        private static Func<Assembly, IEnumerable<System.Type>> <>f__am$cache0;
        private readonly GUIContent[] names;
        private readonly int[] values;

        public MaterialEnumDrawer(string enumName)
        {
            <MaterialEnumDrawer>c__AnonStorey0 storey = new <MaterialEnumDrawer>c__AnonStorey0 {
                enumName = enumName
            };
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<Assembly, IEnumerable<System.Type>>(MaterialEnumDrawer.<MaterialEnumDrawer>m__0);
            }
            System.Type[] typeArray = Enumerable.SelectMany<Assembly, System.Type>(AppDomain.CurrentDomain.GetAssemblies(), <>f__am$cache0).ToArray<System.Type>();
            try
            {
                System.Type enumType = Enumerable.FirstOrDefault<System.Type>(typeArray, new Func<System.Type, bool>(storey.<>m__0));
                string[] names = Enum.GetNames(enumType);
                this.names = new GUIContent[names.Length];
                for (int i = 0; i < names.Length; i++)
                {
                    this.names[i] = new GUIContent(names[i]);
                }
                Array values = Enum.GetValues(enumType);
                this.values = new int[values.Length];
                for (int j = 0; j < values.Length; j++)
                {
                    this.values[j] = (int) values.GetValue(j);
                }
            }
            catch (Exception)
            {
                object[] args = new object[] { storey.enumName };
                Debug.LogWarningFormat("Failed to create MaterialEnum, enum {0} not found", args);
                throw;
            }
        }

        public MaterialEnumDrawer(string n1, float v1) : this(textArray1, singleArray1)
        {
            string[] textArray1 = new string[] { n1 };
            float[] singleArray1 = new float[] { v1 };
        }

        public MaterialEnumDrawer(string[] enumNames, float[] vals)
        {
            this.names = new GUIContent[enumNames.Length];
            for (int i = 0; i < enumNames.Length; i++)
            {
                this.names[i] = new GUIContent(enumNames[i]);
            }
            this.values = new int[vals.Length];
            for (int j = 0; j < vals.Length; j++)
            {
                this.values[j] = (int) vals[j];
            }
        }

        public MaterialEnumDrawer(string n1, float v1, string n2, float v2) : this(textArray1, singleArray1)
        {
            string[] textArray1 = new string[] { n1, n2 };
            float[] singleArray1 = new float[] { v1, v2 };
        }

        public MaterialEnumDrawer(string n1, float v1, string n2, float v2, string n3, float v3) : this(textArray1, singleArray1)
        {
            string[] textArray1 = new string[] { n1, n2, n3 };
            float[] singleArray1 = new float[] { v1, v2, v3 };
        }

        public MaterialEnumDrawer(string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4) : this(textArray1, singleArray1)
        {
            string[] textArray1 = new string[] { n1, n2, n3, n4 };
            float[] singleArray1 = new float[] { v1, v2, v3, v4 };
        }

        public MaterialEnumDrawer(string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4, string n5, float v5) : this(textArray1, singleArray1)
        {
            string[] textArray1 = new string[] { n1, n2, n3, n4, n5 };
            float[] singleArray1 = new float[] { v1, v2, v3, v4, v5 };
        }

        public MaterialEnumDrawer(string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4, string n5, float v5, string n6, float v6) : this(textArray1, singleArray1)
        {
            string[] textArray1 = new string[] { n1, n2, n3, n4, n5, n6 };
            float[] singleArray1 = new float[] { v1, v2, v3, v4, v5, v6 };
        }

        public MaterialEnumDrawer(string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4, string n5, float v5, string n6, float v6, string n7, float v7) : this(textArray1, singleArray1)
        {
            string[] textArray1 = new string[] { n1, n2, n3, n4, n5, n6, n7 };
            float[] singleArray1 = new float[] { v1, v2, v3, v4, v5, v6, v7 };
        }

        [CompilerGenerated]
        private static IEnumerable<System.Type> <MaterialEnumDrawer>m__0(Assembly x) => 
            AssemblyHelper.GetTypesFromAssembly(x);

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            if ((prop.type != MaterialProperty.PropType.Float) && (prop.type != MaterialProperty.PropType.Range))
            {
                return 40f;
            }
            return base.GetPropertyHeight(prop, label, editor);
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if ((prop.type != MaterialProperty.PropType.Float) && (prop.type != MaterialProperty.PropType.Range))
            {
                GUIContent content = EditorGUIUtility.TempContent("Enum used on a non-float property: " + prop.name, EditorGUIUtility.GetHelpIcon(MessageType.Warning));
                EditorGUI.LabelField(position, content, EditorStyles.helpBox);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = prop.hasMixedValue;
                int floatValue = (int) prop.floatValue;
                floatValue = EditorGUI.IntPopup(position, label, floatValue, this.names, this.values);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    prop.floatValue = floatValue;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <MaterialEnumDrawer>c__AnonStorey0
        {
            internal string enumName;

            internal bool <>m__0(System.Type x) => 
                (x.IsSubclassOf(typeof(Enum)) && ((x.Name == this.enumName) || (x.FullName == this.enumName)));
        }
    }
}

