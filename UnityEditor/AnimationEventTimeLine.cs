namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    [Serializable]
    internal class AnimationEventTimeLine
    {
        [CompilerGenerated]
        private static Func<bool, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<bool, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Predicate<bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Predicate<bool> <>f__am$cache3;
        private bool m_DirtyTooltip = false;
        [NonSerialized]
        private AnimationEvent[] m_EventsAtMouseDown;
        [NonSerialized]
        private float[] m_EventTimes;
        private int m_HoverEvent = -1;
        private Vector2 m_InstantTooltipPoint = Vector2.zero;
        private string m_InstantTooltipText = null;

        public AnimationEventTimeLine(EditorWindow owner)
        {
        }

        public void AddEvent(float time, GameObject gameObject, AnimationClip animationClip)
        {
            Selection.activeObject = AnimationWindowEvent.CreateAndEdit(gameObject, animationClip, time);
        }

        private void CheckRectsOnMouseMove(Rect eventLineRect, AnimationEvent[] events, Rect[] hitRects)
        {
            Vector2 mousePosition = Event.current.mousePosition;
            bool flag = false;
            if (events.Length == hitRects.Length)
            {
                for (int i = hitRects.Length - 1; i >= 0; i--)
                {
                    if (hitRects[i].Contains(mousePosition))
                    {
                        flag = true;
                        if (this.m_HoverEvent != i)
                        {
                            this.m_HoverEvent = i;
                            this.m_InstantTooltipText = events[this.m_HoverEvent].functionName;
                            this.m_InstantTooltipPoint = new Vector2((hitRects[this.m_HoverEvent].xMin + ((int) (hitRects[this.m_HoverEvent].width / 2f))) + eventLineRect.x, eventLineRect.yMax);
                            this.m_DirtyTooltip = true;
                        }
                    }
                }
            }
            if (!flag)
            {
                this.m_HoverEvent = -1;
                this.m_InstantTooltipText = "";
            }
        }

        public void DeleteEvents(AnimationClip clip, bool[] deleteIndices)
        {
            bool flag = false;
            List<AnimationEvent> list = new List<AnimationEvent>(AnimationUtility.GetAnimationEvents(clip));
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (deleteIndices[i])
                {
                    list.RemoveAt(i);
                    flag = true;
                }
            }
            if (flag)
            {
                Undo.RegisterCompleteObjectUndo(clip, "Delete Event");
                AnimationUtility.SetAnimationEvents(clip, list.ToArray());
                Selection.objects = new AnimationWindowEvent[0];
                this.m_DirtyTooltip = true;
            }
        }

        public void DrawInstantTooltip(Rect position)
        {
            if ((this.m_InstantTooltipText != null) && (this.m_InstantTooltipText != ""))
            {
                GUIStyle style = "AnimationEventTooltip";
                style.contentOffset = new Vector2(0f, 0f);
                style.overflow = new RectOffset(10, 10, 0, 0);
                Vector2 vector = style.CalcSize(new GUIContent(this.m_InstantTooltipText));
                Rect rect = new Rect(this.m_InstantTooltipPoint.x - (vector.x * 0.5f), this.m_InstantTooltipPoint.y + 24f, vector.x, vector.y);
                if (rect.xMax > position.width)
                {
                    rect.x = position.width - rect.width;
                }
                GUI.Label(rect, this.m_InstantTooltipText, style);
                rect = new Rect(this.m_InstantTooltipPoint.x - 33f, this.m_InstantTooltipPoint.y, 7f, 25f);
                GUI.Label(rect, "", "AnimationEventTooltipArrow");
            }
        }

        public void EditEvent(GameObject gameObject, AnimationClip clip, int index)
        {
            Selection.activeObject = AnimationWindowEvent.Edit(gameObject, clip, index);
        }

        public void EditEvents(GameObject gameObject, AnimationClip clip, bool[] selectedIndices)
        {
            List<AnimationWindowEvent> list = new List<AnimationWindowEvent>();
            for (int i = 0; i < selectedIndices.Length; i++)
            {
                if (selectedIndices[i])
                {
                    list.Add(AnimationWindowEvent.Edit(gameObject, clip, i));
                }
            }
            Selection.objects = list.ToArray();
        }

        public void EventLineContextMenuAdd(object obj)
        {
            EventLineContextMenuObject obj2 = (EventLineContextMenuObject) obj;
            this.AddEvent(obj2.m_Time, obj2.m_Animated, obj2.m_Clip);
        }

        public void EventLineContextMenuDelete(object obj)
        {
            EventLineContextMenuObject obj2 = (EventLineContextMenuObject) obj;
            AnimationClip clip = obj2.m_Clip;
            if (clip != null)
            {
                int index = obj2.m_Index;
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = selected => selected;
                }
                if (Array.Exists<bool>(obj2.m_Selected, <>f__am$cache3))
                {
                    this.DeleteEvents(clip, obj2.m_Selected);
                }
                else if (index >= 0)
                {
                    bool[] deleteIndices = new bool[obj2.m_Selected.Length];
                    deleteIndices[index] = true;
                    this.DeleteEvents(clip, deleteIndices);
                }
            }
        }

        public void EventLineContextMenuEdit(object obj)
        {
            EventLineContextMenuObject obj2 = (EventLineContextMenuObject) obj;
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = selected => selected;
            }
            if (Array.Exists<bool>(obj2.m_Selected, <>f__am$cache2))
            {
                this.EditEvents(obj2.m_Animated, obj2.m_Clip, obj2.m_Selected);
            }
            else if (obj2.m_Index >= 0)
            {
                this.EditEvent(obj2.m_Animated, obj2.m_Clip, obj2.m_Index);
            }
        }

        public void EventLineGUI(Rect rect, AnimationWindowState state)
        {
            if (state.selectedItem != null)
            {
                AnimationClip animationClip = state.selectedItem.animationClip;
                GameObject rootGameObject = state.selectedItem.rootGameObject;
                GUI.BeginGroup(rect);
                Color color = GUI.color;
                Rect rect2 = new Rect(0f, 0f, rect.width, rect.height);
                float time = Mathf.Max((float) (((float) Mathf.RoundToInt(state.PixelToTime(Event.current.mousePosition.x, rect) * state.frameRate)) / state.frameRate), (float) 0f);
                if (animationClip != null)
                {
                    int num9;
                    float num10;
                    float num11;
                    AnimationEvent[] animationEvents = AnimationUtility.GetAnimationEvents(animationClip);
                    Texture image = EditorGUIUtility.IconContent("Animation.EventMarker").image;
                    Rect[] hitPositions = new Rect[animationEvents.Length];
                    Rect[] positions = new Rect[animationEvents.Length];
                    int num2 = 1;
                    int num3 = 0;
                    for (int i = 0; i < animationEvents.Length; i++)
                    {
                        AnimationEvent event2 = animationEvents[i];
                        if (num3 == 0)
                        {
                            num2 = 1;
                            while (((i + num2) < animationEvents.Length) && (animationEvents[i + num2].time == event2.time))
                            {
                                num2++;
                            }
                            num3 = num2;
                        }
                        num3--;
                        float num5 = Mathf.Floor(state.FrameToPixel(event2.time * animationClip.frameRate, rect));
                        int num6 = 0;
                        if (num2 > 1)
                        {
                            float num7 = Mathf.Min((int) ((num2 - 1) * (image.width - 1)), (int) (((int) state.FrameDeltaToPixel(rect)) - (image.width * 2)));
                            num6 = Mathf.FloorToInt(Mathf.Max((float) 0f, (float) (num7 - ((image.width - 1) * num3))));
                        }
                        Rect rect3 = new Rect((num5 + num6) - (image.width / 2), ((rect.height - 10f) * ((num3 - num2) + 1)) / ((float) Mathf.Max(1, num2 - 1)), (float) image.width, (float) image.height);
                        hitPositions[i] = rect3;
                        positions[i] = rect3;
                    }
                    if (this.m_DirtyTooltip)
                    {
                        if ((this.m_HoverEvent >= 0) && (this.m_HoverEvent < hitPositions.Length))
                        {
                            this.m_InstantTooltipText = AnimationWindowEventInspector.FormatEvent(rootGameObject, animationEvents[this.m_HoverEvent]);
                            this.m_InstantTooltipPoint = new Vector2(((hitPositions[this.m_HoverEvent].xMin + ((int) (hitPositions[this.m_HoverEvent].width / 2f))) + rect.x) - 30f, rect.yMax);
                        }
                        this.m_DirtyTooltip = false;
                    }
                    bool[] selections = new bool[animationEvents.Length];
                    UnityEngine.Object[] objects = Selection.objects;
                    foreach (UnityEngine.Object obj3 in objects)
                    {
                        AnimationWindowEvent event3 = obj3 as AnimationWindowEvent;
                        if ((event3 != null) && ((event3.eventIndex >= 0) && (event3.eventIndex < selections.Length)))
                        {
                            selections[event3.eventIndex] = true;
                        }
                    }
                    Vector2 zero = Vector2.zero;
                    switch (EditorGUIExt.MultiSelection(rect, positions, new GUIContent(image), hitPositions, ref selections, null, out num9, out zero, out num10, out num11, GUIStyle.none))
                    {
                        case HighLevelEvent.DoubleClick:
                            if (num9 == -1)
                            {
                                this.EventLineContextMenuAdd(new EventLineContextMenuObject(rootGameObject, animationClip, time, -1, selections));
                                break;
                            }
                            this.EditEvents(rootGameObject, animationClip, selections);
                            break;

                        case HighLevelEvent.ContextClick:
                        {
                            GenericMenu menu = new GenericMenu();
                            EventLineContextMenuObject userData = new EventLineContextMenuObject(rootGameObject, animationClip, animationEvents[num9].time, num9, selections);
                            if (<>f__am$cache0 == null)
                            {
                                <>f__am$cache0 = selected => selected;
                            }
                            int num16 = Enumerable.Count<bool>(selections, <>f__am$cache0);
                            menu.AddItem(new GUIContent("Add Animation Event"), false, new GenericMenu.MenuFunction2(this.EventLineContextMenuAdd), userData);
                            menu.AddItem(new GUIContent((num16 <= 1) ? "Delete Animation Event" : "Delete Animation Events"), false, new GenericMenu.MenuFunction2(this.EventLineContextMenuDelete), userData);
                            menu.ShowAsContext();
                            this.m_InstantTooltipText = null;
                            this.m_DirtyTooltip = true;
                            state.Repaint();
                            break;
                        }
                        case HighLevelEvent.BeginDrag:
                            this.m_EventsAtMouseDown = animationEvents;
                            this.m_EventTimes = new float[animationEvents.Length];
                            for (int j = 0; j < animationEvents.Length; j++)
                            {
                                this.m_EventTimes[j] = animationEvents[j].time;
                            }
                            break;

                        case HighLevelEvent.Drag:
                        {
                            for (int k = animationEvents.Length - 1; k >= 0; k--)
                            {
                                if (selections[k])
                                {
                                    AnimationEvent event5 = this.m_EventsAtMouseDown[k];
                                    event5.time = this.m_EventTimes[k] + (zero.x * state.PixelDeltaToTime(rect));
                                    event5.time = Mathf.Max(0f, event5.time);
                                    event5.time = ((float) Mathf.RoundToInt(event5.time * animationClip.frameRate)) / animationClip.frameRate;
                                }
                            }
                            int[] items = new int[selections.Length];
                            for (int m = 0; m < items.Length; m++)
                            {
                                items[m] = m;
                            }
                            Array.Sort(this.m_EventsAtMouseDown, items, new EventComparer());
                            bool[] flagArray2 = (bool[]) selections.Clone();
                            float[] numArray2 = (float[]) this.m_EventTimes.Clone();
                            for (int n = 0; n < items.Length; n++)
                            {
                                selections[n] = flagArray2[items[n]];
                                this.m_EventTimes[n] = numArray2[items[n]];
                            }
                            this.EditEvents(rootGameObject, animationClip, selections);
                            Undo.RegisterCompleteObjectUndo(animationClip, "Move Event");
                            AnimationUtility.SetAnimationEvents(animationClip, this.m_EventsAtMouseDown);
                            this.m_DirtyTooltip = true;
                            break;
                        }
                        case HighLevelEvent.Delete:
                            this.DeleteEvents(animationClip, selections);
                            break;

                        case HighLevelEvent.SelectionChanged:
                            state.ClearKeySelections();
                            this.EditEvents(rootGameObject, animationClip, selections);
                            break;
                    }
                    this.CheckRectsOnMouseMove(rect, animationEvents, hitPositions);
                    if ((Event.current.type == EventType.ContextClick) && rect2.Contains(Event.current.mousePosition))
                    {
                        Event.current.Use();
                        GenericMenu menu2 = new GenericMenu();
                        EventLineContextMenuObject obj5 = new EventLineContextMenuObject(rootGameObject, animationClip, time, -1, selections);
                        if (<>f__am$cache1 == null)
                        {
                            <>f__am$cache1 = selected => selected;
                        }
                        int num17 = Enumerable.Count<bool>(selections, <>f__am$cache1);
                        menu2.AddItem(new GUIContent("Add Animation Event"), false, new GenericMenu.MenuFunction2(this.EventLineContextMenuAdd), obj5);
                        if (num17 > 0)
                        {
                            menu2.AddItem(new GUIContent((num17 <= 1) ? "Delete Animation Event" : "Delete Animation Events"), false, new GenericMenu.MenuFunction2(this.EventLineContextMenuDelete), obj5);
                        }
                        menu2.ShowAsContext();
                    }
                }
                GUI.color = color;
                GUI.EndGroup();
            }
        }

        public class EventComparer : IComparer
        {
            int IComparer.Compare(object objX, object objY)
            {
                AnimationEvent event2 = (AnimationEvent) objX;
                AnimationEvent event3 = (AnimationEvent) objY;
                float time = event2.time;
                float num2 = event3.time;
                if (time != num2)
                {
                    return (int) Mathf.Sign(time - num2);
                }
                int hashCode = event2.GetHashCode();
                int num5 = event3.GetHashCode();
                return (hashCode - num5);
            }
        }

        private class EventLineContextMenuObject
        {
            public GameObject m_Animated;
            public AnimationClip m_Clip;
            public int m_Index;
            public bool[] m_Selected;
            public float m_Time;

            public EventLineContextMenuObject(GameObject animated, AnimationClip clip, float time, int index, bool[] selected)
            {
                this.m_Animated = animated;
                this.m_Clip = clip;
                this.m_Time = time;
                this.m_Index = index;
                this.m_Selected = selected;
            }
        }
    }
}

