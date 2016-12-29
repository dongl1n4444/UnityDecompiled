namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class GUIViewDebuggerHelper
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void DebugWindow(GUIView view);
        internal static void GetClipInstructions(List<IMGUIClipInstruction> clipInstructions)
        {
            GetClipInstructionsInternal(clipInstructions);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void GetClipInstructionsInternal(object clipInstructions);
        internal static GUIContent GetContentFromInstruction(int instructionIndex) => 
            new GUIContent { 
                text = GetContentTextFromInstruction(instructionIndex),
                image = GetContentImageFromInstruction(instructionIndex)
            };

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Texture GetContentImageFromInstruction(int instructionIndex);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string GetContentTextFromInstruction(int instructionIndex);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int GetInstructionCount();
        internal static void GetLayoutInstructions(List<IMGUILayoutInstruction> layoutInstructions)
        {
            GetLayoutInstructionsInternal(layoutInstructions);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void GetLayoutInstructionsInternal(object layoutInstructions);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern StackFrame[] GetManagedStackTrace(int instructionIndex);
        public static Rect GetRectFromInstruction(int instructionIndex)
        {
            Rect rect;
            INTERNAL_CALL_GetRectFromInstruction(instructionIndex, out rect);
            return rect;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern GUIStyle GetStyleFromInstruction(int instructionIndex);
        internal static void GetUnifiedInstructions(List<IMGUIInstruction> layoutInstructions)
        {
            GetUnifiedInstructionsInternal(layoutInstructions);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void GetUnifiedInstructionsInternal(object instructions);
        internal static void GetViews(List<GUIView> views)
        {
            GetViewsInternal(views);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void GetViewsInternal(object views);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetRectFromInstruction(int instructionIndex, out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void StopDebugging();
    }
}

