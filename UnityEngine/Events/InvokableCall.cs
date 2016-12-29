namespace UnityEngine.Events
{
    using System;
    using System.Reflection;
    using System.Threading;
    using UnityEngineInternal;

    internal class InvokableCall : BaseInvokableCall
    {
        private event UnityAction Delegate;

        public InvokableCall(UnityAction action)
        {
            this.Delegate += action;
        }

        public InvokableCall(object target, MethodInfo theFunction) : base(target, theFunction)
        {
            this.Delegate += ((UnityAction) theFunction.CreateDelegate(typeof(UnityAction), target));
        }

        public override bool Find(object targetObj, MethodInfo method) => 
            ((this.Delegate.Target == targetObj) && this.Delegate.GetMethodInfo().Equals(method));

        public override void Invoke(object[] args)
        {
            if (BaseInvokableCall.AllowInvoke(this.Delegate))
            {
                this.Delegate();
            }
        }
    }
}

