namespace Unity.IL2CPP.Common
{
    using NiceIO;
    using System;
    using System.IO;
    using Unity.IL2CPP.Portability;

    public class CommonPaths
    {
        private static NPath _il2cppRoot;

        public static string Il2CppPath(string path)
        {
            string[] append = new string[] { path };
            return Il2CppRoot.Combine(append).ToString();
        }

        public static NPath Il2CppBuild
        {
            get
            {
                string[] append = new string[] { "build" };
                return Il2CppRoot.Combine(append);
            }
        }

        public static NPath Il2CppDependencies
        {
            get
            {
                NPath path = Il2CppRoot.ParentContaining("il2cpp-dependencies");
                if (path == null)
                {
                    string[] textArray1 = new string[] { "il2cpp-dependencies" };
                    throw new DirectoryNotFoundException(Il2CppRoot.Combine(textArray1).ToString());
                }
                string[] append = new string[] { "il2cpp-dependencies" };
                return path.Combine(append);
            }
        }

        public static bool Il2CppDependenciesAvailable =>
            (Il2CppRoot.ParentContaining("il2cpp-dependencies") != null);

        public static NPath Il2CppRoot
        {
            get
            {
                if (_il2cppRoot == null)
                {
                    Uri uri = new Uri(typeof(CommonPaths).GetAssemblyPortable().GetCodeBasePortable());
                    string localPath = uri.LocalPath;
                    if (!string.IsNullOrEmpty(uri.Fragment))
                    {
                        localPath = localPath + Uri.UnescapeDataString(uri.Fragment);
                    }
                    _il2cppRoot = new NPath(localPath).ParentContaining("il2cpp_root");
                    if (_il2cppRoot == null)
                    {
                        NPath path = new NPath(localPath).ParentContaining("build.pl");
                        if (path != null)
                        {
                            string[] append = new string[] { "Tools", "il2cpp", "il2cpp" };
                            _il2cppRoot = path.Combine(append);
                            if (!_il2cppRoot.Exists(""))
                            {
                                string[] textArray2 = new string[] { "External", "il2cpp", "il2cpp" };
                                _il2cppRoot = path.Combine(textArray2);
                            }
                        }
                    }
                }
                return _il2cppRoot;
            }
        }
    }
}

