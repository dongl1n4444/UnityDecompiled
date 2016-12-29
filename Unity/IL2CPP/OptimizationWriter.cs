namespace Unity.IL2CPP
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal class OptimizationWriter : IDisposable
    {
        private string[] _platformsWithOptimizationsDisabled;
        private CppCodeWriter _writer;
        [CompilerGenerated]
        private static Func<string, string, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, string, string> <>f__am$cache1;

        public OptimizationWriter(CppCodeWriter writer, string methodFullName)
        {
            this._platformsWithOptimizationsDisabled = OptimizationDatabase.GetPlatformsWithDisabledOptimizations(methodFullName);
            if (this._platformsWithOptimizationsDisabled != null)
            {
                this._writer = writer;
                object[] args = new object[1];
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<string, string, string>(null, (IntPtr) <OptimizationWriter>m__0);
                }
                args[0] = this._platformsWithOptimizationsDisabled.Aggregate<string>(<>f__am$cache0);
                this._writer.WriteLine("#if {0}", args);
                this._writer.WriteLine("IL2CPP_DISABLE_OPTIMIZATIONS");
                this._writer.WriteLine("#endif");
            }
        }

        [CompilerGenerated]
        private static string <OptimizationWriter>m__0(string x, string y) => 
            (x + " || " + y);

        public void Dispose()
        {
            if (this._platformsWithOptimizationsDisabled != null)
            {
                object[] args = new object[1];
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = new Func<string, string, string>(null, (IntPtr) <Dispose>m__1);
                }
                args[0] = this._platformsWithOptimizationsDisabled.Aggregate<string>(<>f__am$cache1);
                this._writer.WriteLine("#if {0}", args);
                this._writer.WriteLine("IL2CPP_ENABLE_OPTIMIZATIONS");
                this._writer.WriteLine("#endif");
            }
        }
    }
}

