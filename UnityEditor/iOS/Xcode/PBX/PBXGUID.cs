namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Runtime.CompilerServices;

    internal class PBXGUID
    {
        [CompilerGenerated]
        private static GuidGenerator <>f__mg$cache0;
        private static GuidGenerator guidGenerator;

        static PBXGUID()
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new GuidGenerator(PBXGUID.DefaultGuidGenerator);
            }
            guidGenerator = <>f__mg$cache0;
        }

        internal static string DefaultGuidGenerator()
        {
            return Guid.NewGuid().ToString("N").Substring(8).ToUpper();
        }

        public static string Generate()
        {
            return guidGenerator();
        }

        internal static void SetGuidGenerator(GuidGenerator generator)
        {
            guidGenerator = generator;
        }

        internal delegate string GuidGenerator();
    }
}

