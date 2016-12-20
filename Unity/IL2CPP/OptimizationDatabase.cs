namespace Unity.IL2CPP
{
    using System;
    using System.Collections.Generic;

    internal static class OptimizationDatabase
    {
        private static Dictionary<string, string[]> _disabledOptimizationMap;

        static OptimizationDatabase()
        {
            Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
            string[] textArray1 = new string[] { "IL2CPP_TARGET_XBOXONE" };
            dictionary.Add("System.Void Mono.Globalization.Unicode.MSCompatUnicodeTable::.cctor()", textArray1);
            _disabledOptimizationMap = dictionary;
        }

        public static string[] GetPlatformsWithDisabledOptimizations(string methodFullName)
        {
            string[] strArray = null;
            _disabledOptimizationMap.TryGetValue(methodFullName, out strArray);
            return strArray;
        }
    }
}

