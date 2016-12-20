namespace UnityEditor.PlaymodeTestsRunner
{
    using System;

    internal class FilteringOptions
    {
        public string[] categories = null;
        public string nameFilter = null;
        public bool showFailed = true;
        public bool showIgnored = true;
        public bool showNotRunned = true;
        public bool showSucceeded = true;
    }
}

