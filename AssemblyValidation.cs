using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

internal class AssemblyValidation
{
    private static Dictionary<RuntimePlatform, List<Type>> _rulesByPlatform;
    [CompilerGenerated]
    private static Func<IValidationRule, bool> <>f__am$cache0;
    [CompilerGenerated]
    private static Func<object, Type> <>f__am$cache1;
    [CompilerGenerated]
    private static Func<AssemblyValidationRule, int> <>f__am$cache2;
    [CompilerGenerated]
    private static Func<Type, bool> <>f__mg$cache0;

    internal static int CompareValidationRulesByPriority(Type a, Type b, RuntimePlatform platform)
    {
        int num = PriorityFor(a, platform);
        int num2 = PriorityFor(b, platform);
        if (num == num2)
        {
            return 0;
        }
        return ((num >= num2) ? 1 : -1);
    }

    private static ConstructorInfo ConstructorFor(Type type, IEnumerable<object> options)
    {
        if (<>f__am$cache1 == null)
        {
            <>f__am$cache1 = new Func<object, Type>(null, (IntPtr) <ConstructorFor>m__1);
        }
        Type[] types = Enumerable.ToArray<Type>(Enumerable.Select<object, Type>(options, <>f__am$cache1));
        return type.GetConstructor(types);
    }

    private static IValidationRule CreateValidationRuleWithOptions(Type type, params object[] options)
    {
        List<object> list = new List<object>(options);
        while (true)
        {
            object[] objArray = list.ToArray();
            ConstructorInfo info = ConstructorFor(type, objArray);
            if (info != null)
            {
                return (IValidationRule) info.Invoke(objArray);
            }
            if (list.Count == 0)
            {
                return null;
            }
            list.RemoveAt(list.Count - 1);
        }
    }

    private static bool IsValidationRule(Type type)
    {
        return Enumerable.Any<AssemblyValidationRule>(ValidationRuleAttributesFor(type));
    }

    private static int PriorityFor(Type type, RuntimePlatform platform)
    {
        <PriorityFor>c__AnonStorey3 storey = new <PriorityFor>c__AnonStorey3 {
            platform = platform
        };
        if (<>f__am$cache2 == null)
        {
            <>f__am$cache2 = new Func<AssemblyValidationRule, int>(null, (IntPtr) <PriorityFor>m__2);
        }
        return Enumerable.FirstOrDefault<int>(Enumerable.Select<AssemblyValidationRule, int>(Enumerable.Where<AssemblyValidationRule>(ValidationRuleAttributesFor(type), new Func<AssemblyValidationRule, bool>(storey, (IntPtr) this.<>m__0)), <>f__am$cache2));
    }

    internal static void RegisterValidationRule(Type type)
    {
        foreach (AssemblyValidationRule rule in ValidationRuleAttributesFor(type))
        {
            RegisterValidationRuleForPlatform(rule.Platform, type);
        }
    }

    internal static void RegisterValidationRuleForPlatform(RuntimePlatform platform, Type type)
    {
        <RegisterValidationRuleForPlatform>c__AnonStorey2 storey = new <RegisterValidationRuleForPlatform>c__AnonStorey2 {
            platform = platform
        };
        if (!_rulesByPlatform.ContainsKey(storey.platform))
        {
            _rulesByPlatform[storey.platform] = new List<Type>();
        }
        if (_rulesByPlatform[storey.platform].IndexOf(type) == -1)
        {
            _rulesByPlatform[storey.platform].Add(type);
        }
        _rulesByPlatform[storey.platform].Sort(new Comparison<Type>(storey.<>m__0));
    }

    public static ValidationResult Validate(RuntimePlatform platform, IEnumerable<string> userAssemblies, params object[] options)
    {
        string[] strArray;
        WarmUpRulesCache();
        string[] textArray1 = userAssemblies as string[];
        if (textArray1 != null)
        {
            strArray = textArray1;
        }
        else
        {
            strArray = Enumerable.ToArray<string>(userAssemblies);
        }
        if (strArray.Length != 0)
        {
            foreach (IValidationRule rule in ValidationRulesFor(platform, options))
            {
                ValidationResult result = rule.Validate(strArray, options);
                if (!result.Success)
                {
                    return result;
                }
            }
        }
        return new ValidationResult { Success = true };
    }

