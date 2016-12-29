namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class AnimationWindowManipulator
    {
        [CompilerGenerated]
        private static OnStartDragDelegate <>f__am$cache0;
        [CompilerGenerated]
        private static OnDragDelegate <>f__am$cache1;
        [CompilerGenerated]
        private static OnEndDragDelegate <>f__am$cache2;
        public int controlID;
        public OnDragDelegate onDrag;
        public OnEndDragDelegate onEndDrag;
        public OnStartDragDelegate onStartDrag;
        public Rect rect;

        public AnimationWindowManipulator()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new OnStartDragDelegate(AnimationWindowManipulator.<AnimationWindowManipulator>m__0);
            }
            this.onStartDrag = (OnStartDragDelegate) Delegate.Combine(this.onStartDrag, <>f__am$cache0);
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new OnDragDelegate(AnimationWindowManipulator.<AnimationWindowManipulator>m__1);
            }
            this.onDrag = (OnDragDelegate) Delegate.Combine(this.onDrag, <>f__am$cache1);
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new OnEndDragDelegate(AnimationWindowManipulator.<AnimationWindowManipulator>m__2);
            }
            this.onEndDrag = (OnEndDragDelegate) Delegate.Combine(this.onEndDrag, <>f__am$cache2);
        }

        [CompilerGenerated]
        private static bool <AnimationWindowManipulator>m__0(AnimationWindowManipulator manipulator, Event evt) => 
            false;

        [CompilerGenerated]
        private static bool <AnimationWindowManipulator>m__1(AnimationWindowManipulator manipulator, Event evt) => 
            false;

        [CompilerGenerated]
        private static bool <AnimationWindowManipulator>m__2(AnimationWindowManipulator manipulator, Event evt) => 
            false;

        public virtual void HandleEvents()
        {
            this.controlID = GUIUtility.GetControlID(FocusType.Passive);
            Event current = Event.current;
            EventType typeForControl = current.GetTypeForControl(this.controlID);
            bool flag = false;
            switch (typeForControl)
            {
                case EventType.MouseDown:
                    if (current.button == 0)
                    {
                        flag = this.onStartDrag(this, current);
                        if (flag)
                        {
                            GUIUtility.hotControl = this.controlID;
                        }
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == this.controlID)
                    {
                        flag = this.onDrag(this, current);
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == this.controlID)
                    {
                        flag = this.onEndDrag(this, current);
                        GUIUtility.hotControl = 0;
                    }
                    break;
            }
            if (flag)
            {
                current.Use();
            }
        }

        public virtual void IgnoreEvents()
        {
            GUIUtility.GetControlID(FocusType.Passive);
        }

        public delegate bool OnDragDelegate(AnimationWindowManipulator manipulator, Event evt);

        public delegate bool OnEndDragDelegate(AnimationWindowManipulator manipulator, Event evt);

        public delegate bool OnStartDragDelegate(AnimationWindowManipulator manipulator, Event evt);
    }
}

