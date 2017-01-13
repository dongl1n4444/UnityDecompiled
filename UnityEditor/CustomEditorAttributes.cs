namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    internal class CustomEditorAttributes
    {
        private static readonly Dictionary<Type, Type> kCachedEditorForType = new Dictionary<Type, Type>();
        private static readonly Dictionary<Type, Type> kCachedMultiEditorForType = new Dictionary<Type, Type>();
        private static readonly List<MonoEditorType> kSCustomEditors = new List<MonoEditorType>();
        private static readonly List<MonoEditorType> kSCustomMultiEditors = new List<MonoEditorType>();
        private static bool s_Initialized;

        internal static Type FindCustomEditorType(Object o, bool multiEdit) => 
            FindCustomEditorTypeByType(o.GetType(), multiEdit);

        internal static Type FindCustomEditorTypeByType(Type type, bool multiEdit)
        {
            if (!s_Initialized)
            {
                Assembly[] loadedAssemblies = EditorAssemblies.loadedAssemblies;
                for (int i = loadedAssemblies.Length - 1; i >= 0; i--)
                {
                    Rebuild(loadedAssemblies[i]);
                }
                s_Initialized = true;
            }
            if (type != null)
            {
                Type inspectorType;
                Dictionary<Type, Type> dictionary = !multiEdit ? kCachedEditorForType : kCachedMultiEditorForType;
                if (dictionary.TryGetValue(type, out inspectorType))
                {
                    return inspectorType;
                }
                List<MonoEditorType> list = !multiEdit ? kSCustomEditors : kSCustomMultiEditors;
                for (int j = 0; j < 2; j++)
                {
                    for (Type type4 = type; type4 != null; type4 = type4.BaseType)
                    {
                        for (int k = 0; k < list.Count; k++)
                        {
                            if (IsAppropriateEditor(list[k], type4, !(type == type4), j == 1))
                            {
                                inspectorType = list[k].m_InspectorType;
                                dictionary.Add(type, inspectorType);
                                return inspectorType;
                            }
                        }
                    }
                }
                dictionary.Add(type, null);
            }
            return null;
        }

        private static bool IsAppropriateEditor(MonoEditorType editor, Type parentClass, bool isChildClass, bool isFallback)
        {
            if (isChildClass && !editor.m_EditorForChildClasses)
            {
                return false;
            }
            if (isFallback != editor.m_IsFallback)
            {
                return false;
            }
            return (parentClass == editor.m_InspectedType);
        }

        internal static void Rebuild(Assembly assembly)
        {
            Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
            foreach (Type type in typesFromAssembly)
            {
                object[] customAttributes = type.GetCustomAttributes(typeof(CustomEditor), false);
                foreach (CustomEditor editor in customAttributes)
                {
                    MonoEditorType item = new MonoEditorType();
                    if (editor.m_InspectedType == null)
                    {
                        Debug.Log("Can't load custom inspector " + type.Name + " because the inspected type is null.");
                    }
                    else if (!type.IsSubclassOf(typeof(Editor)))
                    {
                        if (((type.FullName != "TweakMode") || !type.IsEnum) || (editor.m_InspectedType.FullName != "BloomAndFlares"))
                        {
                            Debug.LogWarning(type.Name + " uses the CustomEditor attribute but does not inherit from Editor.\nYou must inherit from Editor. See the Editor class script documentation.");
                        }
                    }
                    else
                    {
                        item.m_InspectedType = editor.m_InspectedType;
                        item.m_InspectorType = type;
                        item.m_EditorForChildClasses = editor.m_EditorForChildClasses;
                        item.m_IsFallback = editor.isFallback;
                        kSCustomEditors.Add(item);
                        if (type.GetCustomAttributes(typeof(CanEditMultipleObjects), false).Length > 0)
                        {
                            kSCustomMultiEditors.Add(item);
                        }
                    }
                }
            }
        }

        private class MonoEditorType
        {
            public bool m_EditorForChildClasses;
            public Type m_InspectedType;
            public Type m_InspectorType;
            public bool m_IsFallback;
        }
    }
}

