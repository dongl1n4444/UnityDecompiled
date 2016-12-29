namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using UnityEngine;

    internal class MaterialPropertyHandler
    {
        private List<MaterialPropertyDrawer> m_DecoratorDrawers;
        private MaterialPropertyDrawer m_PropertyDrawer;
        private static Dictionary<string, MaterialPropertyHandler> s_PropertyHandlers = new Dictionary<string, MaterialPropertyHandler>();

        private static MaterialPropertyDrawer CreatePropertyDrawer(Type klass, string argsText)
        {
            if (string.IsNullOrEmpty(argsText))
            {
                return (Activator.CreateInstance(klass) as MaterialPropertyDrawer);
            }
            char[] separator = new char[] { ',' };
            string[] strArray = argsText.Split(separator);
            object[] args = new object[strArray.Length];
            for (int i = 0; i < strArray.Length; i++)
            {
                float num2;
                string s = strArray[i].Trim();
                if (float.TryParse(s, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num2))
                {
                    args[i] = num2;
                }
                else
                {
                    args[i] = s;
                }
            }
            return (Activator.CreateInstance(klass, args) as MaterialPropertyDrawer);
        }

        internal static MaterialPropertyHandler GetHandler(Shader shader, string name)
        {
            MaterialPropertyHandler shaderPropertyHandler;
            if (shader == null)
            {
                return null;
            }
            string propertyString = GetPropertyString(shader, name);
            if (!s_PropertyHandlers.TryGetValue(propertyString, out shaderPropertyHandler))
            {
                shaderPropertyHandler = GetShaderPropertyHandler(shader, name);
                if ((shaderPropertyHandler != null) && shaderPropertyHandler.IsEmpty())
                {
                    shaderPropertyHandler = null;
                }
                s_PropertyHandlers[propertyString] = shaderPropertyHandler;
            }
            return shaderPropertyHandler;
        }

        public float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            float num = 0f;
            if (this.m_DecoratorDrawers != null)
            {
                foreach (MaterialPropertyDrawer drawer in this.m_DecoratorDrawers)
                {
                    num += drawer.GetPropertyHeight(prop, label, editor);
                }
            }
            if (this.m_PropertyDrawer != null)
            {
                num += this.m_PropertyDrawer.GetPropertyHeight(prop, label, editor);
            }
            return num;
        }

        private static string GetPropertyString(Shader shader, string name)
        {
            if (shader == null)
            {
                return string.Empty;
            }
            return (shader.GetInstanceID() + "_" + name);
        }

        private static MaterialPropertyDrawer GetShaderPropertyDrawer(string attrib, out bool isDecorator)
        {
            isDecorator = false;
            string str = attrib;
            string argsText = string.Empty;
            Match match = Regex.Match(attrib, @"(\w+)\s*\((.*)\)");
            if (match.Success)
            {
                str = match.Groups[1].Value;
                argsText = match.Groups[2].Value.Trim();
            }
            foreach (Type type in EditorAssemblies.SubclassesOf(typeof(MaterialPropertyDrawer)))
            {
                if (((type.Name == str) || (type.Name == (str + "Drawer"))) || (((type.Name == ("Material" + str + "Drawer")) || (type.Name == (str + "Decorator"))) || (type.Name == ("Material" + str + "Decorator"))))
                {
                    try
                    {
                        isDecorator = type.Name.EndsWith("Decorator");
                        return CreatePropertyDrawer(type, argsText);
                    }
                    catch (Exception)
                    {
                        object[] args = new object[] { str, argsText };
                        Debug.LogWarningFormat("Failed to create material drawer {0} with arguments '{1}'", args);
                        return null;
                    }
                }
            }
            return null;
        }

        private static MaterialPropertyHandler GetShaderPropertyHandler(Shader shader, string name)
        {
            string[] shaderPropertyAttributes = ShaderUtil.GetShaderPropertyAttributes(shader, name);
            if ((shaderPropertyAttributes == null) || (shaderPropertyAttributes.Length == 0))
            {
                return null;
            }
            MaterialPropertyHandler handler2 = new MaterialPropertyHandler();
            foreach (string str in shaderPropertyAttributes)
            {
                bool flag;
                MaterialPropertyDrawer shaderPropertyDrawer = GetShaderPropertyDrawer(str, out flag);
                if (shaderPropertyDrawer != null)
                {
                    if (flag)
                    {
                        if (handler2.m_DecoratorDrawers == null)
                        {
                            handler2.m_DecoratorDrawers = new List<MaterialPropertyDrawer>();
                        }
                        handler2.m_DecoratorDrawers.Add(shaderPropertyDrawer);
                    }
                    else
                    {
                        if (handler2.m_PropertyDrawer != null)
                        {
                            Debug.LogWarning($"Shader property {name} already has a property drawer", shader);
                        }
                        handler2.m_PropertyDrawer = shaderPropertyDrawer;
                    }
                }
            }
            return handler2;
        }

        internal static void InvalidatePropertyCache(Shader shader)
        {
            if (shader != null)
            {
                string str = shader.GetInstanceID() + "_";
                List<string> list = new List<string>();
                foreach (string str2 in s_PropertyHandlers.Keys)
                {
                    if (str2.StartsWith(str))
                    {
                        list.Add(str2);
                    }
                }
                foreach (string str3 in list)
                {
                    s_PropertyHandlers.Remove(str3);
                }
            }
        }

        public bool IsEmpty() => 
            ((this.m_PropertyDrawer == null) && ((this.m_DecoratorDrawers == null) || (this.m_DecoratorDrawers.Count == 0)));

        public void OnGUI(ref Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            float labelWidth;
            float fieldWidth;
            float height = position.height;
            position.height = 0f;
            if (this.m_DecoratorDrawers != null)
            {
                foreach (MaterialPropertyDrawer drawer in this.m_DecoratorDrawers)
                {
                    position.height = drawer.GetPropertyHeight(prop, label.text, editor);
                    labelWidth = EditorGUIUtility.labelWidth;
                    fieldWidth = EditorGUIUtility.fieldWidth;
                    drawer.OnGUI(position, prop, label, editor);
                    EditorGUIUtility.labelWidth = labelWidth;
                    EditorGUIUtility.fieldWidth = fieldWidth;
                    position.y += position.height;
                    height -= position.height;
                }
            }
            position.height = height;
            if (this.m_PropertyDrawer != null)
            {
                labelWidth = EditorGUIUtility.labelWidth;
                fieldWidth = EditorGUIUtility.fieldWidth;
                this.m_PropertyDrawer.OnGUI(position, prop, label, editor);
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUIUtility.fieldWidth = fieldWidth;
            }
        }

        public MaterialPropertyDrawer propertyDrawer =>
            this.m_PropertyDrawer;
    }
}

