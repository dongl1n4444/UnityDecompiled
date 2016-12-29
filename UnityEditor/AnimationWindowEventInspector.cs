namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [CustomEditor(typeof(AnimationWindowEvent))]
    internal class AnimationWindowEventInspector : Editor
    {
        [CompilerGenerated]
        private static Func<ParameterInfo, Type> <>f__am$cache0;
        private const string kNoneSelected = "(No Function Selected)";
        private const string kNotSupportedPostFix = " (Function Not Supported)";

        public static List<AnimationWindowEventMethod> CollectSupportedMethods(AnimationWindowEvent awevt)
        {
            List<AnimationWindowEventMethod> list = new List<AnimationWindowEventMethod>();
            if (awevt.root != null)
            {
                MonoBehaviour[] components = awevt.root.GetComponents<MonoBehaviour>();
                HashSet<string> set = new HashSet<string>();
                foreach (MonoBehaviour behaviour in components)
                {
                    if (behaviour != null)
                    {
                        for (Type type = behaviour.GetType(); (type != typeof(MonoBehaviour)) && (type != null); type = type.BaseType)
                        {
                            MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                            for (int i = 0; i < methods.Length; i++)
                            {
                                <CollectSupportedMethods>c__AnonStorey1 storey = new <CollectSupportedMethods>c__AnonStorey1();
                                MethodInfo info = methods[i];
                                storey.name = info.Name;
                                if (IsSupportedMethodName(storey.name))
                                {
                                    ParameterInfo[] parameters = info.GetParameters();
                                    if (parameters.Length <= 1)
                                    {
                                        Type parameterType = null;
                                        if (parameters.Length == 1)
                                        {
                                            parameterType = parameters[0].ParameterType;
                                            if ((((parameterType != typeof(string)) && (parameterType != typeof(float))) && ((parameterType != typeof(int)) && (parameterType != typeof(AnimationEvent)))) && (((parameterType != typeof(Object)) && !parameterType.IsSubclassOf(typeof(Object))) && !parameterType.IsEnum))
                                            {
                                                continue;
                                            }
                                        }
                                        AnimationWindowEventMethod item = new AnimationWindowEventMethod {
                                            name = info.Name,
                                            parameterType = parameterType
                                        };
                                        int num3 = list.FindIndex(new Predicate<AnimationWindowEventMethod>(storey.<>m__0));
                                        if (num3 != -1)
                                        {
                                            AnimationWindowEventMethod method2 = list[num3];
                                            if (method2.parameterType != parameterType)
                                            {
                                                set.Add(storey.name);
                                            }
                                        }
                                        list.Add(item);
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (string str in set)
                {
                    for (int j = list.Count - 1; j >= 0; j--)
                    {
                        AnimationWindowEventMethod method3 = list[j];
                        if (method3.name.Equals(str))
                        {
                            list.RemoveAt(j);
                        }
                    }
                }
            }
            return list;
        }

        private static void DoEditRegularParameters(AnimationEvent evt, Type selectedParameter)
        {
            if ((selectedParameter == typeof(AnimationEvent)) || (selectedParameter == typeof(float)))
            {
                evt.floatParameter = EditorGUILayout.FloatField("Float", evt.floatParameter, new GUILayoutOption[0]);
            }
            if (selectedParameter.IsEnum)
            {
                evt.intParameter = EnumPopup("Enum", selectedParameter, evt.intParameter);
            }
            else if ((selectedParameter == typeof(AnimationEvent)) || (selectedParameter == typeof(int)))
            {
                evt.intParameter = EditorGUILayout.IntField("Int", evt.intParameter, new GUILayoutOption[0]);
            }
            if ((selectedParameter == typeof(AnimationEvent)) || (selectedParameter == typeof(string)))
            {
                evt.stringParameter = EditorGUILayout.TextField("String", evt.stringParameter, new GUILayoutOption[0]);
            }
            if (((selectedParameter == typeof(AnimationEvent)) || selectedParameter.IsSubclassOf(typeof(Object))) || (selectedParameter == typeof(Object)))
            {
                Type objType = typeof(Object);
                if (selectedParameter != typeof(AnimationEvent))
                {
                    objType = selectedParameter;
                }
                bool allowSceneObjects = false;
                evt.objectReferenceParameter = EditorGUILayout.ObjectField(ObjectNames.NicifyVariableName(objType.Name), evt.objectReferenceParameter, objType, allowSceneObjects, new GUILayoutOption[0]);
            }
        }

        private static int EnumPopup(string label, Type enumType, int selected)
        {
            if (!enumType.IsEnum)
            {
                throw new Exception("parameter _enum must be of type System.Enum");
            }
            string[] names = Enum.GetNames(enumType);
            int index = Array.IndexOf<string>(names, Enum.GetName(enumType, selected));
            index = EditorGUILayout.Popup(label, index, names, EditorStyles.popup, new GUILayoutOption[0]);
            if (index == -1)
            {
                return selected;
            }
            Enum enum2 = (Enum) Enum.Parse(enumType, names[index]);
            return Convert.ToInt32(enum2);
        }

        public static string FormatEvent(GameObject root, AnimationEvent evt)
        {
            if (string.IsNullOrEmpty(evt.functionName))
            {
                return "(No Function Selected)";
            }
            if (IsSupportedMethodName(evt.functionName))
            {
                if (root == null)
                {
                    return (evt.functionName + " (Function Not Supported)");
                }
                foreach (MonoBehaviour behaviour in root.GetComponents<MonoBehaviour>())
                {
                    if (behaviour != null)
                    {
                        Type type = behaviour.GetType();
                        if ((type != typeof(MonoBehaviour)) && ((type.BaseType == null) || (type.BaseType.Name != "GraphBehaviour")))
                        {
                            MethodInfo method = null;
                            try
                            {
                                method = type.GetMethod(evt.functionName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                            }
                            catch (AmbiguousMatchException)
                            {
                            }
                            if (method != null)
                            {
                                if (<>f__am$cache0 == null)
                                {
                                    <>f__am$cache0 = new Func<ParameterInfo, Type>(null, (IntPtr) <FormatEvent>m__0);
                                }
                                IEnumerable<Type> paramTypes = Enumerable.Select<ParameterInfo, Type>(method.GetParameters(), <>f__am$cache0);
                                return (evt.functionName + FormatEventArguments(paramTypes, evt));
                            }
                        }
                    }
                }
            }
            return (evt.functionName + " (Function Not Supported)");
        }

        private static string FormatEventArguments(IEnumerable<Type> paramTypes, AnimationEvent evt)
        {
            if (!paramTypes.Any<Type>())
            {
                return " ( )";
            }
            if (paramTypes.Count<Type>() <= 1)
            {
                Type enumType = paramTypes.First<Type>();
                if (enumType == typeof(string))
                {
                    return (" ( \"" + evt.stringParameter + "\" )");
                }
                if (enumType == typeof(float))
                {
                    return (" ( " + evt.floatParameter + " )");
                }
                if (enumType == typeof(int))
                {
                    return (" ( " + evt.intParameter + " )");
                }
                if (enumType.IsEnum)
                {
                    string[] textArray1 = new string[] { " ( ", enumType.Name, ".", Enum.GetName(enumType, evt.intParameter), " )" };
                    return string.Concat(textArray1);
                }
                if (enumType == typeof(AnimationEvent))
                {
                    object[] objArray1 = new object[] { " ( ", evt.floatParameter, " / ", evt.intParameter, " / \"", evt.stringParameter, "\" / ", (evt.objectReferenceParameter != null) ? evt.objectReferenceParameter.name : "null", " )" };
                    return string.Concat(objArray1);
                }
                if (enumType.IsSubclassOf(typeof(Object)) || (enumType == typeof(Object)))
                {
                    return (" ( " + ((evt.objectReferenceParameter != null) ? evt.objectReferenceParameter.name : "null") + " )");
                }
            }
            return " (Function Not Supported)";
        }

        private static bool IsSupportedMethodName(string name)
        {
            if (((name == "Main") || (name == "Start")) || ((name == "Awake") || (name == "Update")))
            {
                return false;
            }
            return true;
        }

        public static void OnDisabledAnimationEvent()
        {
            AnimationEvent evt = new AnimationEvent();
            using (new EditorGUI.DisabledScope(true))
            {
                evt.functionName = EditorGUILayout.TextField(new GUIContent("Function"), evt.functionName, new GUILayoutOption[0]);
                DoEditRegularParameters(evt, typeof(AnimationEvent));
            }
        }

        public static void OnEditAnimationEvent(AnimationWindowEvent awevt)
        {
            <OnEditAnimationEvent>c__AnonStorey0 storey = new <OnEditAnimationEvent>c__AnonStorey0();
            AnimationEvent[] events = null;
            if (awevt.clip != null)
            {
                events = AnimationUtility.GetAnimationEvents(awevt.clip);
            }
            else if (awevt.clipInfo != null)
            {
                events = awevt.clipInfo.GetEvents();
            }
            if (((events != null) && (awevt.eventIndex >= 0)) && (awevt.eventIndex < events.Length))
            {
                storey.evt = events[awevt.eventIndex];
                GUI.changed = false;
                if (awevt.root != null)
                {
                    List<AnimationWindowEventMethod> list = CollectSupportedMethods(awevt);
                    List<string> list2 = new List<string>(list.Count);
                    for (int i = 0; i < list.Count; i++)
                    {
                        AnimationWindowEventMethod method = list[i];
                        string str = " ( )";
                        if (method.parameterType != null)
                        {
                            if (method.parameterType == typeof(float))
                            {
                                str = " ( float )";
                            }
                            else if (method.parameterType == typeof(int))
                            {
                                str = " ( int )";
                            }
                            else
                            {
                                str = $" ( {method.parameterType.Name} )";
                            }
                        }
                        list2.Add(method.name + str);
                    }
                    int count = list.Count;
                    int selectedIndex = list.FindIndex(new Predicate<AnimationWindowEventMethod>(storey.<>m__0));
                    if (selectedIndex == -1)
                    {
                        selectedIndex = list.Count;
                        AnimationWindowEventMethod item = new AnimationWindowEventMethod {
                            name = storey.evt.functionName,
                            parameterType = null
                        };
                        list.Add(item);
                        if (string.IsNullOrEmpty(storey.evt.functionName))
                        {
                            list2.Add("(No Function Selected)");
                        }
                        else
                        {
                            list2.Add(storey.evt.functionName + " (Function Not Supported)");
                        }
                    }
                    EditorGUIUtility.labelWidth = 130f;
                    int num4 = selectedIndex;
                    selectedIndex = EditorGUILayout.Popup("Function: ", selectedIndex, list2.ToArray(), new GUILayoutOption[0]);
                    if (((num4 != selectedIndex) && (selectedIndex != -1)) && (selectedIndex != count))
                    {
                        AnimationWindowEventMethod method3 = list[selectedIndex];
                        storey.evt.functionName = method3.name;
                        storey.evt.stringParameter = string.Empty;
                    }
                    AnimationWindowEventMethod method4 = list[selectedIndex];
                    Type parameterType = method4.parameterType;
                    if (parameterType != null)
                    {
                        EditorGUILayout.Space();
                        if (parameterType == typeof(AnimationEvent))
                        {
                            EditorGUILayout.PrefixLabel("Event Data");
                        }
                        else
                        {
                            EditorGUILayout.PrefixLabel("Parameters");
                        }
                        DoEditRegularParameters(storey.evt, parameterType);
                    }
                }
                else
                {
                    storey.evt.functionName = EditorGUILayout.TextField(new GUIContent("Function"), storey.evt.functionName, new GUILayoutOption[0]);
                    DoEditRegularParameters(storey.evt, typeof(AnimationEvent));
                }
                if (GUI.changed)
                {
                    if (awevt.clip != null)
                    {
                        Undo.RegisterCompleteObjectUndo(awevt.clip, "Animation Event Change");
                        AnimationUtility.SetAnimationEvents(awevt.clip, events);
                    }
                    else if (awevt.clipInfo != null)
                    {
                        awevt.clipInfo.SetEvent(awevt.eventIndex, storey.evt);
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            OnEditAnimationEvent(base.target as AnimationWindowEvent);
        }

        [CompilerGenerated]
        private sealed class <CollectSupportedMethods>c__AnonStorey1
        {
            internal string name;

            internal bool <>m__0(AnimationWindowEventMethod m) => 
                (m.name == this.name);
        }

        [CompilerGenerated]
        private sealed class <OnEditAnimationEvent>c__AnonStorey0
        {
            internal AnimationEvent evt;

            internal bool <>m__0(AnimationWindowEventMethod method) => 
                (method.name == this.evt.functionName);
        }
    }
}

