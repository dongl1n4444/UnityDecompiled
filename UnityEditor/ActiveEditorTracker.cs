namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [Serializable]
    public sealed class ActiveEditorTracker
    {
        private MonoReloadableIntPtrClear m_Property;

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public extern ActiveEditorTracker();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void ClearDirty();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Destroy();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
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

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void ForceRebuild();
        public override int GetHashCode()
        {
            return this.m_Property.m_IntPtr.GetHashCode();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int GetVisible(int index);
        public static bool HasCustomEditor(Object obj)
        {
            return (CustomEditorAttributes.FindCustomEditorType(obj, false) != null);
        }

        [Obsolete("Use Editor.CreateEditor instead")]
        public static Editor MakeCustomEditor(Object obj)
        {
            return Editor.CreateEditor(obj);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void RebuildIfNecessary();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetupSharedTracker(ActiveEditorTracker sharedTracker);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetVisible(int index, int visible);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void VerifyModifiedMonoBehaviours();

        public Editor[] activeEditors { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public bool hasComponentsWhichCannotBeMultiEdited { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public InspectorMode inspectorMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public bool isDirty { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public bool isLocked { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

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

