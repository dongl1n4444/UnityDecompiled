namespace Unity.IL2CPP.Common
{
    using Mono.Cecil;
    using Mono.Collections.Generic;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public static class CecilExtensions
    {
        [CompilerGenerated]
        private static Func<ModuleDefinition, IEnumerable<TypeDefinition>> <>f__am$cache0;

        public static IEnumerable<TypeDefinition> AllDefinedTypes(this AssemblyDefinition assemblyDefinition)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = m => m.AllDefinedTypes();
            }
            return assemblyDefinition.Modules.SelectMany<ModuleDefinition, TypeDefinition>(<>f__am$cache0);
        }

        [DebuggerHidden]
        public static IEnumerable<TypeDefinition> AllDefinedTypes(this ModuleDefinition moduleDefinition) => 
            new <AllDefinedTypes>c__Iterator0 { 
                moduleDefinition = moduleDefinition,
                $PC = -2
            };

        [DebuggerHidden]
        public static IEnumerable<TypeDefinition> AllDefinedTypes(this TypeDefinition typeDefinition) => 
            new <AllDefinedTypes>c__Iterator1 { 
                typeDefinition = typeDefinition,
                $PC = -2
            };

        [CompilerGenerated]
        private sealed class <AllDefinedTypes>c__Iterator0 : IEnumerable, IEnumerable<TypeDefinition>, IEnumerator, IDisposable, IEnumerator<TypeDefinition>
        {
            internal TypeDefinition $current;
            internal bool $disposing;
            internal Collection<TypeDefinition>.Enumerator $locvar0;
            internal IEnumerator<TypeDefinition> $locvar1;
            internal int $PC;
            internal TypeDefinition <definition>__2;
            internal TypeDefinition <typeDefinition>__1;
            internal ModuleDefinition moduleDefinition;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                    case 2:
                        try
                        {
                            switch (num)
                            {
                                case 2:
                                    try
                                    {
                                    }
                                    finally
                                    {
                                        if (this.$locvar1 != null)
                                        {
                                            this.$locvar1.Dispose();
                                        }
                                    }
                                    return;
                            }
                        }
                        finally
                        {
                            this.$locvar0.Dispose();
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        this.$locvar0 = this.moduleDefinition.Types.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                    case 2:
                        break;

                    default:
                        goto Label_014C;
                }
                try
                {
                    switch (num)
                    {
                        case 1:
                            this.$locvar1 = this.<typeDefinition>__1.AllDefinedTypes().GetEnumerator();
                            num = 0xfffffffd;
                            goto Label_00A5;

                        case 2:
                            goto Label_00A5;
                    }
                    while (this.$locvar0.MoveNext())
                    {
                        this.<typeDefinition>__1 = this.$locvar0.Current;
                        this.$current = this.<typeDefinition>__1;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        flag = true;
                        goto Label_014E;
                    Label_00A5:
                        try
                        {
                            while (this.$locvar1.MoveNext())
                            {
                                this.<definition>__2 = this.$locvar1.Current;
                                this.$current = this.<definition>__2;
                                if (!this.$disposing)
                                {
                                    this.$PC = 2;
                                }
                                flag = true;
                                goto Label_014E;
                            }
                        }
                        finally
                        {
                            if (!flag)
                            {
                            }
                            if (this.$locvar1 != null)
                            {
                                this.$locvar1.Dispose();
                            }
                        }
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    this.$locvar0.Dispose();
                }
                this.$PC = -1;
            Label_014C:
                return false;
            Label_014E:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<TypeDefinition> IEnumerable<TypeDefinition>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new CecilExtensions.<AllDefinedTypes>c__Iterator0 { moduleDefinition = this.moduleDefinition };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<Mono.Cecil.TypeDefinition>.GetEnumerator();

            TypeDefinition IEnumerator<TypeDefinition>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <AllDefinedTypes>c__Iterator1 : IEnumerable, IEnumerable<TypeDefinition>, IEnumerator, IDisposable, IEnumerator<TypeDefinition>
        {
            internal TypeDefinition $current;
            internal bool $disposing;
            internal Collection<TypeDefinition>.Enumerator $locvar0;
            internal IEnumerator<TypeDefinition> $locvar1;
            internal int $PC;
            internal TypeDefinition <definition>__2;
            internal TypeDefinition <nestedType>__1;
            internal TypeDefinition typeDefinition;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                    case 2:
                        try
                        {
                            switch (num)
                            {
                                case 2:
                                    try
                                    {
                                    }
                                    finally
                                    {
                                        if (this.$locvar1 != null)
                                        {
                                            this.$locvar1.Dispose();
                                        }
                                    }
                                    return;
                            }
                        }
                        finally
                        {
                            this.$locvar0.Dispose();
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        this.$locvar0 = this.typeDefinition.NestedTypes.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                    case 2:
                        break;

                    default:
                        goto Label_014C;
                }
                try
                {
                    switch (num)
                    {
                        case 1:
                            this.$locvar1 = this.<nestedType>__1.AllDefinedTypes().GetEnumerator();
                            num = 0xfffffffd;
                            goto Label_00A5;

                        case 2:
                            goto Label_00A5;
                    }
                    while (this.$locvar0.MoveNext())
                    {
                        this.<nestedType>__1 = this.$locvar0.Current;
                        this.$current = this.<nestedType>__1;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        flag = true;
                        goto Label_014E;
                    Label_00A5:
                        try
                        {
                            while (this.$locvar1.MoveNext())
                            {
                                this.<definition>__2 = this.$locvar1.Current;
                                this.$current = this.<definition>__2;
                                if (!this.$disposing)
                                {
                                    this.$PC = 2;
                                }
                                flag = true;
                                goto Label_014E;
                            }
                        }
                        finally
                        {
                            if (!flag)
                            {
                            }
                            if (this.$locvar1 != null)
                            {
                                this.$locvar1.Dispose();
                            }
                        }
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    this.$locvar0.Dispose();
                }
                this.$PC = -1;
            Label_014C:
                return false;
            Label_014E:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<TypeDefinition> IEnumerable<TypeDefinition>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new CecilExtensions.<AllDefinedTypes>c__Iterator1 { typeDefinition = this.typeDefinition };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<Mono.Cecil.TypeDefinition>.GetEnumerator();

            TypeDefinition IEnumerator<TypeDefinition>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

