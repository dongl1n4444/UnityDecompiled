namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [Extension]
    internal static class EditorExtensionMethods
    {
        [Extension]
        internal static List<Enum> EnumGetNonObsoleteValues(Type type)
        {
            string[] names = Enum.GetNames(type);
            Enum[] enumArray = Enumerable.ToArray<Enum>(Enumerable.Cast<Enum>(Enum.GetValues(type)));
            List<Enum> list = new List<Enum>();
            for (int i = 0; i < names.Length; i++)
            {
                object[] customAttributes = type.GetMember(names[i])[0].GetCustomAttributes(typeof(ObsoleteAttribute), false);
                bool flag = false;
                foreach (object obj2 in customAttributes)
                {
                    if (obj2 is ObsoleteAttribute)
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    list.Add(enumArray[i]);
                }
            }
            return list;
        }

        [Extension]
        internal static Type GetArrayOrListElementType(Type listType)
        {
            if (listType.IsArray)
            {
                return listType.GetElementType();
            }
            if (listType.IsGenericType && (listType.GetGenericTypeDefinition() == typeof(List<>)))
            {
                return listType.GetGenericArguments()[0];
            }
            return null;
        }

        [Extension]
        internal static bool IsArrayOrList(Type listType)
        {
            return (listType.IsArray || (listType.IsGenericType && (listType.GetGenericTypeDefinition() == typeof(List<>))));
        }

        [Extension]
        internal static bool MainActionKeyForControl(Event evt, int controlId)
        {
            if (GUIUtility.keyboardControl != controlId)
            {
                return false;
            }
            bool flag2 = ((evt.alt || evt.shift) || evt.command) || evt.control;
            if (((evt.type == EventType.KeyDown) && (evt.character == ' ')) && !flag2)
            {
                evt.Use();
                return false;
            }
            return (((evt.type == EventType.KeyDown) && (((evt.keyCode == KeyCode.Space) || (evt.keyCode == KeyCode.Return)) || (evt.keyCode == KeyCode.KeypadEnter))) && !flag2);
        }
    }
}

