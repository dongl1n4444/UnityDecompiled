namespace UnityEditor.Events
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [CustomPreview(typeof(GameObject))]
    internal class InterceptedEventsPreview : ObjectPreview
    {
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cache0;
        private bool m_InterceptsAnyEvent = false;
        private Styles m_Styles = new Styles();
        private Dictionary<GameObject, List<ComponentInterceptedEvents>> m_TargetEvents;
        private GUIContent m_Title;
        private static readonly Dictionary<System.Type, ComponentInterceptedEvents> s_ComponentEvents2 = new Dictionary<System.Type, ComponentInterceptedEvents>();
        private static List<System.Type> s_EventSystemInterfaces = null;
        private static Dictionary<System.Type, List<int>> s_InterfaceEventSystemEvents = null;
        private static List<GUIContent> s_PossibleEvents = null;

        [DebuggerHidden]
        private static IEnumerable<System.Type> GetAccessibleTypesInLoadedAssemblies() => 
            new <GetAccessibleTypesInLoadedAssemblies>c__Iterator0 { $PC = -2 };

        protected static List<ComponentInterceptedEvents> GetEventsInfo(GameObject gameObject)
        {
            InitializeEvetnsInterfaceCacheIfNeeded();
            List<ComponentInterceptedEvents> list = new List<ComponentInterceptedEvents>();
            MonoBehaviour[] components = gameObject.GetComponents<MonoBehaviour>();
            int index = 0;
            int length = components.Length;
            while (index < length)
            {
                ComponentInterceptedEvents events = null;
                MonoBehaviour behaviour = components[index];
                if (behaviour != null)
                {
                    System.Type key = behaviour.GetType();
                    if (!s_ComponentEvents2.ContainsKey(key))
                    {
                        List<int> list2 = null;
                        if (typeof(IEventSystemHandler).IsAssignableFrom(key))
                        {
                            for (int i = 0; i < s_EventSystemInterfaces.Count; i++)
                            {
                                System.Type type2 = s_EventSystemInterfaces[i];
                                if (type2.IsAssignableFrom(key))
                                {
                                    if (list2 == null)
                                    {
                                        list2 = new List<int>();
                                    }
                                    list2.AddRange(s_InterfaceEventSystemEvents[type2]);
                                }
                            }
                        }
                        if (list2 != null)
                        {
                            events = new ComponentInterceptedEvents {
                                componentName = new GUIContent(key.Name)
                            };
                            if (<>f__am$cache0 == null)
                            {
                                <>f__am$cache0 = new Func<int, string>(null, (IntPtr) <GetEventsInfo>m__0);
                            }
                            events.interceptedEvents = Enumerable.OrderBy<int, string>(list2, <>f__am$cache0).ToArray<int>();
                        }
                        s_ComponentEvents2.Add(key, events);
                    }
                    else
                    {
                        events = s_ComponentEvents2[key];
                    }
                    if (events != null)
                    {
                        list.Add(events);
                    }
                }
                index++;
            }
            return list;
        }

        public override GUIContent GetPreviewTitle()
        {
            if (this.m_Title == null)
            {
                this.m_Title = new GUIContent("Intercepted Events");
            }
            return this.m_Title;
        }

        public override bool HasPreviewGUI() => 
            ((this.m_TargetEvents != null) && this.m_InterceptsAnyEvent);

        public override void Initialize(UnityEngine.Object[] targets)
        {
            base.Initialize(targets);
            this.m_TargetEvents = new Dictionary<GameObject, List<ComponentInterceptedEvents>>(targets.Count<UnityEngine.Object>());
            this.m_InterceptsAnyEvent = false;
            for (int i = 0; i < targets.Length; i++)
            {
                GameObject gameObject = targets[i] as GameObject;
                List<ComponentInterceptedEvents> eventsInfo = GetEventsInfo(gameObject);
                this.m_TargetEvents.Add(gameObject, eventsInfo);
                if (eventsInfo.Any<ComponentInterceptedEvents>())
                {
                    this.m_InterceptsAnyEvent = true;
                }
            }
        }

        private static void InitializeEvetnsInterfaceCacheIfNeeded()
        {
            if (s_EventSystemInterfaces == null)
            {
                s_EventSystemInterfaces = new List<System.Type>();
                s_PossibleEvents = new List<GUIContent>();
                s_InterfaceEventSystemEvents = new Dictionary<System.Type, List<int>>();
                foreach (System.Type type in GetAccessibleTypesInLoadedAssemblies())
                {
                    if (type.IsInterface && typeof(IEventSystemHandler).IsAssignableFrom(type))
                    {
                        s_EventSystemInterfaces.Add(type);
                        List<int> list = new List<int>();
                        foreach (MethodInfo info in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                        {
                            list.Add(s_PossibleEvents.Count);
                            s_PossibleEvents.Add(new GUIContent(info.Name));
                        }
                        s_InterfaceEventSystemEvents.Add(type, list);
                    }
                }
            }
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (this.m_Styles == null)
                {
                    this.m_Styles = new Styles();
                }
                Vector2 zero = Vector2.zero;
                int num = 0;
                List<ComponentInterceptedEvents> list = this.m_TargetEvents[this.target as GameObject];
                foreach (ComponentInterceptedEvents events in list)
                {
                    foreach (int num2 in events.interceptedEvents)
                    {
                        GUIContent content = s_PossibleEvents[num2];
                        num++;
                        Vector2 vector2 = this.m_Styles.labelStyle.CalcSize(content);
                        if (zero.x < vector2.x)
                        {
                            zero.x = vector2.x;
                        }
                        if (zero.y < vector2.y)
                        {
                            zero.y = vector2.y;
                        }
                    }
                }
                r = new RectOffset(-5, -5, -5, -5).Add(r);
                int num4 = Mathf.Max(Mathf.FloorToInt(r.width / zero.x), 1);
                int num5 = Mathf.Max(num / num4, 1) + list.Count;
                float x = r.x + Mathf.Max((float) 0f, (float) ((r.width - (zero.x * num4)) / 2f));
                float y = r.y + Mathf.Max((float) 0f, (float) ((r.height - (zero.y * num5)) / 2f));
                Rect position = new Rect(x, y, zero.x, zero.y);
                int num8 = 0;
                foreach (ComponentInterceptedEvents events2 in list)
                {
                    GUI.Label(position, events2.componentName, this.m_Styles.componentName);
                    position.y += position.height;
                    position.x = x;
                    foreach (int num9 in events2.interceptedEvents)
                    {
                        GUIContent content2 = s_PossibleEvents[num9];
                        GUI.Label(position, content2, this.m_Styles.labelStyle);
                        if (num8 < (num4 - 1))
                        {
                            position.x += position.width;
                        }
                        else
                        {
                            position.y += position.height;
                            position.x = x;
                        }
                        num8 = (num8 + 1) % num4;
                    }
                    if (position.x != x)
                    {
                        position.y += position.height;
                        position.x = x;
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <GetAccessibleTypesInLoadedAssemblies>c__Iterator0 : IEnumerable, IEnumerable<System.Type>, IEnumerator, IDisposable, IEnumerator<System.Type>
        {
            internal System.Type $current;
            internal bool $disposing;
            internal int $PC;
            internal Assembly[] <assemblies>__0;
            internal Assembly <assembly>__2;
            internal int <i>__1;
            internal int <j>__4;
            internal System.Type <type>__5;
            internal System.Type[] <types>__3;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<assemblies>__0 = AppDomain.CurrentDomain.GetAssemblies();
                        this.<i>__1 = 0;
                        while (this.<i>__1 < this.<assemblies>__0.Length)
                        {
                            this.<assembly>__2 = this.<assemblies>__0[this.<i>__1];
                            if (this.<assembly>__2 != null)
                            {
                                try
                                {
                                    this.<types>__3 = this.<assembly>__2.GetTypes();
                                }
                                catch (ReflectionTypeLoadException exception)
                                {
                                    this.<types>__3 = exception.Types;
                                }
                                this.<j>__4 = 0;
                                while (this.<j>__4 < this.<types>__3.Length)
                                {
                                    this.<type>__5 = this.<types>__3[this.<j>__4];
                                    if (this.<type>__5 != null)
                                    {
                                        this.$current = this.<type>__5;
                                        if (!this.$disposing)
                                        {
                                            this.$PC = 1;
                                        }
                                        return true;
                                    }
                                Label_00DF:
                                    this.<j>__4++;
                                }
                            }
                            this.<i>__1++;
                        }
                        this.$PC = -1;
                        break;

                    case 1:
                        goto Label_00DF;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<System.Type> IEnumerable<System.Type>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new InterceptedEventsPreview.<GetAccessibleTypesInLoadedAssemblies>c__Iterator0();
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<System.Type>.GetEnumerator();

            System.Type IEnumerator<System.Type>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        protected class ComponentInterceptedEvents
        {
            public GUIContent componentName;
            public int[] interceptedEvents;
        }

        private class Styles
        {
            public GUIStyle componentName = new GUIStyle(EditorStyles.boldLabel);
            public GUIStyle labelStyle = new GUIStyle(EditorStyles.label);

            public Styles()
            {
                Color color = new Color(0.7f, 0.7f, 0.7f);
                RectOffset padding = this.labelStyle.padding;
                padding.right += 20;
                this.labelStyle.normal.textColor = color;
                this.labelStyle.active.textColor = color;
                this.labelStyle.focused.textColor = color;
                this.labelStyle.hover.textColor = color;
                this.labelStyle.onNormal.textColor = color;
                this.labelStyle.onActive.textColor = color;
                this.labelStyle.onFocused.textColor = color;
                this.labelStyle.onHover.textColor = color;
                this.componentName.normal.textColor = color;
                this.componentName.active.textColor = color;
                this.componentName.focused.textColor = color;
                this.componentName.hover.textColor = color;
                this.componentName.onNormal.textColor = color;
                this.componentName.onActive.textColor = color;
                this.componentName.onFocused.textColor = color;
                this.componentName.onHover.textColor = color;
            }
        }
    }
}

