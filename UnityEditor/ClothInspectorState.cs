namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class ClothInspectorState : ScriptableSingleton<ClothInspectorState>
    {
        [SerializeField]
        public UnityEditor.ClothInspector.DrawMode DrawMode = UnityEditor.ClothInspector.DrawMode.MaxDistance;
        [SerializeField]
        public bool ManipulateBackfaces = false;
        [SerializeField]
        public float PaintCollisionSphereDistance = 0f;
        [SerializeField]
        public bool PaintCollisionSphereDistanceEnabled = false;
        [SerializeField]
        public float PaintMaxDistance = 0.2f;
        [SerializeField]
        public bool PaintMaxDistanceEnabled = true;
        [SerializeField]
        public UnityEditor.ClothInspector.ToolMode ToolMode = UnityEditor.ClothInspector.ToolMode.Select;
    }
}

