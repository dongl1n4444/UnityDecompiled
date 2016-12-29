namespace UnityEngine.Events
{
    using System;
    using System.Reflection;
    using System.Threading;
    using UnityEngineInternal;

    internal class InvokableCall<T1> : BaseInvokableCall
    {
        protected event UnityAction<T1> Delegate;

        public InvokableCall(UnityAction<T1> action)
        {
            this.Delegate += action;
        }

        public InvokableCall(object target, MethodInfo theFunction) : base(target, theFunction)
        {
            this.Delegate += ((UnityAction<T1>) theFunction.CreateDelegate(typeof(UnityAction<T1>), target));
        }

        public override bool Find(object targetObj, MethodInfo method) => 
            ((this.Delegate.Target == targetObj) && this.Delegate.GetMethodInfo().Equals(method));

        public override void Invoke(object[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("Passed argument 'args' is invalid size. Expected size is 1");
            }
            BaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
            if (BaseInvokableCall.AllowInvoke(this.Delegate))
            {
                this.Delegate((T1) args[0]);
            }
        }
    }
}

