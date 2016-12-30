namespace UnityEditor
{
    using System;
    using UnityEngine.Networking;

    [CustomEditor(typeof(NetworkTransformVisualizer), true), CanEditMultipleObjects]
    public class NetworkTransformVisualizerEditor : NetworkBehaviourInspector
    {
        internal override bool hideScriptField =>
            true;
    }
}