    private static IEnumerable<AssemblyValidationRule> ValidationRuleAttributesFor(Type type)
    {
        return Enumerable.OfType<AssemblyValidationRule>(type.GetCustomAttributes(true));
    }

    private static IEnumerable<IValidationRule> ValidationRulesFor(RuntimePlatform platform, params object[] options)
    {
        <ValidationRulesFor>c__AnonStorey1 storey = new <ValidationRulesFor>c__AnonStorey1 {
            options = options
        };
        if (<>f__am$cache0 == null)
        {
            <>f__am$cache0 = new Func<IValidationRule, bool>(null, (IntPtr) <ValidationRulesFor>m__0);
        }
        return Enumerable.Where<IValidationRule>(Enumerable.Select<Type, IValidationRule>(ValidationRuleTypesFor(platform), new Func<Type, IValidationRule>(storey, (IntPtr) this.<>m__0)), <>f__am$cache0);
    }

    [DebuggerHidden]
    private static IEnumerable<Type> ValidationRuleTypesFor(RuntimePlatform platform)
    {
        return new <ValidationRuleTypesFor>c__Iterator0 { 
            platform = platform,
            $PC = -2
        };
    }

    private static void WarmUpRulesCache()
    {
        if (_rulesByPlatform == null)
        {
            _rulesByPlatform = new Dictionary<RuntimePlatform, List<Type>>();
            Assembly assembly = typeof(AssemblyValidation).Assembly;
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Func<Type, bool>(null, (IntPtr) IsValidationRule);
            }
            foreach (Type type in Enumerable.Where<Type>(assembly.GetTypes(), <>f__mg$cache0))
            {
                RegisterValidationRule(type);
            }
        }
    }

    [CompilerGenerated]
    private sealed class <PriorityFor>c__AnonStorey3
    {
        internal RuntimePlatform platform;

        internal bool <>m__0(AssemblyValidationRule attr)
        {
            return (attr.Platform == this.platform);
        }
    }

    [CompilerGenerated]
    private sealed class <RegisterValidationRuleForPlatform>c__AnonStorey2
    {
        internal RuntimePlatform platform;

        internal int <>m__0(Type a, Type b)
        {
            return AssemblyValidation.CompareValidationRulesByPriority(a, b, this.platform);
        }
    }

    [CompilerGenerated]
    private sealed class <ValidationRulesFor>c__AnonStorey1
    {
        internal object[] options;

        internal IValidationRule <>m__0(Type t)
        {
            return AssemblyValidation.CreateValidationRuleWithOptions(t, this.options);
        }
    }

    [CompilerGenerated]
    private sealed class <ValidationRuleTypesFor>c__Iterator0 : IEnumerable, IEnumerable<Type>, IEnumerator, IDisposable, IEnumerator<Type>
    {
        internal Type $current;
        internal bool $disposing;
        internal List<Type>.Enumerator $locvar0;
        internal int $PC;
        internal Type <validationType>__0;
        internal RuntimePlatform platform;

        [DebuggerHidden]
        public void Dispose()
        {
            uint num = (uint) this.$PC;
            this.$disposing = true;
            this.$PC = -1;
            switch (num)
            {
                case 1:
                    try
                    {
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
                    if (AssemblyValidation._rulesByPlatform.ContainsKey(this.platform))
                    {
                        this.$locvar0 = AssemblyValidation._rulesByPlatform[this.platform].GetEnumerator();
                        num = 0xfffffffd;
                        break;
                    }
                    goto Label_00D3;

                case 1:
                    break;

                default:
                    goto Label_00D3;
            }
            try
            {
                while (this.$locvar0.MoveNext())
                {
                    this.<validationType>__0 = this.$locvar0.Current;
                    this.$current = this.<validationType>__0;
                    if (!this.$disposing)
                    {
                        this.$PC = 1;
                    }
                    flag = true;
                    return true;
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
        Label_00D3:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<Type> IEnumerable<Type>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new AssemblyValidation.<ValidationRuleTypesFor>c__Iterator0 { platform = this.platform };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<System.Type>.GetEnumerator();
        }

        Type IEnumerator<Type>.Current
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

