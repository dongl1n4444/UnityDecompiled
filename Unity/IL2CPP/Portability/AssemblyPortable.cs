namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;

    public static class AssemblyPortable
    {
        public static Assembly GetAssemblyPortable(Type type)
        {
            return TypeExtensions.GetAssemblyPortable(type);
        }

        public static Assembly GetExecutingAssemblyPortable()
        {
            return Assembly.GetCallingAssembly();
        }

        public static Assembly LoadFilePortable(string path)
        {
            return Assembly.LoadFile(path);
        }

        public static Assembly LoadFromPortable(string assemblyFile)
        {
            return Assembly.LoadFrom(assemblyFile);
        }

        public static Assembly LoadPortable(string partialName)
        {
            return Assembly.Load(partialName);
        }
    }
}

