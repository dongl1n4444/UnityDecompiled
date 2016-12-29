namespace UnityEditorInternal
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class AddCurvesPopup : EditorWindow
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private static AnimationWindowSelection <selection>k__BackingField;
        private const float k_WindowPadding = 3f;
        private static OnNewCurveAdded NewCurveAddedCallback;
        private static AddCurvesPopup s_AddCurvesPopup;
        private static AddCurvesPopupHierarchy s_Hierarchy;
        private static long s_LastClosedTime;
        internal static IAnimationRecordingState s_State;
        private static Vector2 windowSize = new Vector2(240f, 250f);

        internal static void AddNewCurve(AddCurvesPopupPropertyNode node)
        {
            AnimationWindowUtility.CreateDefaultCurves(s_State, node.selectionItem, node.curveBindings);
            if (NewCurveAddedCallback != null)
            {
                NewCurveAddedCallback(node);
            }
        }

        private void Init(Rect buttonRect)
        {
            buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
            PopupLocationHelper.PopupLocation[] locationPriorityOrder = new PopupLocationHelper.PopupLocation[] { PopupLocationHelper.PopupLocation.Right };
            base.ShowAsDropDown(buttonRect, windowSize, locationPriorityOrder);
        }

        private void OnDisable()
        {
            s_LastClosedTime = DateTime.Now.Ticks / 0x2710L;
            s_AddCurvesPopup = null;
            s_Hierarchy = null;
        }

        internal void OnGUI()
        {
            if (Event.current.type != EventType.Layout)
            {
                if (s_Hierarchy == null)
                {
                    s_Hierarchy = new AddCurvesPopupHierarchy();
                }
                Rect position = new Rect(1f, 1f, windowSize.x - 3f, windowSize.y - 3f);
                GUI.Box(new Rect(0f, 0f, windowSize.x, windowSize.y), GUIContent.none, new GUIStyle("grey_border"));
                s_Hierarchy.OnGUI(position, this);
            }
        }

        internal static bool ShowAtPosition(Rect buttonRect, IAnimationRecordingState state, OnNewCurveAdded newCurveCallback)
        {
            long num = DateTime.Now.Ticks / 0x2710L;
            if (num >= (s_LastClosedTime + 50L))
            {
                Event.current.Use();
                if (s_AddCurvesPopup == null)
                {
                    s_AddCurvesPopup = ScriptableObject.CreateInstance<AddCurvesPopup>();
                }
                NewCurveAddedCallback = newCurveCallback;
                s_State = state;
                s_AddCurvesPopup.Init(buttonRect);
                return true;
            }
            return false;
        }

        internal static AnimationWindowSelection selection
        {
            [CompilerGenerated]
            get => 
                <selection>k__BackingField;
            [CompilerGenerated]
            set
            {
                <selection>k__BackingField = value;
            }
        }

        public delegate void OnNewCurveAdded(AddCurvesPopupPropertyNode node);
    }
}

