namespace UnityEditor
{
    using System;

    [Serializable]
    internal class TickStyle
    {
        public bool centerLabel = false;
        public int distFull = 80;
        public int distLabel = 50;
        public int distMin = 10;
        public EditorGUIUtility.SkinnedColor labelColor = new EditorGUIUtility.SkinnedColor(new Color(0f, 0f, 0f, 0.32f), new Color(0.8f, 0.8f, 0.8f, 0.32f));
        public bool stubs = false;
        public EditorGUIUtility.SkinnedColor tickColor = new EditorGUIUtility.SkinnedColor(new Color(0f, 0f, 0f, 0.2f), new Color(0.45f, 0.45f, 0.45f, 0.2f));
        public string unit = "";
    }
}

