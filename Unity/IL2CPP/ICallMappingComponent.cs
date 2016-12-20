namespace Unity.IL2CPP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.IoCServices;

    internal class ICallMappingComponent : IIcallMappingService
    {
        private Dictionary<string, string> Map = new Dictionary<string, string>();

        public ICallMappingComponent()
        {
            string[] append = new string[] { "libil2cpp/libil2cpp.icalls" };
            this.ReadMap(CommonPaths.Il2CppRoot.Combine(append).ToString());
        }

        private void ReadMap(string path)
        {
            string[] strArray = File.ReadAllLines(path);
            foreach (string str in strArray)
            {
                if ((!str.StartsWith(";") && !str.StartsWith("#")) && !str.StartsWith("//"))
                {
                    char[] separator = new char[] { ' ' };
                    string[] strArray3 = str.Split(separator);
                    if (strArray3.Length == 2)
                    {
                        this.Map[strArray3[0]] = strArray3[1];
                    }
                }
            }
        }

        public string ResolveICallFunction(string icall)
        {
            if (this.Map.ContainsKey(icall))
            {
                return this.Map[icall];
            }
            return null;
        }
    }
}

