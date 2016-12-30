namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class ScriptAttributeUtility
    {
        [CompilerGenerated]
        private static Func<Assembly, IEnumerable<Type>> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<object, PropertyAttribute> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<PropertyAttribute, int> <>f__am$cache2;
        private static Dictionary<string, List<PropertyAttribute>> s_BuiltinAttributes = null;
        private static PropertyHandlerCache s_CurrentCache = null;
        internal static Stack<PropertyDrawer> s_DrawerStack = new Stack<PropertyDrawer>();
        private static Dictionary<Type, DrawerKeySet> s_DrawerTypeForType = null;
        private static PropertyHandlerCache s_GlobalCache = new PropertyHandlerCache();
        private static PropertyHandler s_NextHandler = new PropertyHandler();
        private static PropertyHandler s_SharedNullHandler = new PropertyHandler();

        private static void AddBuiltinAttribute(string componentTypeName, string propertyPath, PropertyAttribute attr)
        {
            string key = componentTypeName + "_" + propertyPath;
            if (!s_BuiltinAttributes.ContainsKey(key))
            {
                s_BuiltinAttributes.Add(key, new List<PropertyAttribute>());
            }
            s_BuiltinAttributes[key].Add(attr);
        }

        private static void BuildDrawerTypeForTypeDictionary()
        {
            s_DrawerTypeForType = new Dictionary<Type, DrawerKeySet>();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = x => AssemblyHelper.GetTypesFromAssembly(x);
            }
            Type[] typeArray = Enumerable.SelectMany<Assembly, Type>(AppDomain.CurrentDomain.GetAssemblies(), <>f__am$cache0).ToArray<Type>();
            foreach (Type type in EditorAssemblies.SubclassesOf(typeof(GUIDrawer)))
            {
                object[] objArray2 = type.GetCustomAttributes(typeof(CustomPropertyDrawer), true);
                for (int i = 0; i < objArray2.Length; i++)
                {
                    <BuildDrawerTypeForTypeDictionary>c__AnonStorey0 storey = new <BuildDrawerTypeForTypeDictionary>c__AnonStorey0 {
                        editor = (CustomPropertyDrawer) objArray2[i]
                    };
                    DrawerKeySet set = new DrawerKeySet {
                        drawer = type,
                        type = storey.editor.m_Type
                    };
                    s_DrawerTypeForType[storey.editor.m_Type] = set;
                    if (storey.editor.m_UseForChildren)
                    {
                        IEnumerable<Type> enumerable = Enumerable.Where<Type>(typeArray, new Func<Type, bool>(storey.<>m__0));
                        foreach (Type type2 in enumerable)
                        {
                            if (s_DrawerTypeForType.ContainsKey(type2))
                            {
                                DrawerKeySet set2 = s_DrawerTypeForType[type2];
                                if (storey.editor.m_Type.IsAssignableFrom(set2.type))
                                {
                                    continue;
                                }
                            }
                            DrawerKeySet set3 = new DrawerKeySet {
                                drawer = type,
                                type = storey.editor.m_Type
                            };
                            s_DrawerTypeForType[type2] = set3;
                        }
                    }
                }
            }
        }

        internal static void ClearGlobalCache()
        {
            s_GlobalCache.Clear();
        }

        private static List<PropertyAttribute> GetBuiltinAttributes(SerializedProperty property)
        {
            if (property.serializedObject.targetObject == null)
            {
                return null;
            }
            Type type = property.serializedObject.targetObject.GetType();
            if (type == null)
            {
                return null;
            }
            string key = type.Name + "_" + property.propertyPath;
            List<PropertyAttribute> list2 = null;
            s_BuiltinAttributes.TryGetValue(key, out list2);
            return list2;
        }

        internal static Type GetDrawerTypeForType(Type type)
        {
            DrawerKeySet set;
            if (s_DrawerTypeForType == null)
            {
                BuildDrawerTypeForTypeDictionary();
            }
            s_DrawerTypeForType.TryGetValue(type, out set);
            if ((set.drawer == null) && type.IsGenericType)
            {
                s_DrawerTypeForType.TryGetValue(type.GetGenericTypeDefinition(), out set);
            }
            return set.drawer;
        }

        private static List<PropertyAttribute> GetFieldAttributes(FieldInfo field)
        {
            if (field != null)
            {
                object[] customAttributes = field.GetCustomAttributes(typeof(PropertyAttribute), true);
                if ((customAttributes != null) && (customAttributes.Length > 0))
                {
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = e => e as PropertyAttribute;
                    }
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = e => -e.order;
                    }
                    return new List<PropertyAttribute>(Enumerable.OrderBy<PropertyAttribute, int>(Enumerable.Select<object, PropertyAttribute>(customAttributes, <>f__am$cache1), <>f__am$cache2));
                }
            }
            return null;
        }

        private static FieldInfo GetFieldInfoFromProperty(SerializedProperty property, out Type type)
        {
            Type scriptTypeFromProperty = GetScriptTypeFromProperty(property);
            if (scriptTypeFromProperty == null)
            {
                type = null;
                return null;
            }
            return GetFieldInfoFromPropertyPath(scriptTypeFromProperty, property.propertyPath, out type);
        }

        private static FieldInfo GetFieldInfoFromPropertyPath(Type host, string path, out Type type)
        {
            FieldInfo info = null;
            type = host;
            char[] separator = new char[] { '.' };
            string[] strArray = path.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                string name = strArray[i];
                if (((i < (strArray.Length - 1)) && (name == "Array")) && strArray[i + 1].StartsWith("data["))
                {
                    if (type.IsArrayOrList())
                    {
                        type = type.GetArrayOrListElementType();
                    }
                    i++;
                }
                else
                {
                    FieldInfo field = null;
                    for (Type type2 = type; (field == null) && (type2 != null); type2 = type2.BaseType)
                    {
                        field = type2.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    }
                    if (field == null)
                    {
                        type = null;
                        return null;
                    }
                    info = field;
                    type = info.FieldType;
                }
            }
            return info;
        }

        internal static PropertyHandler GetHandler(SerializedProperty property)
        {
            if (property == null)
            {
                return s_SharedNullHandler;
            }
            if (property.serializedObject.inspectorMode != InspectorMode.Normal)
            {
                return s_SharedNullHandler;
            }
            PropertyHandler handler = propertyHandlerCache.GetHandler(property);
            if (handler == null)
            {
                Type type = null;
                List<PropertyAttribute> fieldAttributes = null;
                FieldInfo field = null;
                Object targetObject = property.serializedObject.targetObject;
                if ((targetObject is MonoBehaviour) || (targetObject is ScriptableObject))
                {
                    field = GetFieldInfoFromProperty(property, out type);
                    fieldAttributes = GetFieldAttributes(field);
                }
                else
                {
                    if (s_BuiltinAttributes == null)
                    {
                        PopulateBuiltinAttributes();
                    }
                    if (fieldAttributes == null)
                    {
                        fieldAttributes = GetBuiltinAttributes(property);
                    }
                }
                handler = s_NextHandler;
                if (fieldAttributes != null)
                {
                    for (int i = fieldAttributes.Count - 1; i >= 0; i--)
                    {
                        handler.HandleAttribute(fieldAttributes[i], field, type);
                    }
                }
                if (!handler.hasPropertyDrawer && (type != null))
                {
                    handler.HandleDrawnType(type, type, field, null);
                }
                if (handler.empty)
                {
                    propertyHandlerCache.SetHandler(property, s_SharedNullHandler);
                    handler = s_SharedNullHandler;
                }
                else
                {
                    propertyHandlerCache.SetHandler(property, handler);
                    s_NextHandler = new PropertyHandler();
                }
            }
            return handler;
        }

        private static Type GetScriptTypeFromProperty(SerializedProperty property)
        {
            SerializedProperty property2 = property.serializedObject.FindProperty("m_Script");
            if (property2 == null)
            {
                return null;
            }
            MonoScript objectReferenceValue = property2.objectReferenceValue as MonoScript;
            return objectReferenceValue?.GetClass();
        }

        private static void PopulateBuiltinAttributes()
        {
            s_BuiltinAttributes = new Dictionary<string, List<PropertyAttribute>>();
            AddBuiltinAttribute("GUIText", "m_Text", new MultilineAttribute());
            AddBuiltinAttribute("TextMesh", "m_Text", new MultilineAttribute());
        }

        internal static PropertyHandlerCache propertyHandlerCache
        {
            get
            {
                PropertyHandlerCache cache;
                if (s_CurrentCache != null)
                {
                    cache = s_CurrentCache;
                }
                else
                {
                    cache = s_GlobalCache;
                }
                return cache;
            }
            set
            {
                s_CurrentCache = value;
            }
        }

        [CompilerGenerated]
        private sealed class <BuildDrawerTypeForTypeDictionary>c__AnonStorey0
        {
            internal CustomPropertyDrawer editor;

            internal bool <>m__0(Type x) => 
                x.IsSubclassOf(this.editor.m_Type);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DrawerKeySet
        {
            public Type drawer;
            public Type type;
        }
    }
}

