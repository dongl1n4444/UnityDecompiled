namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(AnimationWindowEvent))]
    internal class AnimationWindowEventInspector : Editor
    {
        [CompilerGenerated]
        private static Func<UnityEngine.Object, AnimationWindowEvent> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<System.Reflection.ParameterInfo, System.Type> <>f__am$cache1;
        private const string kNoneSelected = "(No Function Selected)";
        private const string kNotSupportedPostFix = " (Function Not Supported)";

        public static List<AnimationWindowEventMethod> CollectSupportedMethods(GameObject gameObject)
        {
            List<AnimationWindowEventMethod> list = new List<AnimationWindowEventMethod>();
            if (gameObject != null)
            {
                MonoBehaviour[] components = gameObject.GetComponents<MonoBehaviour>();
                HashSet<string> set = new HashSet<string>();
                foreach (MonoBehaviour behaviour in components)
                {
                    if (behaviour != null)
                    {
                        for (System.Type type = behaviour.GetType(); (type != typeof(MonoBehaviour)) && (type != null); type = type.BaseType)
                        {
                            MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                            for (int i = 0; i < methods.Length; i++)
                            {
                                <CollectSupportedMethods>c__AnonStorey1 storey = new <CollectSupportedMethods>c__AnonStorey1();
                                MethodInfo info = methods[i];
                                storey.name = info.Name;
                                if (IsSupportedMethodName(storey.name))
                                {
                                    System.Reflection.ParameterInfo[] parameters = info.GetParameters();
                                    if (parameters.Length <= 1)
                                    {
                                        System.Type parameterType = null;
                                        if (parameters.Length == 1)
                                        {
                                            parameterType = parameters[0].ParameterType;
                                            if ((((parameterType != typeof(string)) && (parameterType != typeof(float))) && ((parameterType != typeof(int)) && (parameterType != typeof(AnimationEvent)))) && (((parameterType != typeof(UnityEngine.Object)) && !parameterType.IsSubclassOf(typeof(UnityEngine.Object))) && !parameterType.IsEnum))
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

        private static void DoEditRegularParameters(AnimationEvent[] events, System.Type selectedParameter)
        {
            <DoEditRegularParameters>c__AnonStorey2 storey = new <DoEditRegularParameters>c__AnonStorey2 {
                firstEvent = events[0]
            };
            if ((selectedParameter == typeof(AnimationEvent)) || (selectedParameter == typeof(float)))
            {
                bool flag = Array.TrueForAll<AnimationEvent>(events, new Predicate<AnimationEvent>(storey.<>m__0));
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = !flag;
                float num = EditorGUILayout.FloatField("Float", storey.firstEvent.floatParameter, new GUILayoutOption[0]);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (AnimationEvent event2 in events)
                    {
                        event2.floatParameter = num;
                    }
                }
            }
            if (((selectedParameter == typeof(AnimationEvent)) || (selectedParameter == typeof(int))) || selectedParameter.IsEnum)
            {
                bool flag2 = Array.TrueForAll<AnimationEvent>(events, new Predicate<AnimationEvent>(storey.<>m__1));
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = !flag2;
                int num3 = 0;
                if (selectedParameter.IsEnum)
                {
                    num3 = EnumPopup("Enum", selectedParameter, storey.firstEvent.intParameter);
                }
                else
                {
                    num3 = EditorGUILayout.IntField("Int", storey.firstEvent.intParameter, new GUILayoutOption[0]);
                }
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (AnimationEvent event3 in events)
                    {
                        event3.intParameter = num3;
                    }
                }
            }
            if ((selectedParameter == typeof(AnimationEvent)) || (selectedParameter == typeof(string)))
            {
                bool flag3 = Array.TrueForAll<AnimationEvent>(events, new Predicate<AnimationEvent>(storey.<>m__2));
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = !flag3;
                string str = EditorGUILayout.TextField("String", storey.firstEvent.stringParameter, new GUILayoutOption[0]);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (AnimationEvent event4 in events)
                    {
                        event4.stringParameter = str;
                    }
                }
            }
            if (((selectedParameter == typeof(AnimationEvent)) || selectedParameter.IsSubclassOf(typeof(UnityEngine.Object))) || (selectedParameter == typeof(UnityEngine.Object)))
            {
                bool flag4 = Array.TrueForAll<AnimationEvent>(events, new Predicate<AnimationEvent>(storey.<>m__3));
                EditorGUI.BeginChangeCheck();
                System.Type objType = typeof(UnityEngine.Object);
                if (selectedParameter != typeof(AnimationEvent))
                {
                    objType = selectedParameter;
                }
                EditorGUI.showMixedValue = !flag4;
                bool allowSceneObjects = false;
                UnityEngine.Object obj2 = EditorGUILayout.ObjectField(ObjectNames.NicifyVariableName(objType.Name), storey.firstEvent.objectReferenceParameter, objType, allowSceneObjects, new GUILayoutOption[0]);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (AnimationEvent event5 in events)
                    {
                        event5.objectReferenceParameter = obj2;
                    }
                }
            }
        }

        private static int EnumPopup(string label, System.Type enumType, int selected)
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
                        System.Type type = behaviour.GetType();
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
                                if (<>f__am$cache1 == null)
                                {
                                    <>f__am$cache1 = p => p.ParameterType;
                                }
                                IEnumerable<System.Type> paramTypes = Enumerable.Select<System.Reflection.ParameterInfo, System.Type>(method.GetParameters(), <>f__am$cache1);
                                return (evt.functionName + FormatEventArguments(paramTypes, evt));
                            }
                        }
                    }
                }
            }
            return (evt.functionName + " (Function Not Supported)");
        }

        private static string FormatEventArguments(IEnumerable<System.Type> paramTypes, AnimationEvent evt)
        {
            if (!paramTypes.Any<System.Type>())
            {
                return " ( )";
            }
            if (paramTypes.Count<System.Type>() <= 1)
            {
                System.Type enumType = paramTypes.First<System.Type>();
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
                if (enumType.IsSubclassOf(typeof(UnityEngine.Object)) || (enumType == typeof(UnityEngine.Object)))
                {
                    return (" ( " + ((evt.objectReferenceParameter != null) ? evt.objectReferenceParameter.name : "null") + " )");
                }
            }
            return " (Function Not Supported)";
        }

        private static AnimationWindowEventData GetData(AnimationWindowEvent[] awEvents)
        {
            AnimationWindowEventData data = new AnimationWindowEventData();
            if (awEvents.Length != 0)
            {
                AnimationWindowEvent event2 = awEvents[0];
                data.root = event2.root;
                data.clip = event2.clip;
                data.clipInfo = event2.clipInfo;
                if (data.clip != null)
                {
                    data.events = AnimationUtility.GetAnimationEvents(data.clip);
                }
                else if (data.clipInfo != null)
                {
                    data.events = data.clipInfo.GetEvents();
                }
                if (data.events != null)
                {
                    List<AnimationEvent> list = new List<AnimationEvent>();
                    foreach (AnimationWindowEvent event3 in awEvents)
                    {
                        if ((event3.eventIndex >= 0) && (event3.eventIndex < data.events.Length))
                        {
                            list.Add(data.events[event3.eventIndex]);
                        }
                    }
                    data.selectedEvents = list.ToArray();
                }
            }
            return data;
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
            AnimationEvent event2 = new AnimationEvent();
            using (new EditorGUI.DisabledScope(true))
            {
                event2.functionName = EditorGUILayout.TextField(new GUIContent("Function"), event2.functionName, new GUILayoutOption[0]);
                AnimationEvent[] events = new AnimationEvent[] { event2 };
                DoEditRegularParameters(events, typeof(AnimationEvent));
            }
        }

        public static void OnEditAnimationEvent(AnimationWindowEvent awe)
        {
            AnimationWindowEvent[] awEvents = new AnimationWindowEvent[] { awe };
            OnEditAnimationEvents(awEvents);
        }

        public static void OnEditAnimationEvents(AnimationWindowEvent[] awEvents)
        {
            <OnEditAnimationEvents>c__AnonStorey0 storey = new <OnEditAnimationEvents>c__AnonStorey0();
            AnimationWindowEventData data = GetData(awEvents);
            if (((data.events != null) && (data.selectedEvents != null)) && (data.selectedEvents.Length != 0))
            {
                storey.firstEvent = data.selectedEvents[0];
                bool flag = Array.TrueForAll<AnimationEvent>(data.selectedEvents, new Predicate<AnimationEvent>(storey.<>m__0));
                GUI.changed = false;
                if (data.root != null)
                {
                    List<AnimationWindowEventMethod> list = CollectSupportedMethods(data.root);
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
                    int selectedIndex = list.FindIndex(new Predicate<AnimationWindowEventMethod>(storey.<>m__1));
                    if (selectedIndex == -1)
                    {
                        selectedIndex = list.Count;
                        AnimationWindowEventMethod item = new AnimationWindowEventMethod {
                            name = storey.firstEvent.functionName,
                            parameterType = null
                        };
                        list.Add(item);
                        if (string.IsNullOrEmpty(storey.firstEvent.functionName))
                        {
                            list2.Add("(No Function Selected)");
                        }
                        else
                        {
                            list2.Add(storey.firstEvent.functionName + " (Function Not Supported)");
                        }
                    }
                    EditorGUIUtility.labelWidth = 130f;
                    EditorGUI.showMixedValue = !flag;
                    int num4 = !flag ? -1 : selectedIndex;
                    selectedIndex = EditorGUILayout.Popup("Function: ", selectedIndex, list2.ToArray(), new GUILayoutOption[0]);
                    if (((num4 != selectedIndex) && (selectedIndex != -1)) && (selectedIndex != count))
                    {
                        foreach (AnimationEvent event2 in data.selectedEvents)
                        {
                            AnimationWindowEventMethod method3 = list[selectedIndex];
                            event2.functionName = method3.name;
                            event2.stringParameter = string.Empty;
                        }
                    }
                    EditorGUI.showMixedValue = false;
                    AnimationWindowEventMethod method4 = list[selectedIndex];
                    System.Type parameterType = method4.parameterType;
                    if (flag && (parameterType != null))
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
                        DoEditRegularParameters(data.selectedEvents, parameterType);
                    }
                }
                else
                {
                    EditorGUI.showMixedValue = !flag;
                    string text = !flag ? "" : storey.firstEvent.functionName;
                    string str3 = EditorGUILayout.TextField(new GUIContent("Function"), text, new GUILayoutOption[0]);
                    if (str3 != text)
                    {
                        foreach (AnimationEvent event3 in data.selectedEvents)
                        {
                            event3.functionName = str3;
                            event3.stringParameter = string.Empty;
                        }
                    }
                    EditorGUI.showMixedValue = false;
                    if (flag)
                    {
                        DoEditRegularParameters(data.selectedEvents, typeof(AnimationEvent));
                    }
                    else
                    {
                        using (new EditorGUI.DisabledScope(true))
                        {
                            AnimationEvent event4 = new AnimationEvent();
                            AnimationEvent[] events = new AnimationEvent[] { event4 };
                            DoEditRegularParameters(events, typeof(AnimationEvent));
                        }
                    }
                }
                if (GUI.changed)
                {
                    SetData(awEvents, data);
                }
            }
        }

        protected override void OnHeaderGUI()
        {
            string header = (base.targets.Length != 1) ? (base.targets.Length + " Animation Events") : "Animation Event";
            Editor.DrawHeaderGUI(this, header);
        }

        public override void OnInspectorGUI()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = o => o as AnimationWindowEvent;
            }
            OnEditAnimationEvents(Enumerable.Select<UnityEngine.Object, AnimationWindowEvent>(base.targets, <>f__am$cache0).ToArray<AnimationWindowEvent>());
        }

        [UnityEditor.MenuItem("CONTEXT/AnimationWindowEvent/Reset")]
        private static void ResetValues(MenuCommand command)
        {
            AnimationWindowEvent context = command.context as AnimationWindowEvent;
            AnimationWindowEvent[] awEvents = new AnimationWindowEvent[] { context };
            AnimationWindowEventData data = GetData(awEvents);
            if (((data.events != null) && (data.selectedEvents != null)) && (data.selectedEvents.Length != 0))
            {
                foreach (AnimationEvent event3 in data.selectedEvents)
                {
                    event3.functionName = "";
                    event3.stringParameter = string.Empty;
                    event3.floatParameter = 0f;
                    event3.intParameter = 0;
                    event3.objectReferenceParameter = null;
                }
                SetData(awEvents, data);
            }
        }

        private static void SetData(AnimationWindowEvent[] awEvents, AnimationWindowEventData data)
        {
            if (data.events != null)
            {
                if (data.clip != null)
                {
                    Undo.RegisterCompleteObjectUndo(data.clip, "Animation Event Change");
                    AnimationUtility.SetAnimationEvents(data.clip, data.events);
                }
                else if (data.clipInfo != null)
                {
                    foreach (AnimationWindowEvent event2 in awEvents)
                    {
                        if ((event2.eventIndex >= 0) && (event2.eventIndex < data.events.Length))
                        {
                            data.clipInfo.SetEvent(event2.eventIndex, data.events[event2.eventIndex]);
                        }
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <CollectSupportedMethods>c__AnonStorey1
        {
            internal string name;

            internal bool <>m__0(AnimationWindowEventMethod m) => 
                (m.name == this.name);
        }

        [CompilerGenerated]
        private sealed class <DoEditRegularParameters>c__AnonStorey2
        {
            internal AnimationEvent firstEvent;

            internal bool <>m__0(AnimationEvent evt) => 
                (evt.floatParameter == this.firstEvent.floatParameter);

            internal bool <>m__1(AnimationEvent evt) => 
                (evt.intParameter == this.firstEvent.intParameter);

            internal bool <>m__2(AnimationEvent evt) => 
                (evt.stringParameter == this.firstEvent.stringParameter);

            internal bool <>m__3(AnimationEvent evt) => 
                (evt.objectReferenceParameter == this.firstEvent.objectReferenceParameter);
        }

        [CompilerGenerated]
        private sealed class <OnEditAnimationEvents>c__AnonStorey0
        {
            internal AnimationEvent firstEvent;

            internal bool <>m__0(AnimationEvent evt) => 
                (evt.functionName == this.firstEvent.functionName);

            internal bool <>m__1(AnimationWindowEventMethod method) => 
                (method.name == this.firstEvent.functionName);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AnimationWindowEventData
        {
            public GameObject root;
            public AnimationClip clip;
            public AnimationClipInfoProperties clipInfo;
            public AnimationEvent[] events;
            public AnimationEvent[] selectedEvents;
        }
    }
}

