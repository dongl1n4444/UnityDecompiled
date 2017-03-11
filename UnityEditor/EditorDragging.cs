namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class EditorDragging
    {
        [CompilerGenerated]
        private static Func<UnityEngine.Object, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<UnityEngine.Object, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Converter<UnityEngine.Object, Component> <>f__am$cache2;
        [CompilerGenerated]
        private static Converter<UnityEngine.Object, Component> <>f__am$cache3;
        [CompilerGenerated]
        private static Converter<UnityEngine.Object, Component> <>f__am$cache4;
        [CompilerGenerated]
        private static Converter<UnityEngine.Object, MonoScript> <>f__am$cache5;
        [CompilerGenerated]
        private static Converter<UnityEngine.Object, Component> <>f__am$cache6;
        private const string k_DraggingModeKey = "InspectorEditorDraggingMode";
        private InspectorWindow m_InspectorWindow;
        private int m_LastIndex = -1;
        private float m_LastMarkerY = 0f;
        private bool m_TargetAbove;
        private int m_TargetIndex = -1;

        public EditorDragging(InspectorWindow inspectorWindow)
        {
            this.m_InspectorWindow = inspectorWindow;
        }

        public void HandleDraggingToBottomArea(Rect bottomRect, ActiveEditorTracker tracker)
        {
            this.HandleEditorDragging(this.m_LastIndex, bottomRect, this.m_LastMarkerY, true, tracker);
        }

        public void HandleDraggingToEditor(int editorIndex, Rect dragRect, Rect contentRect, ActiveEditorTracker tracker)
        {
            if (dragRect.height != 0f)
            {
                if (contentRect.height == 0f)
                {
                    contentRect = dragRect;
                }
                float num = 8f;
                Rect targetRect = new Rect(contentRect.x, contentRect.yMax - (num - 2f), contentRect.width, (num * 2f) + 1f);
                float yMax = contentRect.yMax;
                this.m_LastIndex = editorIndex;
                this.m_LastMarkerY = yMax;
                this.HandleEditorDragging(editorIndex, targetRect, yMax, false, tracker);
            }
        }

        private void HandleEditorDragging(int editorIndex, Rect targetRect, float markerY, bool bottomTarget, ActiveEditorTracker tracker)
        {
            DraggingMode? genericData;
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.Repaint:
                    if ((this.m_TargetIndex != -1) && targetRect.Contains(current.mousePosition))
                    {
                        Rect position = new Rect(targetRect.x, markerY, targetRect.width, 3f);
                        if (!this.m_TargetAbove)
                        {
                            position.y += 2f;
                        }
                        Styles.insertionMarker.Draw(position, false, false, false, false);
                    }
                    return;

                case EventType.DragUpdated:
                    if (!targetRect.Contains(current.mousePosition))
                    {
                        this.m_TargetIndex = -1;
                        return;
                    }
                    genericData = DragAndDrop.GetGenericData("InspectorEditorDraggingMode") as DraggingMode?;
                    if (!genericData.HasValue)
                    {
                        UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
                        if (objectReferences.Length != 0)
                        {
                            if (<>f__am$cache0 == null)
                            {
                                <>f__am$cache0 = o => (o is Component) && !(o is Transform);
                            }
                            if (Enumerable.All<UnityEngine.Object>(objectReferences, <>f__am$cache0))
                            {
                                genericData = 1;
                            }
                            else
                            {
                                if (<>f__am$cache1 == null)
                                {
                                    <>f__am$cache1 = o => o is MonoScript;
                                }
                                if (Enumerable.All<UnityEngine.Object>(objectReferences, <>f__am$cache1))
                                {
                                    genericData = 2;
                                }
                                else
                                {
                                    genericData = 0;
                                }
                            }
                        }
                        else
                        {
                            genericData = 0;
                        }
                        DragAndDrop.SetGenericData("InspectorEditorDraggingMode", genericData);
                    }
                    break;

                case EventType.DragPerform:
                    if (this.m_TargetIndex != -1)
                    {
                        DraggingMode? nullable2 = DragAndDrop.GetGenericData("InspectorEditorDraggingMode") as DraggingMode?;
                        if (nullable2.HasValue && (((DraggingMode) nullable2.Value) != DraggingMode.NotApplicable))
                        {
                            if (<>f__am$cache4 == null)
                            {
                                <>f__am$cache4 = o => o as Component;
                            }
                            Component[] targetComponents = Array.ConvertAll<UnityEngine.Object, Component>(tracker.activeEditors[this.m_TargetIndex].targets, <>f__am$cache4).ToArray<Component>();
                            if (((DraggingMode) nullable2.Value) != DraggingMode.Script)
                            {
                                if (<>f__am$cache6 == null)
                                {
                                    <>f__am$cache6 = o => o as Component;
                                }
                                Component[] sourceComponents = Array.ConvertAll<UnityEngine.Object, Component>(DragAndDrop.objectReferences, <>f__am$cache6).ToArray<Component>();
                                if ((sourceComponents.Length == 0) || (targetComponents.Length == 0))
                                {
                                    return;
                                }
                                this.MoveOrCopyComponents(sourceComponents, targetComponents, EditorUtility.EventHasDragCopyModifierPressed(current), false);
                            }
                            else
                            {
                                if (<>f__am$cache5 == null)
                                {
                                    <>f__am$cache5 = o => o as MonoScript;
                                }
                                MonoScript[] scriptArray = Array.ConvertAll<UnityEngine.Object, MonoScript>(DragAndDrop.objectReferences, <>f__am$cache5);
                                bool flag2 = true;
                                Component[] componentArray4 = targetComponents;
                                for (int i = 0; i < componentArray4.Length; i++)
                                {
                                    <HandleEditorDragging>c__AnonStorey0 storey = new <HandleEditorDragging>c__AnonStorey0 {
                                        targetComponent = componentArray4[i]
                                    };
                                    GameObject gameObject = storey.targetComponent.gameObject;
                                    if (Enumerable.Any<MonoScript>(scriptArray, new Func<MonoScript, bool>(storey.<>m__0)))
                                    {
                                        flag2 = false;
                                        break;
                                    }
                                }
                                if (flag2)
                                {
                                    foreach (Component component in targetComponents)
                                    {
                                        GameObject obj3 = component.gameObject;
                                        foreach (MonoScript script in scriptArray)
                                        {
                                            Component component2 = Undo.AddComponent(obj3, script.GetClass());
                                            if (component2 != null)
                                            {
                                                ComponentUtility.MoveComponentRelativeToComponent(component2, component, this.m_TargetAbove);
                                            }
                                        }
                                    }
                                }
                            }
                            this.m_TargetIndex = -1;
                            DragAndDrop.AcceptDrag();
                            current.Use();
                            GUIUtility.ExitGUI();
                            return;
                        }
                        this.m_TargetIndex = -1;
                    }
                    return;

                case EventType.DragExited:
                    this.m_TargetIndex = -1;
                    return;

                default:
                    return;
            }
            if (((DraggingMode) genericData.Value) != DraggingMode.NotApplicable)
            {
                Editor[] activeEditors = tracker.activeEditors;
                UnityEngine.Object[] objArray2 = DragAndDrop.objectReferences;
                if (bottomTarget)
                {
                    this.m_TargetAbove = false;
                    this.m_TargetIndex = this.m_LastIndex;
                }
                else
                {
                    this.m_TargetAbove = current.mousePosition.y < (targetRect.y + (targetRect.height / 2f));
                    this.m_TargetIndex = editorIndex;
                    if (this.m_TargetAbove)
                    {
                        this.m_TargetIndex++;
                        while ((this.m_TargetIndex < activeEditors.Length) && this.m_InspectorWindow.ShouldCullEditor(activeEditors, this.m_TargetIndex))
                        {
                            this.m_TargetIndex++;
                        }
                        if (this.m_TargetIndex == activeEditors.Length)
                        {
                            this.m_TargetIndex = -1;
                            return;
                        }
                    }
                }
                if (this.m_TargetAbove && this.m_InspectorWindow.EditorHasLargeHeader(this.m_TargetIndex))
                {
                    this.m_TargetIndex--;
                    while ((this.m_TargetIndex >= 0) && this.m_InspectorWindow.ShouldCullEditor(activeEditors, this.m_TargetIndex))
                    {
                        this.m_TargetIndex--;
                    }
                    if (this.m_TargetIndex == -1)
                    {
                        return;
                    }
                    this.m_TargetAbove = false;
                }
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = o => o as Component;
                }
                Component[] componentArray = Array.ConvertAll<UnityEngine.Object, Component>(activeEditors[this.m_TargetIndex].targets, <>f__am$cache2).ToArray<Component>();
                if (((DraggingMode) genericData.Value) == DraggingMode.Script)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                }
                else
                {
                    if (<>f__am$cache3 == null)
                    {
                        <>f__am$cache3 = o => o as Component;
                    }
                    Component[] componentArray2 = Array.ConvertAll<UnityEngine.Object, Component>(DragAndDrop.objectReferences, <>f__am$cache3).ToArray<Component>();
                    if (this.MoveOrCopyComponents(componentArray2, componentArray, EditorUtility.EventHasDragCopyModifierPressed(current), true))
                    {
                        DragAndDrop.visualMode = !EditorUtility.EventHasDragCopyModifierPressed(current) ? DragAndDropVisualMode.Move : DragAndDropVisualMode.Copy;
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.None;
                        this.m_TargetIndex = -1;
                    }
                }
                current.Use();
            }
        }

        private bool MoveOrCopyComponents(Component[] sourceComponents, Component[] targetComponents, bool copy, bool validateOnly)
        {
            if (copy)
            {
                return false;
            }
            if ((sourceComponents.Length == 1) && (targetComponents.Length == 1))
            {
                if (sourceComponents[0].gameObject != targetComponents[0].gameObject)
                {
                    return false;
                }
                return ComponentUtility.MoveComponentRelativeToComponent(sourceComponents[0], targetComponents[0], this.m_TargetAbove, validateOnly);
            }
            return ComponentUtility.MoveComponentsRelativeToComponents(sourceComponents, targetComponents, this.m_TargetAbove, validateOnly);
        }

        [CompilerGenerated]
        private sealed class <HandleEditorDragging>c__AnonStorey0
        {
            internal Component targetComponent;

            internal bool <>m__0(MonoScript s) => 
                !ComponentUtility.WarnCanAddScriptComponent(this.targetComponent.gameObject, s);
        }

        private enum DraggingMode
        {
            NotApplicable,
            Component,
            Script
        }

        private static class Styles
        {
            public static readonly GUIStyle insertionMarker = "InsertionMarker";
        }
    }
}

