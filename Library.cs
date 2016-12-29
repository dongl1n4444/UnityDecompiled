using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

internal class Library
{
    private readonly List<string> _archs = new List<string>();
    private readonly string _directory;
    public const string AnyCpuTag = "AnyCPU";
    public readonly string Name;
    public bool Native;
    public const string PlayerTag = "Player";
    public readonly bool Process;
    public bool WinMd;

    public Library(string name, bool native, bool winmd, bool process, string directory = null)
    {
        this.Name = name;
        this.Native = native;
        this.WinMd = winmd;
        this.Process = (process && !native) && !winmd;
        this._directory = directory;
    }

    public string GetArchReferencePath(string arch)
    {
        string reference = this.Reference;
        if (!string.IsNullOrEmpty(this._directory))
        {
            reference = Path.Combine(this._directory, reference);
            if (!IsAnyCpuTag(arch))
            {
                reference = reference.Replace("$(PlatformShortName)", arch);
            }
        }
        return reference;
    }

    public static bool IsAnyCpuTag(string arch) => 
        IsTag(arch, "AnyCPU");

    public static bool IsPlayerTag(string arch) => 
        IsTag(arch, "Player");

    private static bool IsTag(string arch, string tag) => 
        (string.IsNullOrEmpty(arch) || string.Equals(arch, tag, StringComparison.InvariantCultureIgnoreCase));

    public void RegisterArch(string arch)
    {
        if (IsAnyCpuTag(arch))
        {
            arch = "AnyCPU";
        }
        if (this._archs.Contains(arch))
        {
            throw new Exception($"{this.Name} already contains {arch} architecture.");
        }
        this._archs.Add(arch);
    }

    public bool AnyCpu
    {
        get
        {
            if (this._archs.Count != 1)
            {
                return false;
            }
            return IsAnyCpuTag(this._archs[0]);
        }
    }

    public IEnumerable<string> Archs =>
        this._archs;

    public IEnumerable<string> Files =>
        new <>c__Iterator0 { 
            $this=this,
            $PC=-2
        };

    public bool Player
    {
        get
        {
            if (this._archs.Count != 1)
            {
                return false;
            }
            return IsPlayerTag(this._archs[0]);
        }
    }

    public string Reference =>
        (this.Name + (!this.WinMd ? ".dll" : ".winmd"));

    public string ReferencePath
    {
        get
        {
            string reference = this.Reference;
            if (string.IsNullOrEmpty(this._directory))
            {
                return reference;
            }
            return Path.Combine(this._directory, reference);
        }
    }

    [CompilerGenerated]
    private sealed class <>c__Iterator0 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
    {
        internal string $current;
        internal bool $disposing;
        internal int $PC;
        internal Library $this;

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
                    this.$current = this.$this.Reference;
                    if (!this.$disposing)
                    {
                        this.$PC = 1;
                    }
                    goto Label_00A3;

                case 1:
                    if (!this.$this.Native || !this.$this.WinMd)
                    {
                        break;
                    }
                    this.$current = this.$this.Name + ".dll";
                    if (!this.$disposing)
                    {
                        this.$PC = 2;
                    }
                    goto Label_00A3;

                case 2:
                    break;

                default:
                    goto Label_00A1;
            }
            this.$PC = -1;
        Label_00A1:
            return false;
        Label_00A3:
            return true;
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
            return new Library.<>c__Iterator0 { $this = this.$this };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator() => 
            this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();

        string IEnumerator<string>.Current =>
            this.$current;

        object IEnumerator.Current =>
            this.$current;
    }
}

