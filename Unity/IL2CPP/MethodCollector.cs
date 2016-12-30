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
        private bool _complete;
        private readonly Dictionary<MethodReference, int> _methods = new Dictionary<MethodReference, int>(new MethodReferenceComparer());
        [CompilerGenerated]
        private static Func<KeyValuePair<MethodReference, int>, int> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<KeyValuePair<MethodReference, int>, MethodReference> <>f__am$cache1;

        [CompilerGenerated]
        private static int <GetMethods>m__0(KeyValuePair<MethodReference, int> kvp) => 
            kvp.Value;

        [CompilerGenerated]
        private static MethodReference <GetMethods>m__1(KeyValuePair<MethodReference, int> kvp) => 
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

        void IMethodCollector.AddMethod(MethodReference method)
        {
            this.AssertNotComplete();
            object obj2 = this._methods;
            lock (obj2)
            {
                this._methods.Add(method, this._methods.Count);
            }
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
    }
}

