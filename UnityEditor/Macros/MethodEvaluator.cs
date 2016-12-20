namespace UnityEditor.Macros
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Serialization.Formatters.Binary;

    public static class MethodEvaluator
    {
        private static readonly BinaryFormatter s_Formatter;

        static MethodEvaluator()
        {
            BinaryFormatter formatter = new BinaryFormatter {
                AssemblyFormat = FormatterAssemblyStyle.Simple
            };
            s_Formatter = formatter;
        }

        [CompilerGenerated]
        private static string <ToCommaSeparatedString`1>m__0<T>(T o)
        {
            return o.ToString();
        }

        public static object Eval(string assemblyFile, string typeName, string methodName, Type[] paramTypes, object[] args)
        {
            object obj2;
            AssemblyResolver resolver = new AssemblyResolver(Path.GetDirectoryName(assemblyFile));
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(resolver.AssemblyResolve);
            try
            {
                Assembly assembly = Assembly.LoadFrom(assemblyFile);
                MethodInfo info = assembly.GetType(typeName, true).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, paramTypes, null);
                if (info == null)
                {
                    throw new ArgumentException(string.Format("Method {0}.{1}({2}) not found in assembly {3}!", new object[] { typeName, methodName, ToCommaSeparatedString<Type>(paramTypes), assembly.FullName }));
                }
                obj2 = info.Invoke(null, args);
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(resolver.AssemblyResolve);
            }
            return obj2;
        }

        private static object ExecuteCode(Type target, MethodInfo method, object[] args)
        {
            return method.Invoke(!method.IsStatic ? GetActor(target) : null, args);
        }

        public static object ExecuteExternalCode(string parcel)
        {
            object obj2;
            using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(parcel)))
            {
                string str = (string) s_Formatter.Deserialize(stream);
                if (str != "com.unity3d.automation")
                {
                    throw new Exception("Invalid parcel for external code execution.");
                }
                string path = (string) s_Formatter.Deserialize(stream);
                AssemblyResolver resolver = new AssemblyResolver(Path.GetDirectoryName(path));
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(resolver.AssemblyResolve);
                Assembly assembly = Assembly.LoadFrom(path);
                try
                {
                    Type target = (Type) s_Formatter.Deserialize(stream);
                    string name = (string) s_Formatter.Deserialize(stream);
                    Type[] types = (Type[]) s_Formatter.Deserialize(stream);
                    MethodInfo method = target.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, types, null);
                    if (method == null)
                    {
                        throw new Exception(string.Format("Could not find method {0}.{1} in assembly {2} located in {3}.", new object[] { target.FullName, name, assembly.GetName().Name, path }));
                    }
                    object[] args = (object[]) s_Formatter.Deserialize(stream);
                    obj2 = ExecuteCode(target, method, args);
                }
                finally
                {
                    AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(resolver.AssemblyResolve);
                }
            }
            return obj2;
        }

        private static object GetActor(Type type)
        {
            ConstructorInfo constructor = type.GetConstructor(new Type[0]);
            return ((constructor == null) ? null : constructor.Invoke(new object[0]));
        }

        private static string ToCommaSeparatedString<T>(IEnumerable<T> items)
        {
            return string.Join(", ", Enumerable.ToArray<string>(Enumerable.Select<T, string>(items, new Func<T, string>(null, (IntPtr) <ToCommaSeparatedString`1>m__0<T>))));
        }

        private class AssemblyResolver
        {
            private readonly string m_AssemblyDirectory;

            public AssemblyResolver(string assemblyDirectory)
            {
                this.m_AssemblyDirectory = assemblyDirectory;
            }

            public Assembly AssemblyResolve(object sender, ResolveEventArgs args)
            {
                char[] separator = new char[] { ',' };
                string path = Path.Combine(this.m_AssemblyDirectory, args.Name.Split(separator)[0] + ".dll");
                if (File.Exists(path))
                {
                    return Assembly.LoadFrom(path);
                }
                return null;
            }
        }
    }
}

