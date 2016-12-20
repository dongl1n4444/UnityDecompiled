namespace UnityEditor.Android.Il2Cpp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor.Android;
    using UnityEditor.Utils;
    using UnityEditorInternal;
    using UnityEngine;

    public class AndroidIl2CppNativeCodeBuilder : Il2CppNativeCodeBuilder
    {
        public const string AndroidNdkVersion = "r10e";
        private AndroidTargetDeviceType m_DeviceType;
        private string m_NdkRootDir = AndroidNDKTools.GetInstance().NDKRootDir;

        public AndroidIl2CppNativeCodeBuilder(AndroidTargetDeviceType deviceType)
        {
            this.m_DeviceType = deviceType;
        }

        public override string ConvertOutputFileToFullPath(string outputFileRelativePath)
        {
            string[] components = new string[] { Directory.GetCurrentDirectory(), Path.GetDirectoryName(outputFileRelativePath), this.m_DeviceType.ABI };
            string path = Paths.Combine(components);
            Directory.CreateDirectory(path);
            return Path.Combine(path, Path.GetFileName(outputFileRelativePath));
        }

        public override IEnumerable<string> AdditionalIl2CPPArguments
        {
            get
            {
                return new <>c__Iterator0 { 
                    $this = this,
                    $PC = -2
                };
            }
        }

        public static string AndroidNdkVersionString
        {
            get
            {
                string str = "r10e";
                if (Application.platform == RuntimePlatform.LinuxEditor)
                {
                    str = str + "-rc4";
                }
                if ((Application.platform == RuntimePlatform.WindowsEditor) && !AndroidNdkRoot.Is64BitWindows())
                {
                    return str;
                }
                return (str + " (64-bit)");
            }
        }

        public override string CacheDirectory
        {
            get
            {
                string[] components = new string[] { Path.GetFullPath(Application.dataPath), "..", "Library", "il2cpp_android_" + this.m_DeviceType.ABI };
                return Paths.Combine(components);
            }
        }

        public override string CompilerArchitecture
        {
            get
            {
                return this.m_DeviceType.Architecture;
            }
        }

        public override string CompilerPlatform
        {
            get
            {
                return "Android";
            }
        }

        [CompilerGenerated]
        private sealed class <>c__Iterator0 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal int $PC;
            internal AndroidIl2CppNativeCodeBuilder $this;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = string.Format("--tool-chain-path=\"{0}\"", this.$this.m_NdkRootDir);
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        return true;

                    case 1:
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new AndroidIl2CppNativeCodeBuilder.<>c__Iterator0 { $this = this.$this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();
            }

            string IEnumerator<string>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

