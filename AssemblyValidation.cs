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
    private static Dictionary<RuntimePlatform, List<System.Type>> _rulesByPlatform;
    [CompilerGenerated]
    private static Func<IValidationRule, bool> <>f__am$cache0;
    [CompilerGenerated]
    private static Func<object, System.Type> <>f__am$cache1;
    [CompilerGenerated]
    private static Func<AssemblyValidationRule, int> <>f__am$cache2;
    [CompilerGenerated]
    private static Func<System.Type, bool> <>f__mg$cache0;

    internal static int CompareValidationRulesByPriority(System.Type a, System.Type b, RuntimePlatform platform)
    {
        int num = PriorityFor(a, platform);
        int num2 = PriorityFor(b, platform);
        if (num == num2)
        {
            return 0;
        }
        return ((num >= num2) ? 1 : -1);
    }

    private static ConstructorInfo ConstructorFor(System.Type type, IEnumerable<object> options)
    {
        if (<>f__am$cache1 == null)
        {
            <>f__am$cache1 = o => o.GetType();
        }
        System.Type[] types = Enumerable.Select<object, System.Type>(options, <>f__am$cache1).ToArray<System.Type>();
        return type.GetConstructor(types);
    }

    private static IValidationRule CreateValidationRuleWithOptions(System.Type type, params object[] options)
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

    private static bool IsValidationRule(System.Type type) => 
        ValidationRuleAttributesFor(type).Any<AssemblyValidationRule>();

    private static int PriorityFor(System.Type type, RuntimePlatform platform)
    {
        <PriorityFor>c__AnonStorey3 storey = new <PriorityFor>c__AnonStorey3 {
            platform = platform
        };
        if (<>f__am$cache2 == null)
        {
            <>f__am$cache2 = attr => attr.Priority;
        }
        return Enumerable.Select<AssemblyValidationRule, int>(Enumerable.Where<AssemblyValidationRule>(ValidationRuleAttributesFor(type), new Func<AssemblyValidationRule, bool>(storey.<>m__0)), <>f__am$cache2).FirstOrDefault<int>();
    }

    internal static void RegisterValidationRule(System.Type type)
    {
        foreach (AssemblyValidationRule rule in ValidationRuleAttributesFor(type))
        {
            RegisterValidationRuleForPlatform(rule.Platform, type);
        }
    }

    internal static void RegisterValidationRuleForPlatform(RuntimePlatform platform, System.Type type)
    {
        <RegisterValidationRuleForPlatform>c__AnonStorey2 storey = new <RegisterValidationRuleForPlatform>c__AnonStorey2 {
            platform = platform
        };
        if (!_rulesByPlatform.ContainsKey(storey.platform))
        {
            _rulesByPlatform[storey.platform] = new List<System.Type>();
        }
        if (_rulesByPlatform[storey.platform].IndexOf(type) == -1)
        {
            _rulesByPlatform[storey.platform].Add(type);
        }
        _rulesByPlatform[storey.platform].Sort(new Comparison<System.Type>(storey.<>m__0));
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
            strArray = userAssemblies.ToArray<string>();
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

    private static IEnumerable<AssemblyValidationRule> ValidationRuleAttributesFor(System.Type type) => 
        type.GetCustomAttributes(true).OfType<AssemblyValidationRule>();

    private static IEnumerable<IValidationRule> ValidationRulesFor(RuntimePlatform platform, params object[] options)
    {
        <ValidationRulesFor>c__AnonStorey1 storey = new <ValidationRulesFor>c__AnonStorey1 {
            options = options
        };
        if (<>f__am$cache0 == null)
        {
            <>f__am$cache0 = v => v != null;
        }
        return Enumerable.Where<IValidationRule>(Enumerable.Select<System.Type, IValidationRule>(ValidationRuleTypesFor(platform), new Func<System.Type, IValidationRule>(storey.<>m__0)), <>f__am$cache0);
    }

    [DebuggerHidden]
    private static IEnumerable<System.Type> ValidationRuleTypesFor(RuntimePlatform platform) => 
        new <ValidationRuleTypesFor>c__Iterator0 { 
            platform = platform,
            $PC = -2
        };

    private static void WarmUpRulesCache()
    {
        if (_rulesByPlatform == null)
        {
            _rulesByPlatform = new Dictionary<RuntimePlatform, List<System.Type>>();
            Assembly assembly = typeof(AssemblyValidation).Assembly;
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Func<System.Type, bool>(AssemblyValidation.IsValidationRule);
            }
            foreach (System.Type type in Enumerable.Where<System.Type>(assembly.GetTypes(), <>f__mg$cache0))
            {
                RegisterValidationRule(type);
            }
        }
    }

    [CompilerGenerated]
    private sealed class <PriorityFor>c__AnonStorey3
    {
        internal RuntimePlatform platform;

        internal bool <>m__0(AssemblyValidationRule attr) => 
            (attr.Platform == this.platform);
    }

    [CompilerGenerated]
    private sealed class <RegisterValidationRuleForPlatform>c__AnonStorey2
    {
        internal RuntimePlatform platform;

        internal int <>m__0(System.Type a, System.Type b) => 
            AssemblyValidation.CompareValidationRulesByPriority(a, b, this.platform);
    }

    [CompilerGenerated]
    private sealed class <ValidationRulesFor>c__AnonStorey1
    {
        internal object[] options;

        internal IValidationRule <>m__0(System.Type t) => 
            AssemblyValidation.CreateValidationRuleWithOptions(t, this.options);
    }

    [CompilerGenerated]
    private sealed class <ValidationRuleTypesFor>c__Iterator0 : IEnumerable, IEnumerable<System.Type>, IEnumerator, IDisposable, IEnumerator<System.Type>
    {
        internal System.Type $current;
        internal bool $disposing;
        internal List<System.Type>.Enumerator $locvar0;
        internal int $PC;
        internal System.Type <validationType>__0;
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
        IEnumerator<System.Type> IEnumerable<System.Type>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new AssemblyValidation.<ValidationRuleTypesFor>c__Iterator0 { platform = this.platform };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator() => 
            this.System.Collections.Generic.IEnumerable<System.Type>.GetEnumerator();

        System.Type IEnumerator<System.Type>.Current =>
            this.$current;

        object IEnumerator.Current =>
            this.$current;
    }
}

