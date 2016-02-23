using System;
using System.Reflection;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine.Events
{
	/// <summary>
	///   <para>Two argument version of UnityEvent.</para>
	/// </summary>
	[Serializable]
	public abstract class UnityEvent<T0, T1> : UnityEventBase
	{
		private readonly object[] m_InvokeArray = new object[2];

		[RequiredByNativeCode]
		public UnityEvent()
		{
		}

		public void AddListener(UnityAction<T0, T1> call)
		{
			base.AddCall(UnityEvent<T0, T1>.GetDelegate(call));
		}

		public void RemoveListener(UnityAction<T0, T1> call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[]
			{
				typeof(T0),
				typeof(T1)
			});
		}

		internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall<T0, T1>(target, theFunction);
		}

		private static BaseInvokableCall GetDelegate(UnityAction<T0, T1> action)
		{
			return new InvokableCall<T0, T1>(action);
		}

		public void Invoke(T0 arg0, T1 arg1)
		{
			this.m_InvokeArray[0] = arg0;
			this.m_InvokeArray[1] = arg1;
			base.Invoke(this.m_InvokeArray);
		}

		internal void AddPersistentListener(UnityAction<T0, T1> call)
		{
			this.AddPersistentListener(call, UnityEventCallState.RuntimeOnly);
		}

		internal void AddPersistentListener(UnityAction<T0, T1> call, UnityEventCallState callState)
		{
			int persistentEventCount = base.GetPersistentEventCount();
			base.AddPersistentListener();
			this.RegisterPersistentListener(persistentEventCount, call);
			base.SetPersistentListenerState(persistentEventCount, callState);
		}

		internal void RegisterPersistentListener(int index, UnityAction<T0, T1> call)
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
