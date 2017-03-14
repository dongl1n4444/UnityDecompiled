namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public sealed class MethodTables
    {
        [CompilerGenerated]
        private static Func<MethodReference, bool> <>f__am$cache0;
        [Inject]
        public static IGenericSharingAnalysisService GenericSharingAnalysis;
        public Dictionary<string, int> MethodPointers;
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static IRuntimeInvokerCollectorAdderService RuntimeInvokerCollectorAdder;
        private HashSet<MethodReference> usedMethodReferences;

        internal static MethodTables CollectMethodTables(IEnumerable<MethodReference> genericMethods)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            HashSet<MethodReference> set = new HashSet<MethodReference>();
            dictionary.Add(Naming.Null, 0);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = m => (!m.HasGenericParameters && !m.DeclaringType.HasGenericParameters) && !m.ContainsGenericParameters();
            }
            foreach (MethodReference reference in genericMethods.Where<MethodReference>(<>f__am$cache0).ToArray<MethodReference>())
            {
                string key = MethodPointerFor(reference);
                if (!dictionary.ContainsKey(key))
                {
                    dictionary.Add(key, dictionary.Count);
                    set.Add(reference);
                }
                RuntimeInvokerCollectorAdder.Add(reference);
            }
            return new MethodTables { 
                MethodPointers = dictionary,
                usedMethodReferences = set
            };
        }

        internal bool IsMethodReferenceUsed(MethodReference method) => 
            this.usedMethodReferences.Contains(method);

        internal static string MethodPointerFor(MethodReference method)
        {
            string str = MethodPointerNameFor(method);
            if (str != Naming.Null)
            {
                str = "(Il2CppMethodPointer)&" + str;
            }
            return str;
        }

        internal static string MethodPointerNameFor(MethodReference method)
        {
            if (MethodWriter.IsGetOrSetGenericValueImplOnArray(method))
            {
                return Naming.Null;
            }
            if (GenericsUtilities.IsGenericInstanceOfCompareExchange(method))
            {
                return Naming.Null;
            }
            if (GenericsUtilities.IsGenericInstanceOfExchange(method))
            {
                return Naming.Null;
            }
            if (!MethodWriter.MethodCanBeDirectlyCalled(method.Resolve()))
            {
                return Naming.Null;
            }
            if (GenericSharingAnalysis.CanShareMethod(method))
            {
                method = GenericSharingAnalysis.GetSharedMethod(method);
                if (method.HasThis && method.DeclaringType.IsValueType())
                {
                    return Naming.ForMethodAdjustorThunk(method);
                }
                return (Naming.ForMethod(method) + "_gshared");
            }
            if (method.HasThis && method.DeclaringType.IsValueType())
            {
                return Naming.ForMethodAdjustorThunk(method);
            }
            return Naming.ForMethod(method);
        }
    }
}

