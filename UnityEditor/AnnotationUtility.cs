namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;

    internal sealed class AnnotationUtility
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void DeletePreset(string presetName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Annotation[] GetAnnotations();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string GetNameOfCurrentSetup();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string[] GetPresetList();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Annotation[] GetRecentlyChangedAnnotations();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void LoadPreset(string presetName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void ResetToFactorySettings();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SavePreset(string presetName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetGizmoEnabled(int classID, string scriptClass, int gizmoEnabled);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetIconEnabled(int classID, string scriptClass, int iconEnabled);

        internal static float iconSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal static bool showGrid { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal static bool showSelectionOutline { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal static bool showSelectionWire { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal static bool use3dGizmos { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

