using System;
using System.Reflection;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine.Events
{
	/// <summary>
	///   <para>One argument version of UnityEvent.</para>
	/// </summary>
	[Serializable]
	public abstract class UnityEvent<T0> : UnityEventBase
	{
		private readonly object[] m_InvokeArray = new object[1];

		[RequiredByNativeCode]
		public UnityEvent()
		{
		}

		public void AddListener(UnityAction<T0> call)
		{
			base.AddCall(UnityEvent<T0>.GetDelegate(call));
		}

		public void RemoveListener(UnityAction<T0> call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[]
			{
				typeof(T0)
			});
		}

		internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall<T0>(target, theFunction);
		}

		private static BaseInvokableCall GetDelegate(UnityAction<T0> action)
		{
			return new InvokableCall<T0>(action);
		}

		public void Invoke(T0 arg0)
		{
			this.m_InvokeArray[0] = arg0;
			base.Invoke(this.m_InvokeArray);
		}

		internal void AddPersistentListener(UnityAction<T0> call)
		{
			this.AddPersistentListener(call, UnityEventCallState.RuntimeOnly);
		}

		internal void AddPersistentListener(UnityAction<T0> call, UnityEventCallState callState)
		{
			int persistentEventCount = base.GetPersistentEventCount();
			base.AddPersistentListener();
			this.RegisterPersistentListener(persistentEventCount, call);
			base.SetPersistentListenerState(persistentEventCount, callState);
		}

		internal void RegisterPersistentListener(int index, UnityAction<T0> call)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
				return;
			}
			base.RegisterPersistentListener(index, call.Target as Object, call.Method);
		}
	}
}
