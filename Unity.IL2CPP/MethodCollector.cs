using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.Portability;

namespace Unity.IL2CPP
{
	public class MethodCollector : IMethodCollector, IMethodCollectorResults
	{
		private readonly Dictionary<MethodReference, int> _methods = new Dictionary<MethodReference, int>(new MethodReferenceComparer());

		private readonly Dictionary<MethodReference, int> _reversePInvokeWrappers = new Dictionary<MethodReference, int>();

		private readonly Dictionary<MethodReference, int> _delegateWrappersManagedToNative = new Dictionary<MethodReference, int>();

		private readonly Dictionary<TypeDefinition, int> _typeMarshalingFunctions = new Dictionary<TypeDefinition, int>();

		private readonly Dictionary<TypeDefinition, int> _ccwMarshalingFunctions = new Dictionary<TypeDefinition, int>();

		private bool _complete;

		public void Complete()
		{
			this._complete = true;
		}

		void IMethodCollector.AddMethod(MethodReference method)
		{
			this.AssertNotComplete();
			object methods = this._methods;
			lock (methods)
			{
				this._methods.Add(method, this._methods.Count);
			}
		}

		void IMethodCollector.AddReversePInvokeWrapper(MethodReference method)
		{
			this.AssertNotComplete();
			object reversePInvokeWrappers = this._reversePInvokeWrappers;
			lock (reversePInvokeWrappers)
			{
				this._reversePInvokeWrappers.Add(method, this._reversePInvokeWrappers.Count);
			}
		}

		void IMethodCollector.AddWrapperForDelegateFromManagedToNative(MethodReference method)
		{
			this.AssertNotComplete();
			object delegateWrappersManagedToNative = this._delegateWrappersManagedToNative;
			lock (delegateWrappersManagedToNative)
			{
				this._delegateWrappersManagedToNative.Add(method, this._delegateWrappersManagedToNative.Count);
			}
		}

		void IMethodCollector.AddTypeMarshallingFunctions(TypeDefinition type)
		{
			this.AssertNotComplete();
			object typeMarshalingFunctions = this._typeMarshalingFunctions;
			lock (typeMarshalingFunctions)
			{
				this._typeMarshalingFunctions.Add(type, this._typeMarshalingFunctions.Count);
			}
		}

		void IMethodCollector.AddCCWMarshallingFunction(TypeDefinition type)
		{
			this.AssertNotComplete();
			object ccwMarshalingFunctions = this._ccwMarshalingFunctions;
			lock (ccwMarshalingFunctions)
			{
				this._ccwMarshalingFunctions.Add(type, this._ccwMarshalingFunctions.Count);
			}
		}

		ReadOnlyCollection<MethodReference> IMethodCollectorResults.GetMethods()
		{
			this.AssertComplete();
			return (from kvp in this._methods
			orderby kvp.Value
			select kvp.Key).ToArray<MethodReference>().AsReadOnlyPortable<MethodReference>();
		}

		int IMethodCollectorResults.GetMethodIndex(MethodReference method)
		{
			this.AssertComplete();
			int num;
			int result;
			if (this._methods.TryGetValue(method, out num))
			{
				result = num;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		ReadOnlyCollection<MethodReference> IMethodCollectorResults.GetReversePInvokeWrappers()
		{
			this.AssertComplete();
			return (from kvp in this._reversePInvokeWrappers
			orderby kvp.Value
			select kvp.Key).ToArray<MethodReference>().AsReadOnlyPortable<MethodReference>();
		}

		int IMethodCollectorResults.GetReversePInvokeWrapperIndex(MethodReference method)
		{
			this.AssertComplete();
			int num;
			int result;
			if (this._reversePInvokeWrappers.TryGetValue(method, out num))
			{
				result = num;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		ReadOnlyCollection<MethodReference> IMethodCollectorResults.GetWrappersForDelegateFromManagedToNative()
		{
			this.AssertComplete();
			return (from kvp in this._delegateWrappersManagedToNative
			orderby kvp.Value
			select kvp.Key).ToArray<MethodReference>().AsReadOnlyPortable<MethodReference>();
		}

		int IMethodCollectorResults.GetWrapperForDelegateFromManagedToNativedIndex(MethodReference method)
		{
			this.AssertComplete();
			int num;
			int result;
			if (this._delegateWrappersManagedToNative.TryGetValue(method, out num))
			{
				result = num;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		ReadOnlyCollection<TypeDefinition> IMethodCollectorResults.GetTypeMarshalingFunctions()
		{
			this.AssertComplete();
			return (from kvp in this._typeMarshalingFunctions
			orderby kvp.Value
			select kvp.Key).ToArray<TypeDefinition>().AsReadOnlyPortable<TypeDefinition>();
		}

		int IMethodCollectorResults.GetTypeMarshalingFunctionsIndex(TypeDefinition type)
		{
			this.AssertComplete();
			int num;
			int result;
			if (this._typeMarshalingFunctions.TryGetValue(type, out num))
			{
				result = num;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		ReadOnlyCollection<TypeDefinition> IMethodCollectorResults.GetCCWMarshalingFunctions()
		{
			this.AssertComplete();
			return (from kvp in this._ccwMarshalingFunctions
			orderby kvp.Value
			select kvp.Key).ToArray<TypeDefinition>().AsReadOnlyPortable<TypeDefinition>();
		}

		int IMethodCollectorResults.GetCCWMarshalingFunctionIndex(TypeDefinition type)
		{
			this.AssertComplete();
			int num;
			int result;
			if (this._ccwMarshalingFunctions.TryGetValue(type, out num))
			{
				result = num;
			}
			else
			{
				result = -1;
			}
			return result;
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
	}
}
