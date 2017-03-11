namespace Unity.IL2CPP.Common
{
    using NiceIO;
    using System;

    internal static class Il2CppDependencies
    {
        private static readonly NPath _root;

        static Il2CppDependencies()
        {
            NPath path = CommonPaths.Il2CppRoot.ParentContaining("il2cpp-dependencies");
            if (path != null)
            {
                string[] append = new string[] { "il2cpp-dependencies" };
                _root = path.Combine(append);
            }
        }

        public static NPath MonoInstall(string installName)
        {
            string[] append = new string[] { installName, "builds", "monodistribution" };
            return _root.Combine(append);
        }

        internal static bool Available =>
            (_root != null);

        internal static NPath Root
        {
            get
            {
                if (_root == null)
                {
                    throw new InvalidOperationException("No il2cpp dependencies found");
                }
                return _root;
            }
        }
    }
}

