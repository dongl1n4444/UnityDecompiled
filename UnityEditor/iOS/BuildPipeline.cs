namespace UnityEditor.iOS
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public static class BuildPipeline
    {
        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public static  event OnDemandTagsCollectorDelegate collectInitialInstallTags;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public static  event ResourcesCollectorDelegate collectResources;

        internal static List<string> CollectInitialInstallTags()
        {
            List<string> list = new List<string>();
            if (collectInitialInstallTags != null)
            {
                foreach (OnDemandTagsCollectorDelegate delegate2 in collectInitialInstallTags.GetInvocationList())
                {
                    list.AddRange(delegate2());
                }
            }
            return list;
        }

        internal static List<Resource> CollectResources()
        {
            List<Resource> list = new List<Resource>();
            if (collectResources != null)
            {
                foreach (ResourcesCollectorDelegate delegate2 in collectResources.GetInvocationList())
                {
                    list.AddRange(delegate2());
                }
            }
            return list;
        }

        public delegate string[] OnDemandTagsCollectorDelegate();

        public delegate Resource[] ResourcesCollectorDelegate();
    }
}

