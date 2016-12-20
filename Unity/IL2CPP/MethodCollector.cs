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
        private static int <GetCCWMarshalingFunctions>m__8(KeyValuePair<TypeDefinition, int> kvp)
        {
            return kvp.Value;
        }

        [CompilerGenerated]
        private static TypeDefinition <GetCCWMarshalingFunctions>m__9(KeyValuePair<TypeDefinition, int> kvp)
        {
            return kvp.Key;
        }

        [CompilerGenerated]
        private static int <GetMethods>m__0(KeyValuePair<MethodReference, int> kvp)
        {
            return kvp.Value;
        }

        [CompilerGenerated]
        private static MethodReference <GetMethods>m__1(KeyValuePair<MethodReference, int> kvp)
        {
            return kvp.Key;
        }

        [CompilerGenerated]
        private static int <GetReversePInvokeWrappers>m__2(KeyValuePair<MethodReference, int> kvp)
        {
            return kvp.Value;
        }

        [CompilerGenerated]
        private static MethodReference <GetReversePInvokeWrappers>m__3(KeyValuePair<MethodReference, int> kvp)
        {
            return kvp.Key;
        }

        [CompilerGenerated]
        private static int <GetTypeMarshalingFunctions>m__6(KeyValuePair<TypeDefinition, int> kvp)
        {
            return kvp.Value;
        }

        [CompilerGenerated]
        private static TypeDefinition <GetTypeMarshalingFunctions>m__7(KeyValuePair<TypeDefinition, int> kvp)
        {
            return kvp.Key;
        }

        [CompilerGenerated]
        private static int <GetWrappersForDelegateFromManagedToNative>m__4(KeyValuePair<MethodReference, int> kvp)
        {
            return kvp.Value;
        }

        [CompilerGenerated]
        private static MethodReference <GetWrappersForDelegateFromManagedToNative>m__5(KeyValuePair<MethodReference, int> kvp)
        {
            return kvp.Key;
        }

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
                <>f__am$cache8 = new Func<KeyValuePair<TypeDefinition, int>, int>(null, (IntPtr) <GetCCWMarshalingFunctions>m__8);
            }
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = new Func<KeyValuePair<TypeDefinition, int>, TypeDefinition>(null, (IntPtr) <GetCCWMarshalingFunctions>m__9);
            }
            return CollectionExtensions.AsReadOnlyPortable<TypeDefinition>(Enumerable.ToArray<TypeDefinition>(Enumerable.Select<KeyValuePair<TypeDefinition, int>, TypeDefinition>(Enumerable.OrderBy<KeyValuePair<TypeDefinition, int>, int>(this._ccwMarshalingFunctions, <>f__am$cache8), <>f__am$cache9)));
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
                <>f__am$cache0 = new Func<KeyValuePair<MethodReference, int>, int>(null, (IntPtr) <GetMethods>m__0);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<KeyValuePair<MethodReference, int>, MethodReference>(null, (IntPtr) <GetMethods>m__1);
            }
            return CollectionExtensions.AsReadOnlyPortable<MethodReference>(Enumerable.ToArray<MethodReference>(Enumerable.Select<KeyValuePair<MethodReference, int>, MethodReference>(Enumerable.OrderBy<KeyValuePair<MethodReference, int>, int>(this._methods, <>f__am$cache0), <>f__am$cache1)));
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
                <>f__am$cache2 = new Func<KeyValuePair<MethodReference, int>, int>(null, (IntPtr) <GetReversePInvokeWrappers>m__2);
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Func<KeyValuePair<MethodReference, int>, MethodReference>(null, (IntPtr) <GetReversePInvokeWrappers>m__3);
            }
            return CollectionExtensions.AsReadOnlyPortable<MethodReference>(Enumerable.ToArray<MethodReference>(Enumerable.Select<KeyValuePair<MethodReference, int>, MethodReference>(Enumerable.OrderBy<KeyValuePair<MethodReference, int>, int>(this._reversePInvokeWrappers, <>f__am$cache2), <>f__am$cache3)));
        }

        ReadOnlyCollection<TypeDefinition> IMethodCollectorResults.GetTypeMarshalingFunctions()
        {
            this.AssertComplete();
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = new Func<KeyValuePair<TypeDefinition, int>, int>(null, (IntPtr) <GetTypeMarshalingFunctions>m__6);
            }
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = new Func<KeyValuePair<TypeDefinition, int>, TypeDefinition>(null, (IntPtr) <GetTypeMarshalingFunctions>m__7);
            }
            return CollectionExtensions.AsReadOnlyPortable<TypeDefinition>(Enumerable.ToArray<TypeDefinition>(Enumerable.Select<KeyValuePair<TypeDefinition, int>, TypeDefinition>(Enumerable.OrderBy<KeyValuePair<TypeDefinition, int>, int>(this._typeMarshalingFunctions, <>f__am$cache6), <>f__am$cache7)));
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
                <>f__am$cache4 = new Func<KeyValuePair<MethodReference, int>, int>(null, (IntPtr) <GetWrappersForDelegateFromManagedToNative>m__4);
            }
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = new Func<KeyValuePair<MethodReference, int>, MethodReference>(null, (IntPtr) <GetWrappersForDelegateFromManagedToNative>m__5);
            }
            return CollectionExtensions.AsReadOnlyPortable<MethodReference>(Enumerable.ToArray<MethodReference>(Enumerable.Select<KeyValuePair<MethodReference, int>, MethodReference>(Enumerable.OrderBy<KeyValuePair<MethodReference, int>, int>(this._delegateWrappersManagedToNative, <>f__am$cache4), <>f__am$cache5)));
        }
    }
}

