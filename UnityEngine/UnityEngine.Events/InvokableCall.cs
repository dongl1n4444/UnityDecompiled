using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngineInternal;

namespace UnityEngine.Events
{
	internal class InvokableCall : BaseInvokableCall
	{
		private event UnityAction Delegate
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.Delegate = (UnityAction)System.Delegate.Combine(this.Delegate, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.Delegate = (UnityAction)System.Delegate.Remove(this.Delegate, value);
			}
		}

		public InvokableCall(object target, MethodInfo theFunction) : base(target, theFunction)
		{
			this.Delegate = (UnityAction)System.Delegate.Combine(this.Delegate, (UnityAction)theFunction.CreateDelegate(typeof(UnityAction), target));
		}

		public InvokableCall(UnityAction action)
		{
			this.Delegate = (UnityAction)System.Delegate.Combine(this.Delegate, action);
		}

		public override void Invoke(object[] args)
		{
			if (BaseInvokableCall.AllowInvoke(this.Delegate))
			{
				this.Delegate();
			}
		}

		public override bool Find(object targetObj, MethodInfo method)
		{
			return this.Delegate.Target == targetObj && this.Delegate.GetMethodInfo() == method;
		}
	}
}
