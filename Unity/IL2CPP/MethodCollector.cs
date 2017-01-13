namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.Portability;

    public class MethodCollector : IMethodCollector, IMethodCollectorResults
    {
        private readonly Dictionary<TypeDefinition, int> _ccwMarshalingFunctions = new Dictionary<TypeDefinition, int>();
        private bool _complete;
        private readonly Dictionary<MethodReference, int> _delegateWrappersManagedToNative = new Dictionary<MethodReference, int>();
        private readonly Dictionary<MethodReference, int> _methods = new Dictionary<MethodReference, int>(new MethodReferenceComparer());
        private readonly Dictionary<MethodReference, int> _reversePInvokeWrappers = new Dictionary<MethodReference, int>();
        private readonly Dictionary<TypeDefinition, int> _typeMarshalingFunctions = new Dictionary<TypeDefinition, int>();
        [CompilerGenerated]
        private static Func<KeyValuePair<MethodReference, int>, int> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<KeyValuePair<MethodReference, int>, MethodReference> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<KeyValuePair<MethodReference, int>, int> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<KeyValuePair<MethodReference, int>, MethodReference> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<KeyValuePair<MethodReference, int>, int> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<KeyValuePair<MethodReference, int>, MethodReference> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeDefinition, int>, int> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeDefinition, int>, TypeDefinition> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeDefinition, int>, int> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeDefinition, int>, TypeDefinition> <>f__am$cache9;

        [CompilerGenerated]
        private static int <GetCCWMarshalingFunctions>m__8(KeyValuePair<TypeDefinition, int> kvp) => 
            kvp.Value;

        [CompilerGenerated]
        private static TypeDefinition <GetCCWMarshalingFunctions>m__9(KeyValuePair<TypeDefinition, int> kvp) => 
            kvp.Key;

        [CompilerGenerated]
        private static int <GetMethods>m__0(KeyValuePair<MethodReference, int> kvp) => 
            kvp.Value;

        [CompilerGenerated]
        private static MethodReference <GetMethods>m__1(KeyValuePair<MethodReference, int> kvp) => 
            kvp.Key;

        [CompilerGenerated]
        private static int <GetReversePInvokeWrappers>m__2(KeyValuePair<MethodReference, int> kvp) => 
            kvp.Value;

        [CompilerGenerated]
        private static MethodReference <GetReversePInvokeWrappers>m__3(KeyValuePair<MethodReference, int> kvp) => 
            kvp.Key;

        [CompilerGenerated]
        private static int <GetTypeMarshalingFunctions>m__6(KeyValuePair<TypeDefinition, int> kvp) => 
            kvp.Value;

        [CompilerGenerated]
        private static TypeDefinition <GetTypeMarshalingFunctions>m__7(KeyValuePair<TypeDefinition, int> kvp) => 
            kvp.Key;

        [CompilerGenerated]
        private static int <GetWrappersForDelegateFromManagedToNative>m__4(KeyValuePair<MethodReference, int> kvp) => 
            kvp.Value;

        [CompilerGenerated]
        private static MethodReference <GetWrappersForDelegateFromManagedToNative>m__5(KeyValuePair<MethodReference, int> kvp) => 
            kvp.Key;

        private void AssertComplete()
        {
            if (!this._complete)
            {
                throw new InvalidOperationException("This method cannot be used until Complete() has been called.");
            }
        }

        private void AssertNotComplete()
        {
            if (this._complete)
            {
                throw new InvalidOperationException("Once Complete() has been called, items cannot be added");
            }
        }

        public void Complete()
        {
            this._complete = true;
        }

        void IMethodCollector.AddCCWMarshallingFunction(TypeDefinition type)
        {
            this.AssertNotComplete();
            object obj2 = this._ccwMarshalingFunctions;
            lock (obj2)
            {
                this._ccwMarshalingFunctions.Add(type, this._ccwMarshalingFunctions.Count);
            }
        }

        void IMethodCollector.AddMethod(MethodReference method)
        {
            this.AssertNotComplete();
            object obj2 = this._methods;
            lock (obj2)
            {
                this._methods.Add(method, this._methods.Count);
            }
        }

        void IMethodCollector.AddReversePInvokeWrapper(MethodReference method)
        {
            this.AssertNotComplete();
            object obj2 = this._reversePInvokeWrappers;
            lock (obj2)
            {
                this._reversePInvokeWrappers.Add(method, this._reversePInvokeWrappers.Count);
            }
        }

        void IMethodCollector.AddTypeMarshallingFunctions(TypeDefinition type)
        {
            this.AssertNotComplete();
            object obj2 = this._typeMarshalingFunctions;
            lock (obj2)
            {
                this._typeMarshalingFunctions.Add(type, this._typeMarshalingFunctions.Count);
            }
        }

        void IMethodCollector.AddWrapperForDelegateFromManagedToNative(MethodReference method)
        {
            this.AssertNotComplete();
            object obj2 = this._delegateWrappersManagedToNative;
            lock (obj2)
            {
                this._delegateWrappersManagedToNative.Add(method, this._delegateWrappersManagedToNative.Count);
            }
        }

        int IMethodCollectorResults.GetCCWMarshalingFunctionIndex(TypeDefinition type)
        {
            int num;
            this.AssertComplete();
            if (this._ccwMarshalingFunctions.TryGetValue(type, out num))
            {
                return num;
            }
            return -1;
        }

        ReadOnlyCollection<TypeDefinition> IMethodCollectorResults.GetCCWMarshalingFunctions()
        {
            this.AssertComplete();
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = new Func<KeyValuePair<TypeDefinition, int>, int>(MethodCollector.<GetCCWMarshalingFunctions>m__8);
            }
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = new Func<KeyValuePair<TypeDefinition, int>, TypeDefinition>(MethodCollector.<GetCCWMarshalingFunctions>m__9);
            }
            return this._ccwMarshalingFunctions.OrderBy<KeyValuePair<TypeDefinition, int>, int>(<>f__am$cache8).Select<KeyValuePair<TypeDefinition, int>, TypeDefinition>(<>f__am$cache9).ToArray<TypeDefinition>().AsReadOnlyPortable<TypeDefinition>();
        }

        int IMethodCollectorResults.GetMethodIndex(MethodReference method)
        {
            int num;
            this.AssertComplete();
            if (this._methods.TryGetValue(method, out num))
            {
                return num;
            }
            return -1;
        }

        ReadOnlyCollection<MethodReference> IMethodCollectorResults.GetMethods()
        {
            this.AssertComplete();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<KeyValuePair<MethodReference, int>, int>(MethodCollector.<GetMethods>m__0);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<KeyValuePair<MethodReference, int>, MethodReference>(MethodCollector.<GetMethods>m__1);
            }
            return this._methods.OrderBy<KeyValuePair<MethodReference, int>, int>(<>f__am$cache0).Select<KeyValuePair<MethodReference, int>, MethodReference>(<>f__am$cache1).ToArray<MethodReference>().AsReadOnlyPortable<MethodReference>();
        }

        int IMethodCollectorResults.GetReversePInvokeWrapperIndex(MethodReference method)
        {
            int num;
            this.AssertComplete();
            if (this._reversePInvokeWrappers.TryGetValue(method, out num))
            {
                return num;
            }
            return -1;
        }

        ReadOnlyCollection<MethodReference> IMethodCollectorResults.GetReversePInvokeWrappers()
        {
            this.AssertComplete();
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<KeyValuePair<MethodReference, int>, int>(MethodCollector.<GetReversePInvokeWrappers>m__2);
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Func<KeyValuePair<MethodReference, int>, MethodReference>(MethodCollector.<GetReversePInvokeWrappers>m__3);
            }
            return this._reversePInvokeWrappers.OrderBy<KeyValuePair<MethodReference, int>, int>(<>f__am$cache2).Select<KeyValuePair<MethodReference, int>, MethodReference>(<>f__am$cache3).ToArray<MethodReference>().AsReadOnlyPortable<MethodReference>();
        }

        ReadOnlyCollection<TypeDefinition> IMethodCollectorResults.GetTypeMarshalingFunctions()
        {
            this.AssertComplete();
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = new Func<KeyValuePair<TypeDefinition, int>, int>(MethodCollector.<GetTypeMarshalingFunctions>m__6);
            }
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = new Func<KeyValuePair<TypeDefinition, int>, TypeDefinition>(MethodCollector.<GetTypeMarshalingFunctions>m__7);
            }
            return this._typeMarshalingFunctions.OrderBy<KeyValuePair<TypeDefinition, int>, int>(<>f__am$cache6).Select<KeyValuePair<TypeDefinition, int>, TypeDefinition>(<>f__am$cache7).ToArray<TypeDefinition>().AsReadOnlyPortable<TypeDefinition>();
        }

        int IMethodCollectorResults.GetTypeMarshalingFunctionsIndex(TypeDefinition type)
        {
            int num;
            this.AssertComplete();
            if (this._typeMarshalingFunctions.TryGetValue(type, out num))
            {
                return num;
            }
            return -1;
        }

        int IMethodCollectorResults.GetWrapperForDelegateFromManagedToNativedIndex(MethodReference method)
        {
            int num;
            this.AssertComplete();
            if (this._delegateWrappersManagedToNative.TryGetValue(method, out num))
            {
                return num;
            }
            return -1;
        }

        ReadOnlyCollection<MethodReference> IMethodCollectorResults.GetWrappersForDelegateFromManagedToNative()
        {
            this.AssertComplete();
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = new Func<KeyValuePair<MethodReference, int>, int>(MethodCollector.<GetWrappersForDelegateFromManagedToNative>m__4);
            }
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = new Func<KeyValuePair<MethodReference, int>, MethodReference>(MethodCollector.<GetWrappersForDelegateFromManagedToNative>m__5);
            }
            return this._delegateWrappersManagedToNative.OrderBy<KeyValuePair<MethodReference, int>, int>(<>f__am$cache4).Select<KeyValuePair<MethodReference, int>, MethodReference>(<>f__am$cache5).ToArray<MethodReference>().AsReadOnlyPortable<MethodReference>();
        }
    }
}

