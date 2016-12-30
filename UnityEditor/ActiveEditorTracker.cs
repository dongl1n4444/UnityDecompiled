namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [Serializable]
    public sealed class ActiveEditorTracker
    {
        private MonoReloadableIntPtrClear m_Property;

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        public extern ActiveEditorTracker();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ClearDirty();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Destroy();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
        private extern void Dispose();
        public override bool Equals(object o)
        {
            ActiveEditorTracker tracker = o as ActiveEditorTracker;
            return (this.m_Property.m_IntPtr == tracker.m_Property.m_IntPtr);
        }

        ~ActiveEditorTracker()
        {
            this.Dispose();
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ForceRebuild();
        public override int GetHashCode() => 
            this.m_Property.m_IntPtr.GetHashCode();

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern int GetVisible(int index);
        public static bool HasCustomEditor(Object obj) => 
            (CustomEditorAttributes.FindCustomEditorType(obj, false) != null);

        [Obsolete("Use Editor.CreateEditor instead")]
        public static Editor MakeCustomEditor(Object obj) => 
            Editor.CreateEditor(obj);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void RebuildIfNecessary();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void SetupSharedTracker(ActiveEditorTracker sharedTracker);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetVisible(int index, int visible);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void VerifyModifiedMonoBehaviours();

        public Editor[] activeEditors { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public bool hasComponentsWhichCannotBeMultiEdited { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public InspectorMode inspectorMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public bool isDirty { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public bool isLocked { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public static ActiveEditorTracker sharedTracker
        {
            get
            {
                ActiveEditorTracker sharedTracker = new ActiveEditorTracker();
                SetupSharedTracker(sharedTracker);
                return sharedTracker;
            }
        }
    }
}

